using System;

namespace Backend.ServiceLayer
{
    /// <summary>
    /// Service layer for managing boards within the Kanban system.
    /// </summary>
    public class BoardService
    {
        private BoardFacade _boardFacade; 
        /// <summary>
        /// Creates a new board for a specific user.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <param name="boardName">The name for the new board (must be unique for this user, case insensitive).</param>
        /// <returns>A JSON string containing the created board details.</returns>
        /// <exception cref="ArgumentException">Thrown if board name is invalid, empty, or already exists for the user.</exception>
        public string CreateBoard(string email, string boardName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a board from the user's account.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <param name="boardName">The name of the board to delete.</param>
        /// <returns>A JSON string confirming the board deletion.</returns>
        /// <exception cref="ArgumentException">Thrown if board does not exist or user is unauthorized.</exception>
        public string DeleteBoard(string email, string boardName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a limit on the number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The column index (0 for backlog, 1 for in progress, 2 for done).</param>
        /// <param name="limit">The maximum number of tasks allowed in this column.</param>
        /// <returns>A JSON string confirming the limit update.</returns>
        /// <exception cref="ArgumentException">Thrown if column ordinal or limit is invalid.</exception>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            throw new NotImplementedException();
        }

        ///////////////////////// yuval- new functions/////////////////
        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with the column's limit, unless an error occurs.</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
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
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            throw new NotImplementedException();
        }
///////////////////////////////////////////////////////////////////////////////////

    
    }
}