using System;
using System.Collections.Generic;
using Backend.BusinessLayer;
using log4net;

namespace Backend.Facades
{
    /// <summary>
    /// Facade for all board-related operations.
    /// Validates user login status, then delegates to User and Board objects.
    /// </summary>
    public class BoardFacade
    {
        // Y - reference to UserFacade so we can look up and validate users before any board operation
        private UserFacade _userFacade;

        // A - System-wide collection of all boards, mapped by their unique ID (Requirement 4).
        private Dictionary<int, Board> _allBoards;
        
        // A - Counter to assign unique IDs to new boards.
        private int _nextBoardId;

        // Y - logger for tracking important events and errors (the one allowed static field)
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default constructor — creates its own UserFacade (used when BoardService is created standalone).
        /// </summary>
        public BoardFacade()
        {
            _userFacade = new UserFacade();
            _allBoards = new Dictionary<int, Board>();
            _nextBoardId = 0;
        }

        /// <summary>
        /// Injection constructor — shares a UserFacade from outside (used by GradingService so all services see the same users).
        /// </summary>
        /// <param name="userFacade">The shared UserFacade instance.</param>
        public BoardFacade(UserFacade userFacade)
        {
            _userFacade = userFacade;
            _allBoards = new Dictionary<int, Board>();
            _nextBoardId = 0;
        }


        /// <summary>
        /// Creates a new board and assigns the creator as the owner.
        /// </summary>
        /// <param name="email">Email of the user creating the board. Must be logged in.</param>
        /// <param name="name">The name of the new board.</param>
        public void AddBoard(string email, string name)
        {
            // A - Validate board name is not null or whitespace (Requirement 26a: no blank names)
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Board name cannot be empty.");

            User user = _userFacade.GetLoggedInUser(email); // Y - Validates user exists and is logged in

            // A - Instantiate the board with a system-wide ID and the creator's email as the owner.
            Board newBoard = new Board(_nextBoardId, name, email);
            
            // A - Link the board to the user's collection. Validation for duplicate names happens inside User.AddBoard.
            user.AddBoard(newBoard);
            
            // A - Add to the global system registry and increment ID.
            _allBoards.Add(_nextBoardId, newBoard);
            _nextBoardId++;
            
            log.Info($"User '{email}' created a new board named '{name}' with ID {_nextBoardId - 1}.");
        }

        /// <summary>
        /// Deletes a board from the system.
        /// </summary>
        /// <param name="email">Email of the user deleting the board. Must be logged in and be the owner.</param>
        /// <param name="name">The name of the board to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user is not the owner of the board.</exception>
        public void RemoveBoard(string email, string name)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(name);

            // A - Only the owner can delete the entire board
            if (board.OwnerEmail != email)
            {
                throw new InvalidOperationException("Only the board owner can delete the board.");
            }

            // A - Remove the board from every member's personal board list, not just the owner's
            foreach (string memberEmail in board.Members)
            {
                try
                {
                    User member = _userFacade.GetUser(memberEmail);
                    member.RemoveBoard(name);
                }
                catch { }  // A - skip if member lookup fails (e.g. user deleted)
            }

            // A - Remove from the global registry
            _allBoards.Remove(board.Id);

            log.Info($"Board '{name}' (ID: {board.Id}) was deleted by owner '{email}'.");
        }

        /// <summary>
        /// Gets the maximum number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column.</param>
        /// <returns>The column limit.</returns>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            return board.GetColumn(columnOrdinal).Limit;
        }

        /// <summary>
        /// Gets the name of a specific column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column.</param>
        /// <returns>The name of the column.</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            return board.GetColumn(columnOrdinal).Name;
        }

        /// <summary>
        /// Returns all tasks in a specific column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column.</param>
        /// <returns>List of tasks in that column.</returns>
        public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            return board.GetColumn(columnOrdinal).Tasks;
        }


        /// <summary>
        /// A - Helper method to fetch a board system-wide by its ID.
        /// </summary>
        /// <param name="boardId">The system-wide unique ID of the board.</param>
        /// <returns>The Board object if found.</returns>
        /// <exception cref="ArgumentException">Thrown if a board with the given ID does not exist.</exception>
        private Board GetBoardById(int boardId)
        {
            if (!_allBoards.TryGetValue(boardId, out Board board))
            {
                log.Warn($"Attempted to access non-existent board with ID: {boardId}");
                throw new ArgumentException($"Board with ID {boardId} does not exist in the system.");
            }
            return board;
        }

        /// <summary>
        /// A - Gets the name of a board by its ID. Required by GradingService.
        /// </summary>
        /// <param name="boardId">The system-wide unique ID of the board.</param>
        /// <returns>The name of the board.</returns>
        public string GetBoardName(int boardId)
        {
            return GetBoardById(boardId).Name;
        }

        /// <summary>
        /// A - Allows a user to join an existing shared board.
        /// </summary>
        /// <param name="email">Email of the user joining the board. Must be logged in.</param>
        /// <param name="boardID">The ID of the board to join.</param>
        public void JoinBoard(string email, int boardID)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = GetBoardById(boardID);

            // A - The Facade does the orchestration: first add member to the board, then link the board to the user.
            board.AddMember(email);
            user.AddBoard(board);
            
            log.Info($"User '{email}' successfully joined board ID {boardID}.");
        }

        /// <summary>
        /// A - Allows a member to leave a shared board.
        /// </summary>
        /// <param name="email">Email of the user leaving the board. Must be logged in.</param>
        /// <param name="boardID">The ID of the board to leave.</param>
        public void LeaveBoard(string email, int boardID)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = GetBoardById(boardID);

            // A - The BL logic in RemoveMember will throw an exception if the user is the owner.
            board.RemoveMember(email);
            
            // A - Unlink from the user's collection.
            user.RemoveBoard(board.Name);

            log.Info($"User '{email}' left board ID {boardID}.");
        }

        /// <summary>
        /// A - Transfers ownership of a board to another member.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in.</param>
        /// <param name="newOwnerEmail">Email of the member to become the new owner.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current user is not the owner.</exception>
        public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            User currentUser = _userFacade.GetLoggedInUser(currentOwnerEmail);
            Board board = currentUser.GetBoard(boardName);

            if (board.OwnerEmail != currentOwnerEmail)
            {
                log.Warn($"User '{currentOwnerEmail}' attempted to transfer ownership of '{boardName}' without being the owner.");
                throw new InvalidOperationException("Only the current board owner can transfer ownership.");
            }

            board.TransferOwnership(newOwnerEmail);
            log.Info($"Ownership of board '{boardName}' transferred from '{currentOwnerEmail}' to '{newOwnerEmail}'.");
        }

        /// <summary>
        /// Wrapper method for BoardService compatibility. Creates a new board.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="name">Board name.</param>
        public void CreateBoard(string email, string name)
        {
            AddBoard(email, name);
        }

        /// <summary>
        /// Wrapper method for BoardService compatibility. Deletes a board.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="name">Board name.</param>
        public void DeleteBoard(string email, string name)
        {
            RemoveBoard(email, name);
        }

        /// <summary>
        /// Sets a limit on the number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">The column ID.</param>
        /// <param name="limit">The new limit.</param>
        public void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            board.LimitColumn(columnOrdinal, limit);
        }

        /// <summary>
        /// A - Returns the IDs of all boards the user is a member of (owned or joined).
        /// Satisfies Requirement: GetUserBoards returns list of IDs of all user's boards.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in.</param>
        /// <returns>A list of integer board IDs.</returns>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public List<int> GetUserBoards(string email)
        {
            User user = _userFacade.GetLoggedInUser(email);
            // A - iterate over all boards the user holds and collect their IDs
            List<int> ids = new List<int>();
            foreach (Board board in user.GetBoards())
                ids.Add(board.Id);
            return ids;
        }
    }
}