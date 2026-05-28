using System;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a single task on a Kanban board.
    /// </summary>
    public class Task
    {
        // Y - private fields store the actual data; outside code must use the public properties below to read them
        private int _id;
        private DateTime _creationTime;
        private DateTime _dueDate;
        private string _title;
        private string _description;

        // Y - constants avoid magic numbers; the PDF specifically warns against them
        private const int MaxTitleLength = 50;
        private const int MaxDescriptionLength = 300;

        /// <summary>
        /// Creates a new task. Called by Board.AddTask.
        /// </summary>
        /// <param name="id">Unique ID within the board, assigned by the board.</param>
        /// <param name="title">Task title, max 50 chars, cannot be empty.</param>
        /// <param name="description">Optional description, max 300 chars.</param>
        /// <param name="dueDate">When the task is due.</param>
        public Task(int id, string title, string description, DateTime dueDate)
        {
            // Y - validate title here too so Task is always created in a valid state
            if (string.IsNullOrWhiteSpace(title) || title.Length > MaxTitleLength)
                throw new ArgumentException("Title must be between 1 and 50 characters.");

            // Y - description is optional (can be null or empty), but cannot exceed 300 chars
            if (description != null && description.Length > MaxDescriptionLength)
                throw new ArgumentException("Description cannot exceed 300 characters.");

            _id = id;
            // Y - DateTime.Now captures the exact moment the task was created; this field never changes after this
            _creationTime = DateTime.Now;
            _title = title;
            // Y - if description is null we store empty string so serialization gives "" instead of null
            _description = description ?? string.Empty;
            _dueDate = dueDate;
        }

        // Y - read-only properties: outside code can read these values but cannot set them directly
        // The "=>" arrow syntax is shorthand for { get { return _id; } }
        public int Id => _id;
        public DateTime CreationTime => _creationTime;
        public string Title => _title;
        public string Description => _description;
        public DateTime DueDate => _dueDate;

        /// <summary>
        /// Updates the task title.
        /// </summary>
        /// <param name="title">New title, max 50 chars, cannot be empty.</param>
        /// <exception cref="ArgumentException">Thrown if title is empty or exceeds 50 characters.</exception>
        public void UpdateTitle(string title)
        {
            // Y - same validation as in the constructor — a task can never have an invalid title
            if (string.IsNullOrWhiteSpace(title) || title.Length > MaxTitleLength)
                throw new ArgumentException("Title must be between 1 and 50 characters.");
            _title = title;
        }

        /// <summary>
        /// Updates the task description.
        /// </summary>
        /// <param name="description">New description, max 300 chars, can be null/empty.</param>
        /// <exception cref="ArgumentException">Thrown if description exceeds 300 characters.</exception>
        public void UpdateDescription(string description)
        {
            // Y - null is not allowed on update; use empty string to clear the description
            if (description == null)
                throw new ArgumentException("Description cannot be null.");
            if (description.Length > MaxDescriptionLength)
                throw new ArgumentException("Description cannot exceed 300 characters.");
            _description = description;
        }

        /// <summary>
        /// Updates the task due date.
        /// </summary>
        /// <param name="dueDate">The new due date.</param>
        public void UpdateDueDate(DateTime dueDate)
        {
            // Y - no validation needed here per requirements; any date is acceptable
            _dueDate = dueDate;
        }
    }
}
