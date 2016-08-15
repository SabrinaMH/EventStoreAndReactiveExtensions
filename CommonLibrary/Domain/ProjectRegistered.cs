namespace CommonLibrary.Domain
{
    public class ProjectRegistered : Event
    {
        public string Deadline { get; private set; }
        public string User { get; set; }
        public string ProjectId { get; private set; }
        public string Title { get; private set; }

        public ProjectRegistered() { }

        public ProjectRegistered(string projectId, string title, string deadline, string user)
        {
            ProjectId = projectId;
            Title = title;
            Deadline = deadline;
            User = user;
        }
    }
}