using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for a single board's screen: its three columns (backlog / in progress / done),
    /// adding tasks, and board-level actions (leave, transfer ownership, back to boards list).
    /// </summary>
    public class BoardViewModel : ViewModelBase
    {
        private readonly BoardService _boardService;
        private readonly TaskService _taskService;
        private readonly string _currentUserEmail;

        private string _newTaskTitle = string.Empty;
        private string _newTaskDescription = string.Empty;
        private DateTime _newTaskDueDate = DateTime.Today.AddDays(7);
        private string _transferToEmail = string.Empty;
        private string _errorMessage = string.Empty;

        /// <summary>Raised when the user chooses to go back to the boards list.</summary>
        public event Action BackRequested;

        /// <summary>Raised after the current user successfully leaves this board.</summary>
        // A- leaving removes the user's access to the board, so BoardViewModel can no longer show
        // A- it - MainWindowViewModel treats this the same as BackRequested (return to boards list).
        public event Action LeftBoard;

        /// <summary>
        /// Creates the BoardViewModel and loads the board's three columns.
        /// </summary>
        /// <param name="board">The board to display (name + owner only - Requirement 29.c forbids showing its ID).</param>
        /// <param name="boardService">The shared, DB-connected BoardService built by the composition root.</param>
        /// <param name="taskService">The shared, DB-connected TaskService built by the composition root.</param>
        /// <param name="currentUserEmail">The email of the currently logged-in user.</param>
        public BoardViewModel(BoardModel board, BoardService boardService, TaskService taskService, string currentUserEmail)
        {
            Board = board;
            _boardService = boardService;
            _taskService = taskService;
            _currentUserEmail = currentUserEmail;

            Columns = new ObservableCollection<ColumnViewModel>();

            AddTaskCommand = new RelayCommand(_ => AddTask(), _ => !string.IsNullOrWhiteSpace(NewTaskTitle));
            LeaveBoardCommand = new RelayCommand(_ => Leave());
            TransferOwnershipCommand = new RelayCommand(_ => TransferOwnership(), _ => !string.IsNullOrWhiteSpace(TransferToEmail));
            BackCommand = new RelayCommand(_ => BackRequested?.Invoke());

            LoadColumns();
        }

        /// <summary>The board being displayed.</summary>
        public BoardModel Board { get; }

        /// <summary>True if the current user owns this board (controls which board-level actions are shown).</summary>
        public bool IsOwner => string.Equals(Board.OwnerEmail, _currentUserEmail, StringComparison.OrdinalIgnoreCase);

        /// <summary>True if the current user may leave this board - the owner never can (Requirement 14).</summary>
        // A- previously "Leave Board" was shown to everyone including the owner, who would just get
        // A- a Backend error on click; hiding it for the owner is consistent with how Transfer
        // A- Ownership is already only shown when IsOwner is true.
        public bool CanLeave => !IsOwner;

        /// <summary>The board's three columns, in order: backlog, in progress, done.</summary>
        // A- BUG FIX: this was a plain List<ColumnViewModel>. LoadColumns() mutated it in place and
        // A- called OnPropertyChanged(nameof(Columns)) to signal the change, but since the getter kept
        // A- returning the SAME object reference, WPF's binding engine treated it as "unchanged" and
        // A- never re-queried ItemsControl's ItemsSource - so newly added/moved tasks never appeared
        // A- on screen even though the Backend data was correct. ObservableCollection fixes this: it
        // A- raises CollectionChanged on Clear()/Add(), which ItemsControl listens to directly.
        public ObservableCollection<ColumnViewModel> Columns { get; }

        /// <summary>Title typed into the "add task" form. New tasks always go to the backlog (Requirement 18).</summary>
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set => SetField(ref _newTaskTitle, value);
        }

        /// <summary>Description typed into the "add task" form.</summary>
        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set => SetField(ref _newTaskDescription, value);
        }

        /// <summary>Due date chosen in the "add task" form.</summary>
        public DateTime NewTaskDueDate
        {
            get => _newTaskDueDate;
            set => SetField(ref _newTaskDueDate, value);
        }

        /// <summary>The member email typed in to transfer ownership to (owner-only action).</summary>
        public string TransferToEmail
        {
            get => _transferToEmail;
            set => SetField(ref _transferToEmail, value);
        }

        /// <summary>The last error message to display, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the "Add Task" button.</summary>
        public RelayCommand AddTaskCommand { get; }

        /// <summary>Command bound to the "Leave Board" button.</summary>
        public RelayCommand LeaveBoardCommand { get; }

        /// <summary>Command bound to the "Transfer Ownership" button (owner only - Requirement 13).</summary>
        public RelayCommand TransferOwnershipCommand { get; }

        /// <summary>Command bound to the "Back" button, returning to the boards list.</summary>
        public RelayCommand BackCommand { get; }

        /// <summary>
        /// (Re)loads all three columns and their tasks from the Backend.
        /// </summary>
        // A- called once at startup and again after every mutation (add/advance/assign/edit task,
        // A- change a limit) instead of trying to patch the in-memory collections incrementally -
        // A- simpler to reason about and keeps the UI always consistent with what the Backend has.
        private void LoadColumns()
        {
            Columns.Clear();

            for (int ordinal = 0; ordinal <= 2; ordinal++)
            {
                string name = Unwrap<string>(_boardService.GetColumnName(_currentUserEmail, Board.Name, ordinal));
                int limit = Unwrap<int>(_boardService.GetColumnLimit(_currentUserEmail, Board.Name, ordinal));

                var columnVm = new ColumnViewModel(ordinal, name, limit, _boardService, _currentUserEmail, Board.Name);

                foreach (TaskModel task in Unwrap<List<TaskModel>>(_boardService.GetColumn(_currentUserEmail, Board.Name, ordinal)) ?? new List<TaskModel>())
                {
                    task.ColumnOrdinal = ordinal;
                    columnVm.Tasks.Add(new TaskItemViewModel(task, _taskService, _currentUserEmail, Board.Name, LoadColumns));
                }

                Columns.Add(columnVm);
            }
        }

        /// <summary>
        /// Adds a new task to the backlog column (Requirement 18: new tasks always start in backlog).
        /// </summary>
        private void AddTask()
        {
            string json = _taskService.AddTask(_currentUserEmail, Board.Name, NewTaskTitle, NewTaskDescription, NewTaskDueDate);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            ErrorMessage = string.Empty;
            LoadColumns();
        }

        /// <summary>
        /// Removes the current user from this board (Requirement 14: the owner cannot leave).
        /// </summary>
        private void Leave()
        {
            string json = _boardService.LeaveBoard(_currentUserEmail, Board.Id);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            LeftBoard?.Invoke();
        }

        /// <summary>
        /// Transfers board ownership to another member (Requirement 13: owner only).
        /// </summary>
        private void TransferOwnership()
        {
            string json = _boardService.TransferBoardOwnership(_currentUserEmail, Board.Name, TransferToEmail);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            // A- update in place (instead of navigating back) so IsOwner immediately reflects that
            // A- the current user is no longer the owner, and owner-only controls hide themselves.
            Board.OwnerEmail = TransferToEmail;
            OnPropertyChanged(nameof(IsOwner));
            OnPropertyChanged(nameof(CanLeave));
            TransferToEmail = string.Empty;
            ErrorMessage = string.Empty;
        }

        // A- same shared-unwrap helper pattern as BoardsListViewModel.Unwrap - kept local to this
        // A- class (rather than factored into a static utility) since only these two ViewModels need it.
        private T Unwrap<T>(string json)
        {
            var response = JsonSerializer.Deserialize<ServiceResponse<T>>(json, JsonOptions);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                ErrorMessage = response.ErrorMessage;
            return response.ReturnValue;
        }
    }
}
