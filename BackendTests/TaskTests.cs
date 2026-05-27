using System;
using System.Text.Json;
using Backend.ServiceLayer;

namespace BackendTests
{
    /// <summary>
    /// Provides a comprehensive suite of acceptance tests for Task functionality.
    /// Covers adding, updating, advancing tasks, and enforcing column and done constraints.
    /// </summary>
    public class TaskTests
    {
        private UserService userService = new UserService();
        private BoardService boardService = new BoardService();
        private TaskService taskService = new TaskService();

        // Unique user and board for task testing to prevent state collisions
        private const string EMAIL = "task_master@test.com";
        private const string PASSWORD = "Password123";
        private const string BOARD_NAME = "ProjectTasks";

        /// <summary>
        /// Parses a raw JSON response string into a structured Response object.
        /// (Uses the Response class already defined in BoardTests.cs)
        /// </summary>
        private Response ParseResponse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json)!;
        }

        // --- 1. Basic Add & Invalid Inputs ---

        /// <summary>
        /// Tests valid task creation in the Backlog column.
        /// Covers Requirement 13 (Adding a task).
        /// </summary>
        public void TestAddValidTask()
        {
            Console.WriteLine("Running TestAddValidTask...");
            Response res = ParseResponse(taskService.AddTask(EMAIL, BOARD_NAME, "Setup Database", "Install SQL Server", DateTime.Now.AddDays(7)));
            
            if (res.ErrorMessage == null) Console.WriteLine("-> PASSED: Task added successfully.");
            else Console.WriteLine($"-> FAILED: Failed to add task. Error: {res.ErrorMessage}");
        }

        /// <summary>
        /// Tests invalid task inputs (empty title, excessively long title/description).
        /// Covers Requirement 13 (Field limits) and Requirement 20a (Invalid input).
        /// </summary>
        public void TestAddInvalidTask()
        {
            Console.WriteLine("Running TestAddInvalidTask (Edge Cases)...");
            
            string longTitle = new string('A', 51); // Max allowed is 50
            string longDesc = new string('B', 301); // Max allowed is 300

            Response resEmptyTitle = ParseResponse(taskService.AddTask(EMAIL, BOARD_NAME, "", "Desc", DateTime.Now));
            Response resLongTitle = ParseResponse(taskService.AddTask(EMAIL, BOARD_NAME, longTitle, "Desc", DateTime.Now));
            Response resLongDesc = ParseResponse(taskService.AddTask(EMAIL, BOARD_NAME, "Valid Title", longDesc, DateTime.Now));

            if (resEmptyTitle.ErrorMessage != null && resLongTitle.ErrorMessage != null && resLongDesc.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Invalid task fields correctly blocked upon creation.");
            else
                Console.WriteLine("-> FAILED: System accepted a task with invalid field lengths!");
        }

        // --- 2. Update Actions (Success & Edge Cases) ---

        /// <summary>
        /// Tests successful updating of a task's title, description, and due date.
        /// Covers Requirements 14, 15, and 16.
        /// </summary>
        public void TestUpdateTaskSuccess()
        {
            Console.WriteLine("Running TestUpdateTaskSuccess...");
            // Task ID 0 is currently in column 0 (Backlog)
            Response res1 = ParseResponse(taskService.UpdateTaskTitle(EMAIL, BOARD_NAME, 0, 0, "Updated Title"));
            Response res2 = ParseResponse(taskService.UpdateTaskDescription(EMAIL, BOARD_NAME, 0, 0, "Updated Description"));
            Response res3 = ParseResponse(taskService.UpdateTaskDueDate(EMAIL, BOARD_NAME, 0, 0, DateTime.Now.AddDays(14)));

            if (res1.ErrorMessage == null && res2.ErrorMessage == null && res3.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Task details updated successfully.");
            else
                Console.WriteLine($"-> FAILED: Update failed. Errors: {res1.ErrorMessage} | {res2.ErrorMessage} | {res3.ErrorMessage}");
        }

        /// <summary>
        /// Tests that a task cannot be updated with invalid values.
        /// Covers Requirement 14, 15, and 20a.
        /// </summary>
        public void TestUpdateInvalidTask()
        {
            Console.WriteLine("Running TestUpdateInvalidTask (Edge Cases)...");
            string longDesc = new string('Z', 301);
            
            Response resEmptyTitle = ParseResponse(taskService.UpdateTaskTitle(EMAIL, BOARD_NAME, 0, 0, ""));
            Response resLongDesc = ParseResponse(taskService.UpdateTaskDescription(EMAIL, BOARD_NAME, 0, 0, longDesc));

            if (resEmptyTitle.ErrorMessage != null && resLongDesc.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Blocked updating a task to invalid title/description.");
            else
                Console.WriteLine("-> FAILED: Allowed updating task to invalid values!");
        }

        // --- 3. Advancement & Done Logic ---

        /// <summary>
        /// Tests advancing a task sequentially across columns.
        /// Covers Requirement 17 (Advancing tasks).
        /// </summary>
        public void TestAdvanceTask()
        {
            Console.WriteLine("Running TestAdvanceTask...");
            Response advanceTo1 = ParseResponse(taskService.AdvanceTask(EMAIL, BOARD_NAME, 0, 0));
            Response advanceTo2 = ParseResponse(taskService.AdvanceTask(EMAIL, BOARD_NAME, 1, 0));

            if (advanceTo1.ErrorMessage == null && advanceTo2.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Task advanced through columns successfully.");
            else
                Console.WriteLine($"-> FAILED: Task advancement failed. Errors: {advanceTo1.ErrorMessage} | {advanceTo2.ErrorMessage}");
        }

        /// <summary>
        /// Tests that tasks in the 'Done' column cannot be edited or advanced.
        /// Covers Requirement 17 and Requirement 18.
        /// </summary>
        public void TestDoneColumnConstraints()
        {
            Console.WriteLine("Running TestDoneColumnConstraints (Edge Case)...");
            // Task 0 is now in Done (Column 2).
            Response editRes = ParseResponse(taskService.UpdateTaskTitle(EMAIL, BOARD_NAME, 2, 0, "Hacked Title"));
            Response advanceRes = ParseResponse(taskService.AdvanceTask(EMAIL, BOARD_NAME, 2, 0));

            if (editRes.ErrorMessage != null && advanceRes.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Correctly blocked editing and advancing of a task in the 'Done' column.");
            else
                Console.WriteLine("-> FAILED: System allowed modification of a completed task!");
        }

        /// <summary>
        /// Tests that column capacity limits prevent tasks from advancing.
        /// Covers Requirement 19 (Enforcing column limits during advancement).
        /// </summary>
        public void TestColumnLimitEnforcement()
        {
            Console.WriteLine("Running TestColumnLimitEnforcement (Integration Case)...");
            
            boardService.LimitColumn(EMAIL, BOARD_NAME, 1, 1);

            taskService.AddTask(EMAIL, BOARD_NAME, "Task A", "Desc", DateTime.Now);
            taskService.AddTask(EMAIL, BOARD_NAME, "Task B", "Desc", DateTime.Now);

            // Task A is ID 1, Task B is ID 2 (Task 0 exists from previous tests)
            Response adv1 = ParseResponse(taskService.AdvanceTask(EMAIL, BOARD_NAME, 0, 1));
            Response adv2 = ParseResponse(taskService.AdvanceTask(EMAIL, BOARD_NAME, 0, 2));

            if (adv1.ErrorMessage == null && adv2.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Column task limits strictly enforced during advancement.");
            else
                Console.WriteLine($"-> FAILED: Limit enforcement failed! Adv1 Error: {adv1.ErrorMessage}, Adv2 Error: {adv2.ErrorMessage}");
        }

        // --- 4. Permissions & System Edge Cases ---

        /// <summary>
        /// Tests that accessing or modifying a non-existent task fails safely.
        /// Covers Requirement 20a.
        /// </summary>
        public void TestNonExistentTask()
        {
            Console.WriteLine("Running TestNonExistentTask (Edge Case)...");
            Response res = ParseResponse(taskService.UpdateTaskTitle(EMAIL, BOARD_NAME, 0, 999, "Ghost Task"));
            
            if (res.ErrorMessage != null) Console.WriteLine("-> PASSED: Accessing non-existent task correctly blocked.");
            else Console.WriteLine("-> FAILED: System allowed updating a task that doesn't exist!");
        }

        /// <summary>
        /// Tests that task actions are blocked if the user is not logged in.
        /// Covers Requirement 7 and Requirement 20b.
        /// </summary>
        public void TestTaskActionsLoggedOut()
        {
            Console.WriteLine("Running TestTaskActionsLoggedOut (Edge Case)...");
            userService.Logout(EMAIL);
            Response res = ParseResponse(taskService.AddTask(EMAIL, BOARD_NAME, "Secret Task", "Desc", DateTime.Now));
            userService.Login(EMAIL, PASSWORD); // Restore session

            if (res.ErrorMessage != null) Console.WriteLine("-> PASSED: Correctly blocked task action for a logged-out user.");
            else Console.WriteLine("-> FAILED: System allowed task creation for a logged-out user!");
        }

        /// <summary>
        /// Runs all Acceptance Tests for Task functionality sequentially.
        /// </summary>
        public void RunAll()
        {
            Console.WriteLine("=== STARTING TASK ACCEPTANCE TESTS ===");

            // Setup: Register a user and create a board specifically for these tests
            userService.Register(EMAIL, PASSWORD);
            boardService.CreateBoard(EMAIL, BOARD_NAME);

            TestAddValidTask();
            TestAddInvalidTask();
            TestUpdateTaskSuccess();
            TestUpdateInvalidTask();
            TestAdvanceTask();
            TestDoneColumnConstraints();
            TestColumnLimitEnforcement();
            TestNonExistentTask();
            TestTaskActionsLoggedOut();

            Console.WriteLine("=== FINISHED TASK ACCEPTANCE TESTS ===\n");
        }
    }
}