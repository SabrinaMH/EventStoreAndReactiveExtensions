using System.Reactive.Subjects;
using EventStore.ClientAPI;

namespace ReactiveExtensions
{
    public class EventStoreRxSubscription
    {
        public Subject<ResolvedEvent> ResolvedEvents { get; }
        public Subject<SubscriptionDropReason> DroppedReasons { get; }
        public EventStoreSubscription Subscription { get; }

        public EventStoreRxSubscription(EventStoreSubscription subscription, Subject<ResolvedEvent> resolvedEvent, Subject<SubscriptionDropReason> droppedReasons)
        {
            Subscription = subscription;
            ResolvedEvents = resolvedEvent;
            DroppedReasons = droppedReasons;
        }
    }
}