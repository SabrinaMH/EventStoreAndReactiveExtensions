using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CommonLibrary.Domain;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace CommonLibrary
{
    public class EventStoreRepository<TAggregate> where TAggregate : AggregateRoot
    {
        public TAggregate GetById(string id)
        {
            var settings = ConnectionSettings.Create();
            using (var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, EventStoreUtils.DefaultTcpPort)))
            {
                connection.ConnectAsync().Wait();
                var events = new List<Event>();
                StreamEventsSlice currentSlice;
                var nextSliceStart = StreamPosition.Start;
                var streamName = GetStreamName(typeof(TAggregate), id);

                do
                {
                    currentSlice = connection
                        .ReadStreamEventsForwardAsync(streamName, nextSliceStart, EventStoreUtils.ReadPageSize, false)
                        .Result;
                    nextSliceStart = currentSlice.NextEventNumber;
                    events.AddRange(currentSlice.Events.Select(EventStoreUtils.DeserializeEvent));
                } while (!currentSlice.IsEndOfStream);
                if (!events.Any())
                {
                    return null;
                }

                var constructor = typeof(TAggregate).GetConstructor(new Type[] { typeof(IList<Event>) });
                var aggregate = (TAggregate)constructor.Invoke(new object[] { events });
                return aggregate;
            }
        }

        private string GetStreamName(Type type, string id)
        {
            return string.Format("{0}-{1}", type.Name, id);
        }


        public void Save(AggregateRoot aggregate)
        {
            List<Event> newEvents = aggregate.GetUncommittedEvents().ToList();
            var settings = ConnectionSettings.Create();
            using (var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, EventStoreUtils.DefaultTcpPort)))
            {
                connection.ConnectAsync().Wait();
                string streamName = GetStreamName(aggregate.GetType(), aggregate.Id);
                int originalVersion = aggregate.Version - newEvents.Count;
                int expectedVersion = originalVersion == 0 ? ExpectedVersion.NoStream : originalVersion - 1;

                int i = originalVersion;
                newEvents.ForEach(e => e.Version = i++);

                var commitId = Guid.NewGuid();
                var commitHeaders = new Dictionary<string, object>
                {
                    {EventStoreUtils.CommitIdHeader, commitId},
                    {EventStoreUtils.AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
                };

                List<EventData> eventsToSave =
                    newEvents.Select(e => ToEventData(Guid.NewGuid(), e, commitHeaders)).ToList();
                connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave).Wait();
            }

            aggregate.MarkChangesAsCommitted();
        }

        private static EventData ToEventData(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, serializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    EventStoreUtils.EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                }
            };
            byte[] metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, serializerSettings));
            string typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
    }
}