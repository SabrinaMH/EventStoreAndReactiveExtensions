using System;
using System.Collections.Generic;

namespace CommonLibrary.Domain
{
    public class Task : AggregateRoot
    {
        public Task(IList<Event> history) : base(history)
        {
        }

        public Task(string projectId, string title, string user) : base($"{projectId}_{title}")
        {
            ApplyChange(new TaskRegistered(Id, projectId, title, user));
        }
        
        /// <exception cref="ArgumentNullException"><paramref name="newTitle"/> is <see langword="null" />.</exception>
        public void ChangeTitle(string newTitle)
        {
            ApplyChange(new TitleOnTaskChanged(Id, newTitle));
        }
        
        private void Apply(TaskRegistered @event)
        {
            Id = @event.TaskId;
        }
        
        private void Apply(TitleOnTaskChanged @event)
        {
        }
    }
}