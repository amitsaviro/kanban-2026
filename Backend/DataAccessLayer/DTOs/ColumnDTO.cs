namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>Represents one row in the Column table (stores per-column task limits).</summary>
    public class ColumnDTO
    {
        public int BoardId { get; }
        public int Ordinal { get; }
        public int Limit { get; }

        public ColumnDTO(int boardId, int ordinal, int limit)
        {
            BoardId = boardId;
            Ordinal = ordinal;
            Limit = limit;
        }
    }
}
