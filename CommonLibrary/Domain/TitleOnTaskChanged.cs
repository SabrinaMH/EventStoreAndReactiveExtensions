namespace CommonLibrary.Domain
{
    public class TitleOnTaskChanged : Event
    {
        public string NewTitle { get; private set; }
        public string TaskId { get; private set; }

        public TitleOnTaskChanged(string taskId, string newTitle)
        {
            TaskId = taskId;
            NewTitle = newTitle;
        }
        
    }
}