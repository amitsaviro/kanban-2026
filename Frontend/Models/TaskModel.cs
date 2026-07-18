using System;

namespace Frontend.Models
{
    /// <summary>
    /// A Frontend-side representation of a Kanban task. Its property names and types match
    /// the JSON produced by serializing a <c>Backend.BusinessLayer.Task</c>, so it can be
    /// deserialized directly out of <see cref="ServiceResponse{T}.ReturnValue"/>.
    /// </summary>
    // A- the Id property below is required so the ViewModel can call AdvanceTask/AssignTask/UpdateTask*
    // A- (they all take a taskId int), but per Requirement 29.c ("do not present internal ID fields")
    // A- no View/XAML in this project may bind a visible control to TaskModel.Id.
    public class TaskModel
    {
        /// <summary>The task's unique ID within its board. Used only internally to call service methods - never shown in the UI.</summary>
        public int Id { get; set; }

        /// <summary>When the task was created. Set once, never changes.</summary>
        public DateTime CreationTime { get; set; }

        /// <summary>The task's title (max 50 characters).</summary>
        public string Title { get; set; }

        /// <summary>The task's description (max 300 characters, optional).</summary>
        public string Description { get; set; }

        /// <summary>The task's due date.</summary>
        public DateTime DueDate { get; set; }

        /// <summary>The email of the user assigned to this task, or null if unassigned.</summary>
        public string Assignee { get; set; }

        /// <summary>
        /// The ordinal (0 = backlog, 1 = in progress, 2 = done) of the column this task
        /// currently sits in.
        /// </summary>
        // A- this field does NOT come from the Task JSON itself - a Task has no notion of "which column
        // A- it's in" on its own. The BoardViewModel sets it after fetching each column separately via
        // A- BoardService.GetColumn(email, boardName, columnOrdinal), so later calls (e.g. AdvanceTask)
        // A- know which column the task is currently coming from.
        public int ColumnOrdinal { get; set; }
    }
}
