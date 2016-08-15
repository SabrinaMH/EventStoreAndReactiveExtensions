using System;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonLibrary
{
    public class EventStoreUtils
    {
        public const string EventClrTypeHeader = "EventClrTypeName";
        public const string AggregateClrTypeHeader = "AggregateClrTypeName";
        public const string CommitIdHeader = "CommitId";
        public const int ReadPageSize = 200;
        public const int DefaultTcpPort = 1113;
        public const int DefaultHttpPort = 2113;
        public static UserCredentials UserCredentials = new UserCredentials("admin", "changeit");

        public static Event DeserializeEvent(ResolvedEvent e)
        {
            var eventClrTypeName =
                JObject.Parse(Encoding.UTF8.GetString(e.Event.Metadata)).Property(EventClrTypeHeader).Value;
            string data = Encoding.UTF8.GetString(e.Event.Data);

            Type type = Type.GetType((string)eventClrTypeName);
            var obj = JsonConvert.DeserializeObject(data, type);
            return (Event)obj;
        }

        public static T DeserializeEvent<T>(ResolvedEvent e) where T : class
        {
            return DeserializeEvent(e) as T;
        }
    }
}
