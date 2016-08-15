using System.Collections.Generic;

namespace CommonLibrary.Domain
{
    public abstract class AggregateRoot
    {
        private readonly IList<Event> _changes = new List<Event>();
        public string Id { get; protected set; }

        private int _version = 0;
        public int Version { get { return _version; } }

        public AggregateRoot(string id)
        {
            Id = id;
        }

        public AggregateRoot(IList<Event> history)
        {
            foreach (Event @event in history)
            {
                ApplyChange(@event, false);
            }
        }

        public IList<Event> GetUncommittedEvents()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            _version++;
            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }
    }


}