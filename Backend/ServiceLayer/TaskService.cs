using Backend.BusinessLayer;
using Backend.Facades;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backend.ServiceLayer
{
    /// <summary>
    /// Service layer for managing tasks within the Kanban system.
    /// </summary>
    public class TaskService
    {
        private TaskFacade _taskFacade;
        /// <summary>
        /// Initializes a new instance of the TaskService class and its underlying Facade.
        /// </summary>
        public TaskService()
        {
            _taskFacade = new TaskFacade();
        }

        // Y - injection constructor: allows GradingService to pass a TaskFacade that shares a UserFacade
        public TaskService(TaskFacade taskFacade)
        {
            _taskFacade = taskFacade;
        }
        /// <summary>
        /// Adds a new task to the 'backlog' column of a specific board.
        /// Only board members may add tasks.
        /// </summary>
        /// <param name="email">The user email. Must be logged in and a member of the board.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="title">Task title (max 50 chars, not empty).</param>
        /// <param name="description">Task description (optional, max 300 chars).</param>
        /// <param name="dueDate">The due date for the task.</param>
        /// <returns>A JSON string representing the newly created Task.</returns>
        /// <exception cref="ArgumentException">Thrown if input is malformed, title empty/too long, or description too long.</exception>
        /// <exception cref="InvalidOperationException">Thrown if 'backlog' column is at capacity or user is not a board member.</exception>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                // try to add a new task to the backlog coloumn via facade
                _taskFacade.AddTask(email,boardName,title,description,dueDate);
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

        ///////////////////////////// yuval - new name here ///////////////////////
        /// <summary>
        /// Moves a task to the next logical column (Backlog -> In Progress -> Done).
        /// Only the task's assignee may advance it.
        /// </summary>
        /// <param name="email">The user email. Must be logged in and be the task's assignee.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index.</param>
        /// <param name="taskId">The ID of the task to move.</param>
        /// <returns>A JSON string confirming the move or error message.</returns>
        /// <exception cref="ArgumentException">Thrown if task does not exist or move is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not the assignee or task is already done.</exception>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                // try to move a task to the next logical coloumn via facade
                _taskFacade.AdvanceTask(email, boardName, columnOrdinal, taskId);
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

        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the title of an existing task.
        /// Only the task's assignee or the board owner may edit a non-done task.
        /// </summary>
        /// <param name="email">The user email. Must be logged in and be the task's assignee or the board owner.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="title">New title (max 50 chars, not empty).</param>
        /// <returns>A JSON string confirmation.</returns>
        /// <exception cref="ArgumentException">Thrown if task is done or title is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not the assignee or board owner.</exception>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                // try to updates the title of an existing task via facade. 
                _taskFacade.UpdateTaskTitle(email, boardName, columnOrdinal, taskId, title);
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
        /// Updates the description of an existing task.
        /// Only the task's assignee or the board owner may edit a non-done task.
        /// </summary>
        /// <param name="email">The user email. Must be logged in and be the task's assignee or the board owner.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="description">New description (max 300 chars).</param>
        /// <returns>A JSON string confirmation.</returns>
        /// <exception cref="ArgumentException">Thrown if task is done or description is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not the assignee or board owner.</exception>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                // try to updates the description of an existing task via facade. 
                _taskFacade.UpdateTaskDescription(email, boardName, columnOrdinal, taskId, description);
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
        /// Updates the due date of an existing task.
        /// Only the task's assignee or the board owner may edit a non-done task.
        /// </summary>
        /// <param name="email">The user email. Must be logged in and be the task's assignee or the board owner.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="dueDate">New due date.</param>
        /// <returns>A JSON string confirmation.</returns>
        /// <exception cref="ArgumentException">Thrown if task is done.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user is not the assignee or board owner.</exception>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                // try to updates the due date of an existing task via facade. 
                _taskFacade.UpdateTaskDueDate(email,boardName,columnOrdinal, taskId, dueDate);
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
        /// Retrieves all 'in progress' tasks that are assigned to the user, across all boards they are a member of.
        /// </summary>
        /// <param name="email">The user email. Must be logged in.</param>
        /// <returns>A JSON string containing the list of in-progress tasks assigned to this user.</returns>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public string InProgressTasks(string email)
        {
            try
            {
                // try to retrieves all 'in progress' tasks from all boards owned by the user via facade. 
                List<Task> tasks = _taskFacade.InProgressTasks(email); 
                // On success, return an empty response (both fields are null)
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
        /////////////////////////////////////////////////////////////////////////
        /// 
        /// <summary>
        /// Assigns a task to a board member.
        /// An unassigned task may be assigned by any board member to any board member.
        /// An already-assigned task may only be reassigned by the current assignee or the board owner.
        /// </summary>
        /// <param name="email">The email of the user performing the assignment.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The column index (0, 1, or 2).</param>
        /// <param name="taskID">The ID of the task to assign.</param>
        /// <param name="emailAssignee">The email of the user to assign, or null/empty to unassign.</param>
        /// <returns>A JSON string confirmation.</returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                _taskFacade.AssignTask(email, boardName, columnOrdinal, taskID, emailAssignee);
                
                // On success, return an empty response
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
    }
}