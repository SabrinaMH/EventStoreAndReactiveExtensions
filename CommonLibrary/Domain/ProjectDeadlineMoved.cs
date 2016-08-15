namespace CommonLibrary.Domain
{
    public class ProjectDeadlineMoved : Event
    {
        public string ProjectId { get; private set; }
        public string OldPriority { get; private set; }
        public string NewDeadline { get; private set; }

        public ProjectDeadlineMoved(string projectId, string oldPriority, string newDeadline)
        {
            ProjectId = projectId;
            OldPriority = oldPriority;
            NewDeadline = newDeadline;
        }
    }
}