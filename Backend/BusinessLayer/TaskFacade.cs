using System;
using System.Collections.Generic;
using Backend.BusinessLayer;
using log4net;

namespace Backend.Facades
{
    /// <summary>
    /// Facade for all task-related operations.
    /// Validates login status and enforces task editing rules before delegating to Board/Column/Task objects.
    /// </summary>
    public class TaskFacade
    {
        // Y - same shared-state pattern as BoardFacade: holds a reference to UserFacade for user lookups
        private UserFacade _userFacade;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Default constructor for standalone use.</summary>
        public TaskFacade()
        {
            _userFacade = new UserFacade();
        }

        /// <summary>Injection constructor — receives a shared UserFacade from GradingService.</summary>
        /// <param name="userFacade">The shared UserFacade instance.</param>
        public TaskFacade(UserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        /// <summary>
        /// Adds a new task to the backlog of a board.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="title">Task title, max 50 chars, not empty.</param>
        /// <param name="description">Optional description, max 300 chars.</param>
        /// <param name="dueDate">Task due date.</param>
        /// <returns>The created Task object.</returns>
        /// <exception cref="ArgumentException">Thrown if title/description violate constraints.</exception>
        /// <exception cref="InvalidOperationException">Thrown if backlog is full or user not logged in.</exception>
        public Task AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            Task task = board.AddTask(title, description, dueDate);
            log.Info($"Task '{title}' added to board '{boardName}' for user '{email}'.");
            return task;
        }

        /// <summary>
        /// Advances a task to the next column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Current column index (0 or 1).</param>
        /// <param name="taskId">ID of the task to advance.</param>
        /// <exception cref="ArgumentException">Thrown if column or task not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if task is already done or destination is full.</exception>
        public void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            board.MoveTask(columnOrdinal, taskId);
            log.Info($"Task {taskId} advanced from column {columnOrdinal} in board '{boardName}'.");
        }

        /// <summary>
        /// Updates the title of a task. Task must not be in the Done column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Current column index.</param>
        /// <param name="taskId">ID of the task.</param>
        /// <param name="title">New title, max 50 chars, not empty.</param>
        /// <exception cref="ArgumentException">Thrown if task is done or title is invalid.</exception>
        public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            // Y - GetEditableTask is a private helper that checks login, finds the task, and blocks edits to done tasks
            Task task = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateTitle(title);
        }

        /// <summary>
        /// Updates the description of a task. Task must not be in the Done column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Current column index.</param>
        /// <param name="taskId">ID of the task.</param>
        /// <param name="description">New description, max 300 chars.</param>
        /// <exception cref="ArgumentException">Thrown if task is done or description is too long.</exception>
        public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            Task task = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateDescription(description);
        }

        /// <summary>
        /// Updates the due date of a task. Task must not be in the Done column.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="columnOrdinal">Current column index.</param>
        /// <param name="taskId">ID of the task.</param>
        /// <param name="dueDate">New due date.</param>
        /// <exception cref="ArgumentException">Thrown if task is done.</exception>
        public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            Task task = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateDueDate(dueDate);
        }

        /// <summary>
        /// Returns all in-progress tasks across all of the user's boards.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <returns>A flat list of all in-progress tasks.</returns>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public List<Task> InProgressTasks(string email)
        {
            User user = _userFacade.GetLoggedInUser(email);

            // Y - List<Task> starts empty, then we loop over every board and add its in-progress tasks
            List<Task> result = new List<Task>();

            // Y - foreach loop in C# — iterates over every Board object returned by GetBoards()
            foreach (Board board in user.GetBoards())
                result.AddRange(board.GetInProgressTasks());
            // Y - AddRange adds all items from one list into another, like extend() in Python

            return result;
        }

        /// <summary>
        /// Private helper: finds a task and ensures it is not in the Done column (editable).
        /// </summary>
        private Task GetEditableTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            Column col = board.GetColumn(columnOrdinal);

            // Y - cast int to ColumnType enum so we can compare by name, not by magic number
            if ((ColumnType)columnOrdinal == ColumnType.Done)
                throw new ArgumentException("Cannot update a task that is already in the 'done' column.");

            return col.GetTask(taskId);
        }
    }
}
