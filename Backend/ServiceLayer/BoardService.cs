using Backend.BusinessLayer;
using Backend.Facades;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Task = Backend.BusinessLayer.Task;

namespace Backend.ServiceLayer
{
    /// <summary>
    /// Service layer for managing boards within the Kanban system.
    /// Acts as the entry point for API requests, handling JSON serialization and exception translation.
    /// </summary>
    public class BoardService
    {
        private BoardFacade _boardFacade;

        /// <summary>
        /// Initializes a new instance of the BoardService class and its underlying Facade.
        /// </summary>
        public BoardService()
        {
            _boardFacade = new BoardFacade();
        }

        // Y - injection constructor: allows GradingService to pass a BoardFacade that shares a UserFacade
        public BoardService(BoardFacade boardFacade)
        {
            _boardFacade = boardFacade;
        }

        /// <summary>
        /// Creates a new board for a specific user.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <param name="boardName">The name for the new board (must be unique for this user, case insensitive).</param>
        /// <returns>A JSON string containing the created board details.</returns>
        /// <exception cref="ArgumentException">Thrown if board name is invalid, empty, or already exists for the user.</exception>
        public string CreateBoard(string email, string boardName)
        {
            try
            {
                _boardFacade.CreateBoard(email, boardName);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Deletes a board. Only the board owner can delete it; all tasks are removed.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in and be the board owner.</param>
        /// <param name="boardName">The name of the board to delete.</param>
        /// <returns>A JSON string confirming the board deletion.</returns>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not the board owner or not logged in.</exception>
        public string DeleteBoard(string email, string boardName)
        {
            try
            {
                _boardFacade.DeleteBoard(email, boardName);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
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
            try
            {
                _boardFacade.LimitColumn(email, boardName, columnOrdinal, limit);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with the column's limit, unless an error occurs.</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            try
            {
                int limit = _boardFacade.GetColumnLimit(email, boardName, columnOrdinal);
                var response = new { ErrorMessage = (string)null, ReturnValue = limit };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
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
            try
            {
                string name = _boardFacade.GetColumnName(email, boardName, columnOrdinal);
                var response = new { ErrorMessage = (string)null, ReturnValue = name };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// This method returns a column given its ordinal.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0.</param>
        /// <returns>A JSON string with a list of the column's tasks, unless an error occurs.</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                List<Task> tasks = _boardFacade.GetColumn(email, boardName, columnOrdinal);
                var response = new { ErrorMessage = (string)null, ReturnValue = tasks };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Returns a list of IDs of all boards the user is a member of (owned or joined).
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in.</param>
        /// <returns>A JSON string with a list of integer board IDs on success, or an error message.</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                // A - Delegate to BoardFacade to collect IDs of all boards the user belongs to (Requirement: GetUserBoards)
                List<int> ids = _boardFacade.GetUserBoards(email);
                var response = new { ErrorMessage = (string)null, ReturnValue = ids };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Adds the user as a member of an existing board, identified by its global ID.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in.</param>
        /// <param name="boardID">The unique integer ID of the board to join.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        public string JoinBoard(string email, int boardID)
        {
            try
            {
                _boardFacade.JoinBoard(email, boardID);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Removes the user from a board they are a member of.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in and be a board member.</param>
        /// <param name="boardID">The unique integer ID of the board to leave.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        public string LeaveBoard(string email, int boardID)
        {
            try
            {
                _boardFacade.LeaveBoard(email, boardID);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Returns the name of a board given its global unique ID.
        /// </summary>
        /// <param name="boardId">The unique integer ID of the board.</param>
        /// <returns>A JSON string with the board's name on success, or an error message.</returns>
        public string GetBoardName(int boardId)
        {
            try
            {
                string name = _boardFacade.GetBoardName(boardId);
                var response = new { ErrorMessage = (string)null, ReturnValue = name };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Transfers board ownership from the current owner to another board member.
        /// </summary>
        /// <param name="ownerEmail">The email of the current owner.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="newOwnerEmail">The email of the member who will become the new owner.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        public string TransferBoardOwnership(string ownerEmail, string boardName, string newOwnerEmail)
        {
            try
            {
                _boardFacade.TransferOwnership(ownerEmail, newOwnerEmail, boardName);
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }
    }
}