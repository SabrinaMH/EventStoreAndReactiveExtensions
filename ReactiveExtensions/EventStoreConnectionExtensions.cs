using System.Reactive.Subjects;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ReactiveExtensions
{
    static class EventStoreConnectionExtensions
    {
        public static Task<EventStoreRxSubscription> SubscribeTo(this IEventStoreConnection connection, string streamName, bool resolveLinkTos)
        {
            return Task<EventStoreRxSubscription>.Factory.StartNew(() =>
            {
                var resolvedEvents = new Subject<ResolvedEvent>();
                var droppedReasons = new Subject<SubscriptionDropReason>();

                var subscriptionTask = connection.SubscribeToStreamAsync(streamName, resolveLinkTos,
                    (subscription, @event) => resolvedEvents.OnNext(@event),
                    (subscription, dropReason, arg3) => droppedReasons.OnNext(dropReason));
                subscriptionTask.Wait();

                return new EventStoreRxSubscription(subscriptionTask.Result, resolvedEvents, droppedReasons);
            });
        }
    }
}