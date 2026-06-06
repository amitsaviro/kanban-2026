using Backend.BusinessLayer;
using Backend.Facades;
using System;
using System.Collections.Generic;
using System.Text.Json;// The official C# library for JSON serialization
using System.Threading.Tasks;
using Task = Backend.BusinessLayer.Task;
namespace Backend.ServiceLayer
{
    /// <summary>
    /// Service layer for managing boards within the Kanban system.
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
                // try to create Board via facade
                _boardFacade.CreateBoard(email, boardName);
                // On success, return an empty response (both fields are null)
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch(Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
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
                // try to delete Board via facade
                _boardFacade.DeleteBoard(email, boardName);
                // On success, return an empty response (both fields are null)
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
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
                // try to limit Column in Board via facade
                _boardFacade.LimitColumn(email, boardName, columnOrdinal, limit);
                // On success, return an empty response (both fields are null)
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
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
                // try to get the limit of the spacific column.
                int limit = _boardFacade.GetColumnLimit(email, boardName, columnOrdinal);
                // On success, return the column's limit
                var response = new { ErrorMessage = (string)null, ReturnValue = limit};
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
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
                // try to get the name of the spacific column
                string name = _boardFacade.GetColumnName(email, boardName, columnOrdinal);
                // On success, return the column's name
                var response = new { ErrorMessage = (string)null, ReturnValue = name };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
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
                // try to get the lisit of tasks of the column
                List<Task> tasks = _boardFacade.GetColumn(email, boardName, columnOrdinal);
                // On success, return the column's list of tasks
                var response = new { ErrorMessage = (string)null, ReturnValue = tasks };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Returns a list of IDs of all boards the user is a member of (owned or joined).
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in.</param>
        /// <returns>A JSON string with a list of integer board IDs on success, or an error message.</returns>
        /// <exception cref="ArgumentException">Thrown if user does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public string GetUserBoards(string email)
        {
            try
            {
                // Y - placeholder until BoardFacade implements GetUserBoards
                throw new NotImplementedException("GetUserBoards is not yet implemented.");
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Adds the user as a member of an existing board, identified by its global ID.
        /// Any registered user may join any board without owner permission.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in.</param>
        /// <param name="boardID">The unique integer ID of the board to join.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        /// <exception cref="ArgumentException">Thrown if board ID does not exist or user is already a member.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public string JoinBoard(string email, int boardID)
        {
            try
            {
                // Y - placeholder until BoardFacade implements JoinBoard
                throw new NotImplementedException("JoinBoard is not yet implemented.");
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Removes the user from a board they are a member of.
        /// The board owner cannot leave; ownership must be transferred first.
        /// Any non-done tasks assigned to the leaving user become unassigned.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in and be a board member.</param>
        /// <param name="boardID">The unique integer ID of the board to leave.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        /// <exception cref="ArgumentException">Thrown if board ID does not exist or user is not a member.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in or is the board owner.</exception>
        public string LeaveBoard(string email, int boardID)
        {
            try
            {
                // Y - placeholder until BoardFacade implements LeaveBoard
                throw new NotImplementedException("LeaveBoard is not yet implemented.");
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
        /// <exception cref="ArgumentException">Thrown if no board with the given ID exists.</exception>
        public string GetBoardName(int boardId)
        {
            try
            {
                // Y - placeholder until BoardFacade implements GetBoardName
                throw new NotImplementedException("GetBoardName is not yet implemented.");
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Transfers board ownership from the current owner to another board member.
        /// After transfer the new owner holds all ownership rights and constraints.
        /// </summary>
        /// <param name="ownerEmail">The email of the current owner. Must be logged in.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="newOwnerEmail">The email of the member who will become the new owner. Must be a board member.</param>
        /// <returns>An empty JSON response on success, or an error message.</returns>
        /// <exception cref="ArgumentException">Thrown if board does not exist or new owner is not a member.</exception>
        /// <exception cref="InvalidOperationException">Thrown if caller is not the owner or is not logged in.</exception>
        public string TransferBoardOwnership(string ownerEmail, string boardName, string newOwnerEmail)
        {
            try
            {
                // Y - placeholder until BoardFacade implements TransferBoardOwnership
                throw new NotImplementedException("TransferBoardOwnership is not yet implemented.");
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }
    }
}