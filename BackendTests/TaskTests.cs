using System;
using Backend.ServiceLayer;

namespace BackendTests
{
    public class TaskTests
    {
        private UserService userService = new UserService();
        private BoardService boardService = new BoardService();
        private TaskService taskService = new TaskService();

        /// <summary>
        /// Tests adding a task.
        /// Requirement 13.
        /// </summary>
        public void TestAddTask()
        {
            Console.WriteLine("Running TestAddTask...");

            userService.Register("task@test.com", "Password1");

            boardService.CreateBoard(
                "task@test.com",
                "Work"
            );

            string result = taskService.AddTask(
                "task@test.com",
                "Work",
                "Finish HW",
                "Complete SPL assignment",
                DateTime.Now.AddDays(3)
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests advancing a task.
        /// Requirement 14.
        /// </summary>
        public void TestAdvanceTask()
        {
            Console.WriteLine("Running TestAdvanceTask...");

            userService.Register("advance@test.com", "Password1");

            boardService.CreateBoard(
                "advance@test.com",
                "Work"
            );

            taskService.AddTask(
                "advance@test.com",
                "Work",
                "Task1",
                "Desc",
                DateTime.Now.AddDays(2)
            );

            string result = taskService.AdvanceTask(
                "advance@test.com",
                "Work",
                0,
                0
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests updating task title.
        /// Requirement 16.
        /// </summary>
        public void TestUpdateTitle()
        {
            Console.WriteLine("Running TestUpdateTitle...");

            userService.Register("title@test.com", "Password1");

            boardService.CreateBoard(
                "title@test.com",
                "School"
            );

            taskService.AddTask(
                "title@test.com",
                "School",
                "Old",
                "Desc",
                DateTime.Now.AddDays(1)
            );

            string result = taskService.UpdateTaskTitle(
                "title@test.com",
                "School",
                0,
                0,
                "New Title"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests updating task description.
        /// Requirement 16.
        /// </summary>
        public void TestUpdateDescription()
        {
            Console.WriteLine("Running TestUpdateDescription...");

            userService.Register("desc@test.com", "Password1");

            boardService.CreateBoard(
                "desc@test.com",
                "School"
            );

            taskService.AddTask(
                "desc@test.com",
                "School",
                "Task",
                "Old Desc",
                DateTime.Now.AddDays(1)
            );

            string result = taskService.UpdateTaskDescription(
                "desc@test.com",
                "School",
                0,
                0,
                "New Description"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests retrieving in progress tasks.
        /// Requirement 17.
        /// </summary>
        public void TestInProgressTasks()
        {
            Console.WriteLine("Running TestInProgressTasks...");

            userService.Register("progress@test.com", "Password1");

            boardService.CreateBoard(
                "progress@test.com",
                "Work"
            );

            taskService.AddTask(
                "progress@test.com",
                "Work",
                "Task",
                "Desc",
                DateTime.Now.AddDays(1)
            );

            taskService.AdvanceTask(
                "progress@test.com",
                "Work",
                0,
                0
            );

            string result = taskService.InProgressTasks(
                "progress@test.com"
            );

            Console.WriteLine(result);
        }

        public void RunAll()
        {
            TestAddTask();
            TestAdvanceTask();
            TestUpdateTitle();
            TestUpdateDescription();
            TestInProgressTasks();
        }
    }
}