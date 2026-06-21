namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    // Y - mirrors one row in BoardMembers (many-to-many between boards and users)
    public class UserBoardsDTO
    {
        public int BoardId { get; }
        public string UserEmail { get; }

        public UserBoardsDTO(int boardId, string userEmail)
        {
            BoardId = boardId;
            UserEmail = userEmail;
        }
    }
}
