using System;
using System.Collections.Generic;

namespace CommonLibrary.Domain
{
    public class Project : AggregateRoot
    {
        private string _deadline;

        public Project(IList<Event> history) : base(history) { }

        public Project(string title, string user) : base($"project_{title}")
        {
            ApplyChange(new ProjectRegistered(Id, title, "none", user));
        }
        
        public void MoveDeadline(DateTime newDeadline)
        {
            ApplyChange(new ProjectDeadlineMoved(Id, _deadline, newDeadline.ToString()));
        }

        private void Apply(ProjectRegistered @event)
        {
            Id = @event.ProjectId;
            _deadline = @event.Deadline;
        }

        private void Apply(ProjectDeadlineMoved @event)
        {
            Id = @event.ProjectId;
            _deadline = @event.NewDeadline;
        }
    }
}