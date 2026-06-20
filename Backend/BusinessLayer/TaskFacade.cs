using System;
using System.Collections.Generic;
using Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
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

        // Y - null when created without persistence (tests); injected by GradingService for production use
        private TaskController _taskCtrl;
        private BoardController _boardCtrl;

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

        // Y - full injection constructor used by GradingService to wire in DAL controllers
        public TaskFacade(UserFacade userFacade, TaskController taskCtrl, BoardController boardCtrl)
        {
            _userFacade = userFacade;
            _taskCtrl = taskCtrl;
            _boardCtrl = boardCtrl;
        }

        /// <summary>
        /// Adds a new task to the backlog of a board.
        /// </summary>
        public Task AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            User user = _userFacade.GetLoggedInUser(email);
            Board board = user.GetBoard(boardName);
            Task task = board.AddTask(title, description, dueDate);

            if (_taskCtrl != null)
            {
                // Y - persist the new task row; AddTask always places tasks in column 0 (Backlog)
                _taskCtrl.Insert(new TaskDTO(task.Id, board.Id, 0, task.Title, task.Description,
                    task.DueDate.ToString("o"), task.CreationTime.ToString("o"), task.Assignee));
                // Y - persist the incremented NextTaskId so it survives a restart
                _boardCtrl?.UpdateNextTaskId(board.Id, board.NextTaskId);
            }

            log.Info($"Task '{title}' added to board '{boardName}' for user '{email}'.");
            return task;
        }

        /// <summary>
        /// Advances a task to the next column.
        /// </summary>
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
            _taskCtrl?.UpdateColumnOrdinal(board.Id, taskId, columnOrdinal + 1);
            log.Info($"Task {taskId} advanced from column {columnOrdinal} in board '{boardName}'.");
        }

        /// <summary>
        /// Updates the title of a task. Task must not be in the Done column.
        /// </summary>
        public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            // Y - GetEditableTask returns the task and its board so we have the board ID for DB persistence
            var (task, board) = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateTitle(title);
            _taskCtrl?.UpdateTitle(board.Id, taskId, title);
        }

        /// <summary>
        /// Updates the description of a task. Task must not be in the Done column.
        /// </summary>
        public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            var (task, board) = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateDescription(description);
            _taskCtrl?.UpdateDescription(board.Id, taskId, description);
        }

        /// <summary>
        /// Updates the due date of a task. Task must not be in the Done column.
        /// </summary>
        public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            var (task, board) = GetEditableTask(email, boardName, columnOrdinal, taskId);
            task.UpdateDueDate(dueDate);
            _taskCtrl?.UpdateDueDate(board.Id, taskId, dueDate.ToString("o"));
        }

        /// <summary>
        /// Returns all in-progress tasks assigned to the user, across all boards they are a member of.
        /// Satisfies Requirement 22: list 'in progress' tasks that the user is assigned to.
        /// </summary>
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
        /// Private helper: finds a task, validates it is not Done, and enforces edit permissions.
        /// Returns the task AND its board so callers can use the board ID for DB persistence.
        /// Satisfies Requirement 20: a non-done task can be changed only by its assignee or the board owner.
        /// </summary>
        private (Task task, Board board) GetEditableTask(string email, string boardName, int columnOrdinal, int taskId)
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

            return (task, board);
        }

        /// <summary>
        /// A - Assigns a task to a specific board member, or unassigns it.
        /// Satisfies Requirement 23.
        /// </summary>
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

            string newAssignee = string.IsNullOrEmpty(emailAssignee) ? null : emailAssignee;
            task.AssignTask(newAssignee);
            _taskCtrl?.UpdateAssignee(board.Id, taskID, newAssignee);
            log.Info($"Task {taskID} on board '{boardName}' assigned to '{emailAssignee ?? "nobody"}' by '{email}'.");
        }
    }
}
