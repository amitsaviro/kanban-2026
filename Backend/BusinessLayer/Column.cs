using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a specific column in a Kanban board with task constraints.
    /// </summary>
    public class Column
    {
        private string _name;
        private int _ordinal; // 0 = backlog, 1 = in progress, 2 = done
        private int _limit; 
        private List<Task> _tasks;

        /// <summary>
        /// Adds a task to this column if the limit permits.
        /// </summary>
        /// <param name="task">The task object to add.</param>
        /// <exception cref="InvalidOperationException">Thrown if column capacity limit is reached.</exception>
        /// <exception cref="ArgumentNullException">Thrown if task is null.</exception>
        public void AddTask(Task task) => throw new NotImplementedException();

        /// <summary>
        /// Removes a task from the column by its ID.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <returns>The removed task.</returns>
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task RemoveTask(int taskId) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves a task by its ID.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <returns>The task object.</returns>
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task GetTask(int taskId) => throw new NotImplementedException();

        /// <summary>
        /// Sets a new limit for the number of tasks in this column.
        /// </summary>
        /// <param name="limit">The max number of tasks.</param>
        /// <exception cref="ArgumentException">Thrown if limit is negative or smaller than current tasks count.</exception>
        public void SetLimit(int limit) => throw new NotImplementedException();
    }
}