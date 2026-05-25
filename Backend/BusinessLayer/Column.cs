using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents one column in a Kanban board (backlog, in progress, or done).
    /// </summary>
    public class Column
    {
        // Y - private fields; the column controls its own data
        private string _name;
        private ColumnType _type;
        private int _limit;
        private List<Task> _tasks;

        // Y - -1 means "no limit" throughout the system (Requirement 12)
        private const int NoLimit = -1;

        /// <summary>
        /// Creates a new empty column.
        /// </summary>
        /// <param name="name">Display name e.g. "backlog".</param>
        /// <param name="type">The column type enum value (Backlog, InProgress, Done).</param>
        public Column(string name, ColumnType type)
        {
            _name = name;
            _type = type;
            // Y - columns start with no limit by default (Requirement 12)
            _limit = NoLimit;
            // Y - List<Task> is a resizable list of Task objects, like ArrayList in other languages
            _tasks = new List<Task>();
        }

        // Y - read-only properties so outside code can read but not overwrite these directly
        public string Name => _name;
        public ColumnType Type => _type;
        public int Limit => _limit;
        // Y - returns the internal list directly so callers can iterate tasks; we trust our own layer not to misuse it
        public List<Task> Tasks => _tasks;

        /// <summary>
        /// Adds a task to this column if the limit permits.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if task is null.</exception>
<<<<<<< HEAD
        public void AddTask(Task task)
        {
            if (task == null) throw new ArgumentNullException("task isnt exist");
            else
            {
                if (_tasks.Count == this._limit) throw new InvalidOperationException("the list has got to its linit");
                else
                    _tasks.Add(task);
            }
=======
        /// <exception cref="InvalidOperationException">Thrown if column is at its task limit.</exception>
        public void AddTask(Task task)
        {
            // Y - null check: ArgumentNullException is the standard C# exception for null arguments
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            // Y - only enforce the limit if one is set (_limit != NoLimit)
            if (_limit != NoLimit && _tasks.Count >= _limit)
                throw new InvalidOperationException($"Column '{_name}' has reached its limit of {_limit} tasks.");

            _tasks.Add(task);
>>>>>>> main
        }

        /// <summary>
        /// Removes a task from this column by its ID and returns it.
        /// </summary>
        /// <param name="taskId">The ID of the task to remove.</param>
        /// <returns>The removed task.</returns>
<<<<<<< HEAD
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task RemoveTask(int taskId)
        {
            Task toreturn = _tasks[taskId];
            if (toreturn == null) throw new ArgumentException("there is no task in the given id");
            else
                _tasks.Remove(toreturn);
            return toreturn;
=======
        /// <exception cref="ArgumentException">Thrown if no task with that ID exists.</exception>
        public Task RemoveTask(int taskId)
        {
            // Y - lambda "t => t.Id == taskId" means: for each task t, check if its Id equals taskId
            // Find returns the first match, or null if nothing matches
            Task task = _tasks.Find(t => t.Id == taskId);

            if (task == null)
                throw new ArgumentException($"Task with ID {taskId} not found in column '{_name}'.");

            _tasks.Remove(task);
            return task;
>>>>>>> main
        }

        /// <summary>
        /// Retrieves a task by its ID without removing it.
        /// </summary>
<<<<<<< HEAD
        /// <param name="taskId">The unique ID of the task.</param>
        /// <returns>The task object.</returns>
        /// <exception cref="ArgumentException">Thrown if task with given ID is not found.</exception>
        public Task GetTask(int taskId)
        {
            if (_tasks[taskId] == null)
                throw new ArgumentException("there is no task in thr given taskid");
            return _tasks[taskId];
=======
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>The task.</returns>
        /// <exception cref="ArgumentException">Thrown if no task with that ID exists.</exception>
        public Task GetTask(int taskId)
        {
            // Y - same lambda search as RemoveTask but we don't remove it
            Task task = _tasks.Find(t => t.Id == taskId);

            if (task == null)
                throw new ArgumentException($"Task with ID {taskId} not found in column '{_name}'.");

            return task;
>>>>>>> main
        }

        /// <summary>
        /// Sets the maximum number of tasks allowed in this column.
        /// </summary>
<<<<<<< HEAD
        /// <param name="limit">The max number of tasks.</param>
        /// <exception cref="ArgumentException">Thrown if limit is negative or smaller than current tasks count.</exception>
        public void SetLimit(int limit)
        { 
            if(limit< 0 || limit < _tasks.Count)
                throw new ArgumentException("limit is smaller then the currnt count or smaller then 0");
           this._limit = limit;
        }

        public int limit
        {
            get { return _limit; }
        }
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public List<Task> GetTasks() { return _tasks; }
=======
        /// <param name="limit">The new limit. Use -1 for no limit.</param>
        /// <exception cref="ArgumentException">Thrown if limit is invalid or less than current task count.</exception>
        public void SetLimit(int limit)
        {
            // Y - limit must be -1 (no limit) or a positive number; 0 would make the column unusable
            if (limit < NoLimit)
                throw new ArgumentException("Limit must be -1 (no limit) or a positive integer.");

            // Y - can't set a limit lower than the number of tasks already in the column
            if (limit != NoLimit && limit < _tasks.Count)
                throw new ArgumentException($"Cannot set limit to {limit}: column already has {_tasks.Count} tasks.");

            _limit = limit;
        }
>>>>>>> main
    }
}
