using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a Kanban board containing tasks organized in columns.
    /// </summary>
    public class Board
    {
        private string _name;
        private List<Column> _columns;
        private int _nextTaskId;

        /// <summary>
        /// Adds a new task to the 'backlog' column.
        /// </summary>
        /// <param name="title">Task title (max 50 chars, not empty).</param>
        /// <param name="description">Task description (optional, max 300 chars).</param>
        /// <param name="dueDate">Task due date.</param>
        /// <returns>The created Task object.</returns>
        /// <exception cref="ArgumentException">Thrown if inputs violate length or content constraints.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the column exceeds its task limit.</exception>
        public Task AddTask(string title, string description, DateTime dueDate)
        {
            Task task = new Task();
            task.UpdateTitle(title);
            task.UpdateDescription(description);
            task.UpdateDueDate(dueDate);
            _columns[0].AddTask(task);
            return task;
        }

        /// <summary>
        /// Moves a task between columns (Backlog -> In Progress -> Done).
        /// </summary>
        /// <param name="columnOrdinal">The current column index.</param>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <exception cref="InvalidOperationException">Thrown if the move is not allowed or destination column is full.</exception>
        public void MoveTask(int columnOrdinal, int taskId)
        {
            Task task = _columns[columnOrdinal].RemoveTask(taskId);
            if (columnOrdinal < 2)
                _columns[columnOrdinal + 1].AddTask(task);
            else
                _columns[0].AddTask(task);
        }

        /// <summary>
        /// Limits the maximum number of tasks allowed in a column.
        /// </summary>
        /// <param name="columnOrdinal">The column index (0, 1, or 2).</param>
        /// <param name="limit">The max number of tasks.</param>
        /// <exception cref="ArgumentException">Thrown if columnOrdinal is invalid or limit is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the new limit is less than the current task count in the column.</exception>
        public void LimitColumn(int columnOrdinal, int limit)
        {
            if (columnOrdinal > 2 || columnOrdinal < 0)
                throw new ArgumentException();
            _columns[columnOrdinal].SetLimit(limit);
        }

        /// <summary>
        /// Retrieves a specific column object by its index.
        /// </summary>
        /// <param name="columnOrdinal">The column index (0, 1, or 2).</param>
        /// <returns>The Column object.</returns>
        /// <exception cref="ArgumentException">Thrown if columnOrdinal is invalid.</exception>
        public Column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal > 2 || columnOrdinal < 0)
                throw new ArgumentException();
            return _columns[columnOrdinal];
        }

        /// <summary>
        /// Returns all tasks currently in the 'in progress' column across the board.
        /// </summary>
        /// <returns>A list of tasks in progress.</returns>
        public List<Task> GetInProgressTasks()
        {
            return _columns[1].GetTasks();
        }
    }
}