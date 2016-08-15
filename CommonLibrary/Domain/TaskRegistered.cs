namespace CommonLibrary.Domain
{
    public class TaskRegistered : Event
    {
        public string User { get; private set; }
        public string TaskId { get; private set; }
        public string ProjectId { get; private set; }
        public string Title { get; private set; }

        public TaskRegistered() { }

        public TaskRegistered(string taskId, string projectId, string title, string user)
        {
            TaskId = taskId;
            ProjectId = projectId;
            Title = title;
            User = user;
        }
    }
}