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
        public void AddTask(Task task)
        {
            if (task == null) throw new ArgumentNullException("task isnt exist");
            else
            {
                if (_tasks.Count == this._limit) throw new InvalidOperationException("the list has got to its linit");
                else
                    _tasks.Add(task);
            }
        }

        /// <summary>
        /// Removes a task from the column by its ID.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <returns>The removed task.</returns>
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task RemoveTask(int taskId)
        {
            Task toreturn = _tasks[taskId];
            if (toreturn == null) throw new ArgumentException("there is no task in the given id");
            else
                _tasks.Remove(toreturn);
            return toreturn;
        }

        /// <summary>
        /// Retrieves a task by its ID.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <returns>The task object.</returns>
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task GetTask(int taskId)
        {
            if (_tasks[taskId] == null)
                throw new ArgumentException("there is no task in thr given taskid");
            return _tasks[taskId];
        }

        /// <summary>
        /// Sets a new limit for the number of tasks in this column.
        /// </summary>
        /// <param name="limit">The max number of tasks.</param>
        /// <exception cref="ArgumentException">Thrown if limit is negative or smaller than current tasks count.</exception>
        public void SetLimit(int limit)
        { 
            if(limit< 0 || limit < _tasks.Count)
                throw new ArgumentException("limit is smaller then the currnt count or smaller then 0");
           this._limit = limit;
        }

        public List<Task> GetTasks() { return _tasks; }
    }
}