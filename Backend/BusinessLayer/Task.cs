using System;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a task within a Kanban board.
    /// </summary>
    public class Task
    {
        private int _id;
        private DateTime _creationTime;
        private DateTime _dueDate;
        private string _title;
        private string _description;

        /// <summary>
        /// Updates the task title.
        /// </summary>
        /// <param name="title">New title (max 50 chars, not empty).</param>
        /// <exception cref="ArgumentException">Thrown if title is empty or exceeds 50 characters.</exception>
        public void UpdateTitle(string title) => throw new NotImplementedException();

        /// <summary>
        /// Updates the task description.
        /// </summary>
        /// <param name="description">New description (max 300 chars, optional).</param>
        /// <exception cref="ArgumentException">Thrown if description exceeds 300 characters.</exception>
        public void UpdateDescription(string description) => throw new NotImplementedException();

        /// <summary>
        /// Updates the task due date.
        /// </summary>
        /// <param name="dueDate">New due date.</param>
        public void UpdateDueDate(DateTime dueDate) => throw new NotImplementedException();
    }
}