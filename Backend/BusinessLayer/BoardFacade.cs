using System;
using Backend.BusinessLayer;
using System.Collections.Generic;
using System.Text.Json;

namespace Backend.Facades
{
    /// <summary>
    /// Facade for board management operations.
    /// Acts as an entry point for board-related logic.
    /// </summary>
    public class BoardFacade
    {
        private UserFacade _userFacade; 


        public BoardFacade(UserFacade userFacade) { this._userFacade = userFacade; }

        /// <summary>
        /// Creates a new board for the specified user.
        /// </summary>
        /// <param name="email">The email of the user (must be logged in).</param>
        /// <param name="boardName">The name of the new board.</param>
        /// <remarks>
        /// Precondition: User must be logged in. Board name must be unique for this user.
        /// Postcondition: A new empty board is added to the user's board list.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if board name already exists or user is not logged in.</exception>
        public void CreateBoard(string email, string boardName)
        {
            if (_userFacade.isLoggedIn(email) && _userFacade.GetUser(email).GetBoard(boardName) == null)
                _userFacade.GetUser(email).AddBoard(boardName);
            else
                throw new AggregateException("board name already exists or user is not logged in");
        }

        /// <summary>
        /// Deletes an existing board.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="boardName">The name of the board to delete.</param>
        /// <remarks>
        /// Precondition: User must be the owner of the board and logged in.
        /// Postcondition: The board is removed from the user's system.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if board does not exist or user is not the owner.</exception>
        public void DeleteBoard(string email, string boardName)
        {
            if (_userFacade.GetUser(email).GetBoard(boardName) == null || !_userFacade.isLoggedIn(email))
                throw new ArgumentException("board does not exist or user is not the owner");
            _userFacade.GetUser(email).RemoveBoard(boardName);
        }



        /// <summary>
        /// Sets a new task limit for a specific column in a board.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="boardName">The board name.</param>
        /// <param name="columnOrdinal">The column index (0 for backlog, etc.).</param>
        /// <param name="limit">The new limit (negative value means no limit).</param>
        /// <remarks>
        /// Precondition: Board exists, column ordinal is valid.
        /// Postcondition: The column's capacity limit is updated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if limit is invalid or column does not exist.</exception>
        public void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            if(_userFacade.GetUser(email).GetBoard(boardName) == null)
                throw new ArgumentException("there is no board");
            _userFacade.GetUser(email).GetBoard(boardName).LimitColumn(columnOrdinal, limit);
        }


        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with the column's limit, unless an error occurs.</returns>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method gets the name of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with the column's name, unless an error occurs.</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns a column given its name.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with a list of the column's tasks, unless an error occurs.</returns>
        public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            throw new NotImplementedException();
        }
    }
}