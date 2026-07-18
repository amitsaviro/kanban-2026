using System.Collections.ObjectModel;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for a single board column (backlog / in progress / done): its name,
    /// task limit, and the tasks currently in it.
    /// </summary>
    public class ColumnViewModel : ViewModelBase
    {
        private readonly BoardService _boardService;
        private readonly string _currentUserEmail;
        private readonly string _boardName;

        private string _name;
        private string _limitInput;
        private string _errorMessage = string.Empty;

        /// <summary>
        /// Creates a ColumnViewModel for one column of a board.
        /// </summary>
        /// <param name="ordinal">0 = backlog, 1 = in progress, 2 = done.</param>
        /// <param name="name">The column's display name.</param>
        /// <param name="limit">The column's current task limit (-1 = no limit).</param>
        /// <param name="boardService">The shared, DB-connected BoardService built by the composition root.</param>
        /// <param name="currentUserEmail">The email of the currently logged-in user.</param>
        /// <param name="boardName">The name of the board this column belongs to.</param>
        public ColumnViewModel(int ordinal, string name, int limit, BoardService boardService, string currentUserEmail, string boardName)
        {
            Ordinal = ordinal;
            _name = name;
            _limitInput = limit == -1 ? string.Empty : limit.ToString();
            _boardService = boardService;
            _currentUserEmail = currentUserEmail;
            _boardName = boardName;

            Tasks = new ObservableCollection<TaskItemViewModel>();
            SetLimitCommand = new RelayCommand(_ => SetLimit());
        }

        /// <summary>The column index: 0 = backlog, 1 = in progress, 2 = done.</summary>
        public int Ordinal { get; }

        /// <summary>The column's display name (e.g. "backlog").</summary>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        /// <summary>
        /// The task-limit text box content. Empty means "no limit" (Requirement 17 default).
        /// </summary>
        // A- exposed as a string (not int) so the TextBox can be temporarily empty while the
        // A- user is typing, without needing a separate "is this column unlimited" checkbox.
        public string LimitInput
        {
            get => _limitInput;
            set => SetField(ref _limitInput, value);
        }

        /// <summary>The tasks currently in this column.</summary>
        public ObservableCollection<TaskItemViewModel> Tasks { get; }

        /// <summary>The last error message for this column, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the column header's "Set" button for the task limit.</summary>
        public RelayCommand SetLimitCommand { get; }

        /// <summary>
        /// Applies <see cref="LimitInput"/> as the column's new task limit.
        /// </summary>
        // A- Requirement 16/17: empty input means "no limit" (-1); any other text must parse as
        // A- an integer, otherwise we show an error instead of silently ignoring the bad input.
        private void SetLimit()
        {
            int limit = -1;
            if (!string.IsNullOrWhiteSpace(LimitInput) && !int.TryParse(LimitInput, out limit))
            {
                ErrorMessage = "Limit must be a whole number, or empty for no limit.";
                return;
            }

            string json = _boardService.LimitColumn(_currentUserEmail, _boardName, Ordinal, limit);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            ErrorMessage = response.ErrorMessage ?? string.Empty;
        }
    }
}
