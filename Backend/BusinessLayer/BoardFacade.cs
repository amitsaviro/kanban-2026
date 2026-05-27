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

        // Y - logger for tracking important events and errors (the one allowed static field)
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default constructor — creates its own UserFacade (used when BoardService is created standalone).
        /// </summary>
        public BoardFacade()
        {
            // Y - standalone mode: each service manages its own user state
            _userFacade = new UserFacade();
        }
        /// <summary>
        /// Injection constructor — shares a UserFacade from outside (used by GradingService so all services see the same users).
        /// </summary>
        /// <param name="userFacade">The shared UserFacade instance.</param>
        public BoardFacade(UserFacade userFacade)
        {
            // Y - shared mode: GradingService passes in the same UserFacade it gave to UserService
            _userFacade = userFacade;
        }

        /// <summary>
        /// Creates a new board for the user.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the new board, must be unique for this user.</param>
        /// <exception cref="ArgumentException">Thrown if board name is invalid or already exists.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public void CreateBoard(string email, string boardName)
        {
            // Y - GetLoggedInUser checks both that the user exists AND that they are logged in
            User user = _userFacade.GetLoggedInUser(email);
            user.AddBoard(boardName);
            log.Info($"Board '{boardName}' created for user '{email}'.");
        }

        /// <summary>
        /// Deletes an existing board.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board to delete.</param>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public void DeleteBoard(string email, string boardName)
        {
            User user = _userFacade.GetLoggedInUser(email);
            user.RemoveBoard(boardName);
            log.Info($"Board '{boardName}' deleted for user '{email}'.");
        }

        /// <summary>
        /// Sets the task limit on a specific column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Column index: 0=backlog, 1=in progress, 2=done.</param>
        /// <param name="limit">Max tasks in the column. -1 means no limit.</param>
        /// <exception cref="ArgumentException">Thrown if board/column not found or limit is invalid.</exception>
        public void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            board.LimitColumn(columnOrdinal, limit);
            log.Info($"Column {columnOrdinal} of board '{boardName}' limited to {limit} tasks.");
        }

        /// <summary>
        /// Gets the task limit of a specific column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Column index.</param>
        /// <returns>The limit (-1 if no limit).</returns>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            // Y - board.GetColumn returns the Column object, then we read its Limit property
            return board.GetColumn(columnOrdinal).Limit;
        }

        /// <summary>
        /// Gets the name of a specific column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Column index.</param>
        /// <returns>The column name e.g. "backlog".</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            return board.GetColumn(columnOrdinal).Name;
        }

        /// <summary>
        /// Returns all tasks in a specific column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Column index.</param>
        /// <returns>List of tasks in that column.</returns>
        public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            return board.GetColumn(columnOrdinal).Tasks;
        }
    }
}
