using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for the boards-list screen (Requirement 29.b): shows the current
    /// user's boards (name + owner only, no IDs), and lets them create/delete boards
    /// and open one to see its columns.
    /// </summary>
    public class BoardsListViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private readonly BoardService _boardService;
        private readonly string _currentUserEmail;

        private string _newBoardName = string.Empty;
        private string _errorMessage = string.Empty;

        /// <summary>Raised when the user opens a board, carrying the chosen board.</summary>
        // A- same navigation pattern as AuthViewModel.AuthSucceeded - MainWindowViewModel listens
        // A- and switches CurrentViewModel to a new BoardViewModel for the chosen board.
        public event Action<BoardModel> BoardOpened;

        /// <summary>Raised after the current user logs out, so MainWindowViewModel can return to the Auth screen.</summary>
        // A- added because there was previously no way to log out and sign in as a different user
        // A- without closing and relaunching the whole app.
        public event Action LoggedOut;

        /// <summary>Raised when the user wants to see their in-progress tasks across all boards (Requirement 22).</summary>
        public event Action ShowInProgressTasksRequested;

        /// <summary>
        /// Creates the BoardsListViewModel and immediately loads the user's boards.
        /// </summary>
        /// <param name="userService">The shared, DB-connected UserService built by the composition root.</param>
        /// <param name="boardService">The shared, DB-connected BoardService built by the composition root.</param>
        /// <param name="currentUserEmail">The email of the currently logged-in user.</param>
        public BoardsListViewModel(UserService userService, BoardService boardService, string currentUserEmail)
        {
            _userService = userService;
            _boardService = boardService;
            _currentUserEmail = currentUserEmail;

            Boards = new ObservableCollection<BoardModel>();
            CreateBoardCommand = new RelayCommand(_ => CreateBoard(), _ => !string.IsNullOrWhiteSpace(NewBoardName));
            DeleteBoardCommand = new RelayCommand(board => DeleteBoard((BoardModel)board));
            OpenBoardCommand = new RelayCommand(board => BoardOpened?.Invoke((BoardModel)board));
            LogoutCommand = new RelayCommand(_ => Logout());
            ShowInProgressTasksCommand = new RelayCommand(_ => ShowInProgressTasksRequested?.Invoke());

            LoadBoards();
        }

        /// <summary>The boards belonging to the current user. Bound to the boards list View.</summary>
        public ObservableCollection<BoardModel> Boards { get; }

        /// <summary>The name typed into the "new board" text box.</summary>
        public string NewBoardName
        {
            get => _newBoardName;
            set => SetField(ref _newBoardName, value);
        }

        /// <summary>The last error message to display, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the "Create" button.</summary>
        public RelayCommand CreateBoardCommand { get; }

        /// <summary>Command bound to each board row's "Delete" button. Parameter: the BoardModel to delete.</summary>
        public RelayCommand DeleteBoardCommand { get; }

        /// <summary>Command bound to each board row (e.g. double-click) to open it. Parameter: the BoardModel to open.</summary>
        public RelayCommand OpenBoardCommand { get; }

        /// <summary>Command bound to the "Logout" button.</summary>
        public RelayCommand LogoutCommand { get; }

        /// <summary>Command bound to the "In Progress" button (Requirement 22).</summary>
        public RelayCommand ShowInProgressTasksCommand { get; }

        /// <summary>The email of the currently logged-in user, shown next to the Logout button.</summary>
        public string CurrentUserEmail => _currentUserEmail;

        /// <summary>
        /// Reloads <see cref="Boards"/> from the Backend: fetches the user's board IDs,
        /// then the name and owner of each one.
        /// </summary>
        // A- BoardService.GetUserBoards only returns IDs - this method is what actually
        // A- assembles a full BoardModel (name + owner) per ID, one service call at a time.
        private void LoadBoards()
        {
            Boards.Clear();

            string idsJson = _boardService.GetUserBoards(_currentUserEmail);
            var idsResponse = JsonSerializer.Deserialize<ServiceResponse<List<int>>>(idsJson, JsonOptions);

            if (!string.IsNullOrEmpty(idsResponse.ErrorMessage))
            {
                ErrorMessage = idsResponse.ErrorMessage;
                return;
            }

            foreach (int boardId in idsResponse.ReturnValue ?? Enumerable.Empty<int>())
            {
                string name = Unwrap<string>(_boardService.GetBoardName(boardId));
                string owner = Unwrap<string>(_boardService.GetBoardOwner(boardId));

                Boards.Add(new BoardModel { Id = boardId, Name = name, OwnerEmail = owner });
            }

            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// Creates a new board owned by the current user, then refreshes the list.
        /// </summary>
        private void CreateBoard()
        {
            string json = _boardService.CreateBoard(_currentUserEmail, NewBoardName);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            NewBoardName = string.Empty;
            LoadBoards();
        }

        /// <summary>
        /// Deletes the given board (only allowed if the current user is its owner -
        /// enforced by the Backend), then refreshes the list.
        /// </summary>
        /// <param name="board">The board to delete.</param>
        private void DeleteBoard(BoardModel board)
        {
            if (board == null)
                return;

            string json = _boardService.DeleteBoard(_currentUserEmail, board.Name);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            LoadBoards();
        }

        /// <summary>
        /// Logs the current user out (Requirement 7) and raises <see cref="LoggedOut"/> so
        /// MainWindowViewModel can navigate back to the Auth screen.
        /// </summary>
        private void Logout()
        {
            string json = _userService.Logout(_currentUserEmail);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            LoggedOut?.Invoke();
        }

        // A- small local helper so every "GetX(id)" call above (which all return a plain
        // A- string/primitive ReturnValue) doesn't need to repeat the same 3-line deserialize+check.
        private T Unwrap<T>(string json)
        {
            var response = JsonSerializer.Deserialize<ServiceResponse<T>>(json, JsonOptions);
            return response.ReturnValue;
        }
    }
}
