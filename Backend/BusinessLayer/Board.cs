using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a Kanban board with three fixed columns: Backlog, InProgress, Done.
    /// </summary>
    public class Board
    {
        // Y - private fields; the board controls its columns and task ID counter
        private string _name;
        private List<Column> _columns;
        private int _nextTaskId;

        // A - New fields for Milestone 2: System-wide ID, Owner, and Members list
        private int _id;
        private string _ownerEmail;
        private List<string> _members;

        /// <summary>
        /// Creates a new board.
        /// </summary>
        /// <param name="id">System-wide unique ID for the board.</param>
        /// <param name="name">Name of the board.</param>
        /// <param name="creatorEmail">The email of the user creating the board (becomes the owner).</param>
        public Board(int id, string name, string creatorEmail)
        {
            // A - Initialize new fields for M2.
            _id = id;
            _name = name;
            _ownerEmail = creatorEmail;

            // A - The creator is automatically the first member.
            // A - normalize to lowercase so member lookups are always case-insensitive
            _members = new List<string> { creatorEmail.Trim().ToLower() };

            // task IDs start at 0 and increment with each new task (unique per board, Requirement 5a)
            _nextTaskId = 0;
            
            // every board always has exactly 3 columns in this fixed order (Requirement 4)
            // (ColumnType)0 = Backlog, (ColumnType)1 = InProgress, (ColumnType)2 = Done
            _columns = new List<Column>
            {
                new Column("backlog", ColumnType.Backlog),
                new Column("in progress", ColumnType.InProgress),
                new Column("done", ColumnType.Done)
            };
        }

        // Y - read-only property for the board name
        public string Name => _name;

        // A - Expose ID, Owner, and Members as read-only properties.
        public int Id => _id;
        public string OwnerEmail => _ownerEmail;
        public List<string> Members => _members;

        /// <summary>
        /// A - Adds a new member to the board.
        /// </summary>
        /// <param name="email">Email of the user joining the board.</param>
        public void AddMember(string email)
        {
            if (_members.Contains(email))
                throw new InvalidOperationException($"User '{email}' is already a member of board '{_name}'.");
            
            // A - normalize to lowercase for consistent case-insensitive membership checks
            _members.Add(email.Trim().ToLower());
        }

        /// <summary>
        /// A - Removes a member from the board.
        /// </summary>
        /// <param name="email">Email of the user leaving the board.</param>
        public void RemoveMember(string email)
        {
            // A - Requirement 14: Owner cannot leave the board
            if (email == _ownerEmail)
                throw new InvalidOperationException("The board owner cannot leave the board."); 
            
            if (!_members.Contains(email))
                throw new InvalidOperationException($"User '{email}' is not a member of board '{_name}'.");

            _members.Remove(email);
            
            // A - Requirement 15: "Once a user leaves a board, all of her assigned tasks that are not done become unassigned."
            foreach (Column col in _columns)
            {
                if (col.Type != ColumnType.Done)
                {
                    foreach (Task task in col.Tasks)
                    {
                        if (task.Assignee == email)
                        {
                            task.AssignTask(null); // Unassign
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A - Transfers ownership to another board member.
        /// </summary>
        /// <param name="newOwnerEmail">Email of the new owner.</param>
        public void TransferOwnership(string newOwnerEmail)
        {
            // A - Requirement 13: New owner must be a member
            if (!_members.Contains(newOwnerEmail))
                throw new InvalidOperationException($"Cannot transfer ownership to '{newOwnerEmail}' as they are not a member of the board."); 
                
            _ownerEmail = newOwnerEmail;
        }

        /// <summary>
        /// Adds a new task to the backlog column.
        /// </summary>
        /// <param name="title">Task title, max 50 chars, not empty.</param>
        /// <param name="description">Optional description, max 300 chars.</param>
        /// <param name="dueDate">Task due date.</param>
        public Task AddTask(string title, string description, DateTime dueDate)
        {
            Task task = new Task(_nextTaskId, title, description, dueDate);
            _columns[(int)ColumnType.Backlog].Tasks.Add(task); // Delegating to Column
            _nextTaskId++;
            return task;
        }

        /// <summary>
        /// Moves a task from its current column to the next one (backlog→in progress, in progress→done).
        /// </summary>
        /// <param name="columnOrdinal">Current column index (0 or 1). Cannot move from done (2).</param>
        /// <param name="taskId">ID of the task to move.</param>
        /// <exception cref="ArgumentException">Thrown if columnOrdinal is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if task is already done, or destination is full.</exception>
        public void MoveTask(int columnOrdinal, int taskId)
        {
            // cast the int to ColumnType enum so we can use named comparisons
            ColumnType currentType = (ColumnType)columnOrdinal;

            if (columnOrdinal < 0 || columnOrdinal >= _columns.Count)
                throw new ArgumentException($"Invalid column ordinal: {columnOrdinal}.");

            // cannot move a task that is already in Done (Requirement 14)
            if (currentType == ColumnType.Done)
                throw new InvalidOperationException("Cannot advance a task that is already in the 'done' column.");

            // remove the task from the current column first
            Task task = _columns[columnOrdinal].RemoveTask(taskId);

            try
            {
                // try to add the task to the next column (index + 1)
                _columns[columnOrdinal + 1].AddTask(task);
            }
            catch
            {
                // if the destination column is full, put the task back where it was (rollback)
                _columns[columnOrdinal].AddTask(task);
                throw;
            }
        }

        /// <summary>
        /// Sets the maximum number of tasks allowed in a column.
        /// </summary>
        /// <param name="columnOrdinal">Column index (0-2).</param>
        /// <param name="limit">Max tasks. -1 means no limit.</param>
        /// <exception cref="ArgumentException">Thrown if columnOrdinal is invalid.</exception>
        public void LimitColumn(int columnOrdinal, int limit)
        {
            // Y - GetColumn handles the ordinal validation, so we just delegate
            GetColumn(columnOrdinal).SetLimit(limit);
        }

        /// <summary>
        /// Returns the Column object at the given ordinal.
        /// </summary>
        /// <param name="columnOrdinal">Column index (0-2).</param>
        /// <returns>The Column object.</returns>
        /// <exception cref="ArgumentException">Thrown if columnOrdinal is out of range.</exception>
        public Column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal < 0 || columnOrdinal >= _columns.Count)
                throw new ArgumentException($"Invalid column ordinal: {columnOrdinal}. Must be 0, 1, or 2.");

            return _columns[columnOrdinal];
        }

        /// <summary>
        /// Returns all tasks currently in the 'in progress' column.
        /// </summary>
        /// <returns>A new list containing the in-progress tasks.</returns>
        public List<Task> GetInProgressTasks()
        {
            // Y - new List<Task>(...) creates a copy of the list so callers can't accidentally modify our internal data
            return new List<Task>(_columns[(int)ColumnType.InProgress].Tasks);
        }

        /// <summary>
        /// A - Returns all in-progress tasks assigned to a specific user.
        /// Used by TaskFacade.InProgressTasks to satisfy Requirement 22.
        /// </summary>
        /// <param name="email">The email of the user whose assigned in-progress tasks to return.</param>
        /// <returns>A list of in-progress tasks assigned to <paramref name="email"/>.</returns>
        public List<Task> GetInProgressTasksAssignedTo(string email)
        {
            string cleanEmail = email.Trim().ToLower();
            List<Task> result = new List<Task>();
            // A - only include tasks whose assignee matches the requested email
            foreach (Task t in _columns[(int)ColumnType.InProgress].Tasks)
                if (t.Assignee != null && t.Assignee.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase))
                    result.Add(t);
            return result;
        }
    }
}