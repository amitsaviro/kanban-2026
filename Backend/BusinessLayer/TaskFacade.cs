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
        /// <exception cref="InvalidOperationException">Thrown if task is already done, destination is full, or user is not the assignee.</exception>
        public void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            Column col = board.GetColumn(columnOrdinal);

            if ((ColumnType)columnOrdinal == ColumnType.Done)
                throw new ArgumentException("Cannot advance a task that is already in the 'done' column.");

            Task task = col.GetTask(taskId);

            // A - Requirement 19: only the assignee may advance a task (stricter than edit — owner cannot advance)
            if (!string.IsNullOrEmpty(task.Assignee))
            {
                string cleanEmail = email.Trim().ToLower();
                if (!task.Assignee.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase))
                {
                    log.Warn($"User '{email}' attempted to advance task {taskId} assigned to '{task.Assignee}'.");
                    throw new InvalidOperationException("Only the task assignee may advance this task.");
                }
            }

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
        /// <summary>
        /// Returns all in-progress tasks assigned to the user, across all boards they are a member of.
        /// Satisfies Requirement 22: list 'in progress' tasks that the user is assigned to.
        /// </summary>
        /// <param name="email">User email, must be logged in.</param>
        /// <returns>A flat list of in-progress tasks assigned to this user.</returns>
        /// <exception cref="InvalidOperationException">Thrown if user is not logged in.</exception>
        public List<Task> InProgressTasks(string email)
        {
            User user = _userFacade.GetLoggedInUser(email);
            List<Task> result = new List<Task>();
            // A - Requirement 22: only tasks ASSIGNED TO this user, across all boards they belong to
            foreach (Board board in user.GetBoards())
                result.AddRange(board.GetInProgressTasksAssignedTo(email));
            return result;
        }

        /// <summary>
        /// Private helper: finds a task and ensures it is not in the Done column (editable).
        /// A - Updated to also check if the user is the assignee (Requirement for Milestone 2).
        /// </summary>
        /// <summary>
        /// Private helper: finds a task, validates it is not Done, and enforces edit permissions.
        /// Satisfies Requirement 20: a non-done task can be changed only by its assignee or the board owner.
        /// </summary>
        private Task GetEditableTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            Column col = board.GetColumn(columnOrdinal);

            // Y - cast int to ColumnType enum so we can compare by name, not by magic number
            if ((ColumnType)columnOrdinal == ColumnType.Done)
                throw new ArgumentException("Cannot update a task that is already in the 'done' column.");

            Task task = col.GetTask(taskId);

            string cleanEmail = email.Trim().ToLower();
            bool isOwner = board.OwnerEmail.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase);
            bool isAssignee = !string.IsNullOrEmpty(task.Assignee) &&
                              task.Assignee.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase);

            // A - Requirement 20: only assignee or board owner may edit a non-done task
            // A - if task has no assignee, any member may edit it (unassigned = no restriction yet)
            if (!string.IsNullOrEmpty(task.Assignee) && !isAssignee && !isOwner)
            {
                log.Warn($"User '{email}' attempted to modify task {taskId} assigned to '{task.Assignee}'.");
                throw new InvalidOperationException("Only the task assignee or board owner may modify this task.");
            }

            return task;
        }


        /// <summary>
        /// A - Assigns a task to a specific user on the board.
        /// </summary>
        /// <param name="email">Email of the user making the request. Must be logged in.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The column ID (0 = Backlog, 1 = InProgress, 2 = Done).</param>
        /// <param name="taskID">The task ID to assign.</param>
        /// <param name="emailAssignee">Email of the user to assign the task to. Can be null/empty to unassign.</param>
        /// <summary>
        /// A - Assigns a task to a specific board member, or unassigns it.
        /// Satisfies Requirement 23:
        ///   - Unassigned task: any board member may assign it to any board member.
        ///   - Already-assigned task: only the current assignee or board owner may change the assignment.
        /// </summary>
        /// <param name="email">Email of the user making the request. Must be logged in and be a board member.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">Column index (0=backlog, 1=in progress, 2=done).</param>
        /// <param name="taskID">The task ID to assign.</param>
        /// <param name="emailAssignee">Email of the new assignee (must be a board member), or null/empty to unassign.</param>
        /// <exception cref="ArgumentException">Thrown if assignee is not a board member or task not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if task is done or caller lacks permission to reassign.</exception>
        public void AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);

            // A - done tasks cannot be reassigned (Requirement 20)
            if ((ColumnType)columnOrdinal == ColumnType.Done)
                throw new InvalidOperationException("Cannot assign a task that is in the 'done' column.");

            Task task = board.GetColumn(columnOrdinal).GetTask(taskID);
            string cleanCaller = email.Trim().ToLower();

            // A - Requirement 23: if the task already has an assignee, only that assignee or the board owner may change it
            if (!string.IsNullOrEmpty(task.Assignee))
            {
                bool isOwner = board.OwnerEmail.Equals(cleanCaller, StringComparison.OrdinalIgnoreCase);
                bool isCurrentAssignee = task.Assignee.Equals(cleanCaller, StringComparison.OrdinalIgnoreCase);
                if (!isOwner && !isCurrentAssignee)
                {
                    log.Warn($"User '{email}' attempted to reassign task {taskID} assigned to '{task.Assignee}'.");
                    throw new InvalidOperationException("Only the current assignee or board owner may reassign this task.");
                }
            }

            // A - validate the new assignee is a board member 
            if (!string.IsNullOrEmpty(emailAssignee) && !board.Members.Contains(emailAssignee.Trim().ToLower()))
            {
                log.Warn($"Failed to assign task: User '{emailAssignee}' is not a member of board '{boardName}'.");
                throw new ArgumentException($"User '{emailAssignee}' is not a member of the board.");
            }

            task.AssignTask(string.IsNullOrEmpty(emailAssignee) ? null : emailAssignee);
            log.Info($"Task {taskID} on board '{boardName}' assigned to '{emailAssignee ?? "nobody"}' by '{email}'.");
        }
    }
}