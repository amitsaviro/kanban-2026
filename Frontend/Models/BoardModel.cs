namespace Frontend.Models
{
    /// <summary>
    /// A Frontend-side representation of a board, assembled from several separate
    /// service-layer calls (there is no single service method that returns a full board
    /// object - see <see cref="Frontend.ViewModels.BoardsListViewModel"/>).
    /// </summary>
    // A- unlike TaskModel, this class is NOT a direct 1:1 mirror of a single JSON response:
    // A- BoardService.GetUserBoards only returns a List<int> of board IDs, so BoardsListViewModel
    // A- builds one BoardModel per ID by separately calling GetBoardName(id) and GetBoardOwner(id).
    public class BoardModel
    {
        /// <summary>The board's system-wide unique ID. Used only internally to call service methods - never shown in the UI (Requirement 29.c).</summary>
        public int Id { get; set; }

        /// <summary>The board's name, shown in the boards list (Requirement 29.b).</summary>
        public string Name { get; set; }

        /// <summary>The email of the board's owner, shown in the boards list (Requirement 29.b).</summary>
        public string OwnerEmail { get; set; }
    }
}
