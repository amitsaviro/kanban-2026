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

        public Board(string name)
        {
            _name = name;
            // Y - task IDs start at 0 and increment with each new task (unique per board, Requirement 5a)
            _nextTaskId = 0;
            // Y - every board always has exactly 3 columns in this fixed order (Requirement 4)
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

        /// <summary>
        /// Adds a new task to the backlog column.
        /// </summary>
        /// <param name="title">Task title, max 50 chars, not empty.</param>
        /// <param name="description">Optional description, max 300 chars.</param>
        /// <param name="dueDate">Task due date.</param>
        /// <returns>The newly created Task.</returns>
        /// <exception cref="ArgumentException">Thrown if title or description violates constraints.</exception>
        /// <exception cref="InvalidOperationException">Thrown if backlog column is at its limit.</exception>
        public Task AddTask(string title, string description, DateTime dueDate)
        {
            // Y - Task constructor validates title and description, so we just create it and let it throw if invalid
            Task task = new Task(_nextTaskId, title, description, dueDate);

            // Y - new tasks always go to backlog only (Requirement 13)
            // _columns[0] is always backlog because of how we built the list in the constructor
            _columns[(int)ColumnType.Backlog].AddTask(task);

            // Y - only increment the ID counter after successfully adding the task
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
            // Y - cast the int to ColumnType enum so we can use named comparisons instead of magic numbers
            ColumnType currentType = (ColumnType)columnOrdinal;

            if (columnOrdinal < 0 || columnOrdinal >= _columns.Count)
                throw new ArgumentException($"Invalid column ordinal: {columnOrdinal}.");

            // Y - cannot move a task that is already in Done (Requirement 14)
            if (currentType == ColumnType.Done)
                throw new InvalidOperationException("Cannot advance a task that is already in the 'done' column.");

            // Y - remove the task from the current column first
            Task task = _columns[columnOrdinal].RemoveTask(taskId);

            try
            {
                // Y - try to add the task to the next column (index + 1)
                _columns[columnOrdinal + 1].AddTask(task);
            }
            catch
            {
                // Y - if the destination column is full, put the task back where it was (rollback)
                // This keeps the board in a consistent state even when the move fails
                _columns[columnOrdinal].AddTask(task);
                throw;
            }
        }

        /// <summary>
        /// Sets the task limit on a specific column.
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
    }
}
