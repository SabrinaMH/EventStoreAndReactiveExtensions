using System;
using EventStore.ClientAPI;
using static System.Console;

namespace Projections
{
    public class CatchUpSubscription
    {
        private readonly string _streamName;

        public CatchUpSubscription(string streamName)
        {
            _streamName = streamName;
        }

        public void Run(IEventStoreConnection connection)
        {
            Console.WriteLine($"Reading stream {_streamName}");

            var catchUpSubscriptionSettings = new CatchUpSubscriptionSettings(2, 2, true, true);
            var subscription = connection.SubscribeToStreamFrom(_streamName, 0, catchUpSubscriptionSettings,
                EventAppeared, null,
                (_, reason, ex) =>
                {
                    Console.WriteLine($"Subscription dropped.\nReason {reason}.\nException {ex}");

                });
        }

        void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            var deserializeEvent = CommonLibrary.EventStoreUtils.DeserializeEvent(@event);
            WriteLine(deserializeEvent);
        }
    }
}