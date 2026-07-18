using System;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for a single task card inside a board column. Wraps a <see cref="TaskModel"/>
    /// with editable fields and the commands (advance/assign/save) that act on it.
    /// </summary>
    // A- TaskModel itself is a plain data holder (no INotifyPropertyChanged); this class is the
    // A- "live", bindable wrapper the View actually binds to, keeping the two concerns separate.
    public class TaskItemViewModel : ViewModelBase
    {
        private readonly TaskService _taskService;
        private readonly string _currentUserEmail;
        private readonly string _boardName;
        private readonly Action _onChanged;

        private string _title;
        private string _description;
        private DateTime _dueDate;
        private string _assigneeInput;
        private string _errorMessage = string.Empty;

        /// <summary>
        /// Creates a TaskItemViewModel wrapping the given task.
        /// </summary>
        /// <param name="task">The task data this card represents.</param>
        /// <param name="taskService">The shared, DB-connected TaskService built by the composition root.</param>
        /// <param name="currentUserEmail">The email of the currently logged-in user.</param>
        /// <param name="boardName">The name of the board this task belongs to.</param>
        /// <param name="onChanged">Callback invoked after any successful change, so the parent BoardViewModel can reload all columns.</param>
        public TaskItemViewModel(TaskModel task, TaskService taskService, string currentUserEmail, string boardName, Action onChanged)
        {
            Task = task;
            _taskService = taskService;
            _currentUserEmail = currentUserEmail;
            _boardName = boardName;
            _onChanged = onChanged;

            _title = task.Title;
            _description = task.Description;
            _dueDate = task.DueDate;
            _assigneeInput = task.Assignee ?? string.Empty;

            SaveCommand = new RelayCommand(_ => Save());
            AdvanceCommand = new RelayCommand(_ => Advance(), _ => Task.ColumnOrdinal < 2);
            AssignCommand = new RelayCommand(_ => Assign());
        }

        /// <summary>The underlying task data (Id, ColumnOrdinal, etc.) this card wraps.</summary>
        public TaskModel Task { get; }

        /// <summary>Editable title bound to the card's title TextBox.</summary>
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        /// <summary>Editable description bound to the card's description TextBox.</summary>
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        /// <summary>Editable due date bound to the card's due-date picker.</summary>
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetField(ref _dueDate, value);
        }

        /// <summary>The email typed in to (re)assign this task. Empty clears the assignment.</summary>
        public string AssigneeInput
        {
            get => _assigneeInput;
            set => SetField(ref _assigneeInput, value);
        }

        /// <summary>The last error message for this card, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the card's "Save" button (title/description/due date).</summary>
        public RelayCommand SaveCommand { get; }

        /// <summary>Command bound to the card's "Advance" button. Disabled once the task is in the "done" column.</summary>
        public RelayCommand AdvanceCommand { get; }

        /// <summary>Command bound to the card's "Assign" button.</summary>
        public RelayCommand AssignCommand { get; }

        /// <summary>
        /// Persists the edited title, description, and due date.
        /// </summary>
        // A- Requirement 21: all task data can be changed except creation time, so this always
        // A- pushes all three editable fields; the Backend itself enforces who is allowed to edit
        // A- (assignee or board owner, non-done tasks only - Requirement 20) and returns an error otherwise.
        private void Save()
        {
            if (!TryUnwrap(_taskService.UpdateTaskTitle(_currentUserEmail, _boardName, Task.ColumnOrdinal, Task.Id, Title)))
                return;
            if (!TryUnwrap(_taskService.UpdateTaskDescription(_currentUserEmail, _boardName, Task.ColumnOrdinal, Task.Id, Description)))
                return;
            if (!TryUnwrap(_taskService.UpdateTaskDueDate(_currentUserEmail, _boardName, Task.ColumnOrdinal, Task.Id, DueDate)))
                return;

            ErrorMessage = string.Empty;
            _onChanged?.Invoke();
        }

        /// <summary>
        /// Advances the task to the next column (Requirement 19: only the assignee may do this).
        /// </summary>
        private void Advance()
        {
            if (!TryUnwrap(_taskService.AdvanceTask(_currentUserEmail, _boardName, Task.ColumnOrdinal, Task.Id)))
                return;

            ErrorMessage = string.Empty;
            _onChanged?.Invoke();
        }

        /// <summary>
        /// Assigns (or unassigns, if <see cref="AssigneeInput"/> is empty) the task.
        /// </summary>
        private void Assign()
        {
            if (!TryUnwrap(_taskService.AssignTask(_currentUserEmail, _boardName, Task.ColumnOrdinal, Task.Id, AssigneeInput)))
                return;

            ErrorMessage = string.Empty;
            _onChanged?.Invoke();
        }

        // A- shared helper: every TaskService call above returns the same {ErrorMessage, ReturnValue}
        // A- envelope: parse it, surface ErrorMessage on this card if present, and report success/failure
        // A- so the caller knows whether to go on to the next step (e.g. Save's 3 sequential updates).
        private bool TryUnwrap(string json)
        {
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return false;
            }
            return true;
        }
    }
}
