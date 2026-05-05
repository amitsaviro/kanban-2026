using System;
using System.Collections.Generic;
using Backend.BusinessLayer;

namespace Backend.Facades
{
    /// <summary>
    /// Facade for task management operations.
    /// Manages task lifecycle: creation, updates, and movement.
    /// </summary>
    public class TaskFacade
    {
        /// <summary>
        /// Adds a new task to the backlog of the specified board.
        /// </summary>
        /// <param name="email">User email (must be logged in).</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="title">Title (max 50 chars).</param>
        /// <param name="description">Description (optional, max 300 chars).</param>
        /// <param name="dueDate">Task due date.</param>
        /// <remarks>
        /// Precondition: User logged in, Board exists, backlog capacity not reached.
        /// Postcondition: New task added to the backlog.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if inputs are invalid or column is full.</exception>
        public Task AddTask(string email, string boardName, string title, string description, DateTime dueDate) => throw new NotImplementedException();

        /// <summary>
        /// Advances a task to the next column.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index of the task.</param>
        /// <param name="taskId">The ID of the task to advance.</param>
        /// <remarks>
        /// Precondition: Task is not in the 'Done' column, and column limit allows the move.
        /// Postcondition: Task moved to the next column.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if task not found or move is invalid.</exception>
        public void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId) => throw new NotImplementedException();

        /// <summary>
        /// Updates the title of an existing task.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index of the task.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="title">The new title.</param>
        /// <remarks>
        /// Precondition: Task exists and user is authenticated.
        /// Postcondition: Task title is updated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if title is invalid or task not found.</exception>
        public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title) => throw new NotImplementedException();

        /// <summary>
        /// Updates the description of an existing task.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index of the task.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="description">The new description.</param>
        /// <remarks>
        /// Precondition: Task exists and user is authenticated.
        /// Postcondition: Task description is updated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if description is invalid or task not found.</exception>
        public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description) => throw new NotImplementedException();

        /// <summary>
        /// Updates the due date of an existing task.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnOrdinal">The current column index of the task.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="dueDate">The new due date.</param>
        /// <remarks>
        /// Precondition: Task exists and user is authenticated.
        /// Postcondition: Task due date is updated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if date is invalid or task not found.</exception>
        public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves all tasks currently in progress for the user.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>A list of tasks.</returns>
        /// <remarks>
        /// Precondition: User is logged in.
        /// Postcondition: Returns a list of tasks currently in progress.
        /// </remarks>
        /// <exception cref="Exception">Thrown if user not logged in.</exception>
        public List<Task> GetInProgressTasks(string email) => throw new NotImplementedException();
    }
}