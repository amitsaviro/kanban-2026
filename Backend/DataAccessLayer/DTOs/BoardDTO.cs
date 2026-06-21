namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>Represents one row in the Board table.</summary>
    public class BoardDTO
    {
        public int Id { get; }
        public string Name { get; }
        public string OwnerEmail { get; }
        public int NextTaskId { get; }

        public BoardDTO(int id, string name, string ownerEmail, int nextTaskId)
        {
            Id = id;
            Name = name;
            OwnerEmail = ownerEmail;
            NextTaskId = nextTaskId;
        }
    }
}
