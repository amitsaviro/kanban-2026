using System.Windows;
using Backend.Facades;
using Backend.ServiceLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using Frontend.ViewModels;

namespace Frontend
{
    /// <summary>
    /// The application's composition root. On startup, wires up a DB-connected, shared
    /// service layer (UserService/BoardService/TaskService) and restores any previously
    /// persisted data (Requirement 4.c: the GUI must load persisted data automatically),
    /// then shows the main window.
    /// </summary>
    // A- this class exists because UserService()/BoardService()/TaskService()'s parameterless
    // A- constructors each build their own, unconnected, non-persisted UserFacade - the only
    // A- place that wired them together with a shared UserFacade and the DB was GradingService.cs,
    // A- which the Frontend must not use. Everything below calls only public, pre-existing
    // A- Backend constructors/methods - no existing Backend file's behavior was changed for this.
    public partial class App : Application
    {
        /// <summary>
        /// Builds the service layer, restores persisted data, and opens the main window.
        /// </summary>
        /// <param name="e">Startup event args (unused - no command-line arguments are expected).</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            (UserService userService, BoardService boardService, TaskService taskService) = BuildServices();

            var mainWindowViewModel = new MainWindowViewModel(userService, boardService, taskService);
            var mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Show();
        }

        /// <summary>
        /// Creates one shared <see cref="UserFacade"/>/<see cref="BoardFacade"/>/<see cref="TaskFacade"/>,
        /// each wired to the same SQLite-backed controllers, then restores any data already
        /// persisted in kanban.db into memory before returning the three services built on top of them.
        /// </summary>
        // A- this method mirrors GradingService.LoadData's wiring + load sequence (same DTOs, same
        // A- Load* calls, same order: users, then boards, then column limits, then members, then
        // A- tasks - order matters, e.g. limits must be restored before tasks so Column.AddTask
        // A- respects them) but built independently, through our own service layer, per Milestone 3's
        // A- instruction that the Frontend must use "your service layer" and not GradingService.cs.
        private (UserService, BoardService, TaskService) BuildServices()
        {
            DataBaseManager dbManager = new DataBaseManager();
            dbManager.CreateSchema();

            UserController userCtrl = new UserController(dbManager);
            BoardController boardCtrl = new BoardController(dbManager);
            ColumnController columnCtrl = new ColumnController(dbManager);
            TaskController taskCtrl = new TaskController(dbManager);
            UserBoardsController membersCtrl = new UserBoardsController(dbManager);

            UserFacade userFacade = new UserFacade(userCtrl);
            BoardFacade boardFacade = new BoardFacade(userFacade, boardCtrl, columnCtrl, membersCtrl, taskCtrl);
            TaskFacade taskFacade = new TaskFacade(userFacade, taskCtrl, boardCtrl);

            foreach (UserDTO dto in userCtrl.LoadAll())
                userFacade.LoadUser(dto.Email, dto.Password);

            foreach (BoardDTO dto in boardCtrl.LoadAll())
                boardFacade.LoadBoard(dto);

            foreach (ColumnDTO dto in columnCtrl.LoadAll())
                boardFacade.LoadColumnLimit(dto.BoardId, dto.Ordinal, dto.Limit);

            foreach (UserBoardsDTO dto in membersCtrl.LoadAll())
                boardFacade.LoadMember(dto.BoardId, dto.UserEmail);

            foreach (TaskDTO dto in taskCtrl.LoadAll())
                boardFacade.LoadTask(dto);

            UserService userService = new UserService(userFacade);
            BoardService boardService = new BoardService(boardFacade);
            TaskService taskService = new TaskService(taskFacade);

            return (userService, boardService, taskService);
        }
    }
}
