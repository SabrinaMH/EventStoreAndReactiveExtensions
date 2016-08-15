using System;
using System.Reactive.Disposables;
using CommonLibrary;
using EventStore.ClientAPI;

namespace ReactiveExtensions
{
    public class CatchUpSubscriptionObservable<T> : IObservable<T> where T : class
    {
        EventStoreAllCatchUpSubscription _catchUpSubscription;
        private IObserver<T> _observer;

        public CatchUpSubscriptionObservable(IEventStoreConnection connection)
        {
            _catchUpSubscription = connection.SubscribeToAllFrom(Position.Start, true, EventAppeared, SubscriptionDropped);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;
            return Disposable.Create(Stop);
        }

        private void Stop()
        {
            _catchUpSubscription.Stop();
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription)
        {
        }

        private void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            Program.NumberOfEventsProcessed++;

            T deserializedEvent = null;
            try
            {
                deserializedEvent = EventStoreUtils.DeserializeEvent<T>(@event);
            }
            catch (Exception ex)
            {
                // System event. Ignore.
                return;
            }

            if (deserializedEvent?.GetType() == typeof(T))
            {
                _observer.OnNext(deserializedEvent);
            }
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason,
            Exception e)
        {
            if (e != null)
            {
                _observer.OnError(e);
            }
            else
            {
                _observer.OnCompleted();
            }
        }
    }
}