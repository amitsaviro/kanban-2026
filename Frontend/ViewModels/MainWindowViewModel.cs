using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// Top-level, ViewModel-first navigation controller. Owns the single service instances
    /// (built once by the composition root) and swaps <see cref="CurrentViewModel"/> between
    /// Auth -> BoardsList -> Board as the user navigates, so no View ever performs navigation itself.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private readonly BoardService _boardService;
        private readonly TaskService _taskService;

        private ViewModelBase _currentViewModel;
        private string _currentUserEmail;

        /// <summary>
        /// Creates the MainWindowViewModel, starting on the Auth screen.
        /// </summary>
        /// <param name="userService">The shared, DB-connected UserService built by the composition root.</param>
        /// <param name="boardService">The shared, DB-connected BoardService built by the composition root.</param>
        /// <param name="taskService">The shared, DB-connected TaskService built by the composition root.</param>
        // A- the three services are constructed exactly once, in App.xaml.cs, and threaded through
        // A- here - no ViewModel in this project ever does "new UserService()" etc. itself, so every
        // A- screen shares the same in-memory state and the same DB-backed facades (see the wiring
        // A- discussion: UserService()/BoardService()/TaskService()'s parameterless constructors do NOT
        // A- share a UserFacade or connect to the DB on their own).
        public MainWindowViewModel(UserService userService, BoardService boardService, TaskService taskService)
        {
            _userService = userService;
            _boardService = boardService;
            _taskService = taskService;

            ShowAuth();
        }

        /// <summary>The ViewModel for whichever screen is currently displayed. MainWindow.xaml binds a ContentControl to this.</summary>
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            private set => SetField(ref _currentViewModel, value);
        }

        /// <summary>Shows the Registration/Login screen.</summary>
        private void ShowAuth()
        {
            var auth = new AuthViewModel(_userService);
            auth.AuthSucceeded += OnAuthSucceeded;
            CurrentViewModel = auth;
        }

        private void OnAuthSucceeded(string email)
        {
            _currentUserEmail = email;
            ShowBoardsList();
        }

        /// <summary>Shows the current user's boards list screen.</summary>
        private void ShowBoardsList()
        {
            var boardsList = new BoardsListViewModel(_userService, _boardService, _currentUserEmail);
            boardsList.BoardOpened += OnBoardOpened;
            boardsList.LoggedOut += ShowAuth;
            boardsList.ShowInProgressTasksRequested += ShowInProgressTasks;
            CurrentViewModel = boardsList;
        }

        private void OnBoardOpened(BoardModel board)
        {
            var boardVm = new BoardViewModel(board, _boardService, _taskService, _currentUserEmail);
            boardVm.BackRequested += ShowBoardsList;
            boardVm.LeftBoard += ShowBoardsList;
            CurrentViewModel = boardVm;
        }

        /// <summary>Shows the current user's in-progress tasks across all their boards (Requirement 22).</summary>
        private void ShowInProgressTasks()
        {
            var vm = new InProgressTasksViewModel(_taskService, _currentUserEmail);
            vm.BackRequested += ShowBoardsList;
            CurrentViewModel = vm;
        }
    }
}
