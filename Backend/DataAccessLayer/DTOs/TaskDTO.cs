namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    // Y - mirrors the Task table row; dates are TEXT in SQLite so we store them as ISO-8601 strings
    public class TaskDTO
    {
        public int Id { get; }
        public int BoardId { get; }
        public int ColumnOrdinal { get; }
        public string Title { get; }
        public string Description { get; }
        public string DueDate { get; }
        public string CreationTime { get; }
        public string AssigneeEmail { get; }

        public TaskDTO(int id, int boardId, int columnOrdinal, string title, string description,
                       string dueDate, string creationTime, string assigneeEmail)
        {
            Id = id;
            BoardId = boardId;
            ColumnOrdinal = columnOrdinal;
            Title = title;
            Description = description;
            DueDate = dueDate;
            CreationTime = creationTime;
            AssigneeEmail = assigneeEmail;
        }
    }
}
