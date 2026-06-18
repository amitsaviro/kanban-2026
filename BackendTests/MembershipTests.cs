using System;
using System.Text.Json;
using Backend.Facades;
using Backend.ServiceLayer;

namespace BackendTests
{
    /// <summary>
    /// Acceptance tests for Milestone 2 board membership, ownership, and task assignment features.
    /// Covers Requirements 8, 11, 12, 13, 14, 15, 18, 19, 20, 22, and 23.
    /// </summary>
    public class MembershipTests
    {
        private UserService userService;
        private BoardService boardService;
        private TaskService taskService;

        // Y - shared UserFacade so all three services operate on the same user/board state
        public MembershipTests()
        {
            UserFacade uf = new UserFacade();
            userService = new UserService(uf);
            boardService = new BoardService(new BoardFacade(uf));
            taskService = new TaskService(new TaskFacade(uf));
        }

        // Y - unique email addresses so this test class does not collide with UserTests/BoardTests/TaskTests
        private const string OWNER = "owner_member@test.com";
        private const string MEMBER = "regular_member@test.com";
        private const string OUTSIDER = "outsider_member@test.com";
        private const string PASSWORD = "Password1";
        private const string BOARD_NAME = "MembershipBoard";

        // Y - board ID is set during setup once CreateBoard + GetUserBoards are implemented
        private int boardId = -1;

        /// <summary>
        /// Parses a raw JSON response string into a structured Response object.
        /// </summary>
        private Response ParseResponse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json)!;
        }

        // ─── Requirement 11 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that only the board owner can delete the board, not a regular member.
        /// Covers Requirement 11.
        /// </summary>
        public void TestOnlyOwnerCanDelete()
        {
            Console.WriteLine("Running TestOnlyOwnerCanDelete (Requirement 11)...");

            // Y - member tries to delete a board they don't own — should fail
            Response memberDeleteRes = ParseResponse(boardService.DeleteBoard(MEMBER, BOARD_NAME));

            // Y - owner deletes the board — should succeed
            Response ownerDeleteRes = ParseResponse(boardService.DeleteBoard(OWNER, BOARD_NAME));

            // Y - recreate the board so later tests still work
            boardService.CreateBoard(OWNER, BOARD_NAME);

            if (memberDeleteRes.ErrorMessage != null && ownerDeleteRes.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Only owner can delete the board.");
            else
                Console.WriteLine($"-> FAILED: Member delete error: {memberDeleteRes.ErrorMessage}, Owner delete error: {ownerDeleteRes.ErrorMessage}");
        }

        // ─── Requirement 12 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that any logged-in user can join an existing board by ID.
        /// Covers Requirement 12.
        /// </summary>
        public void TestJoinBoard()
        {
            Console.WriteLine("Running TestJoinBoard (Requirement 12)...");

            // Y - outsider joins the board using its global ID; boardId was set in setup
            Response joinRes = ParseResponse(boardService.JoinBoard(OUTSIDER, boardId));

            if (joinRes.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Outsider successfully joined the board.");
            else
                Console.WriteLine($"-> FAILED: JoinBoard returned error: {joinRes.ErrorMessage}");
        }

        /// <summary>
        /// Tests that joining an invalid board ID fails gracefully.
        /// Covers Requirement 12 and Requirement 26a (malformed input).
        /// </summary>
        public void TestJoinNonExistentBoard()
        {
            Console.WriteLine("Running TestJoinNonExistentBoard (Requirement 12, Edge Case)...");

            Response res = ParseResponse(boardService.JoinBoard(OUTSIDER, -999));

            if (res.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Joining a non-existent board ID correctly blocked.");
            else
                Console.WriteLine("-> FAILED: System allowed joining a board that does not exist!");
        }

        // ─── Requirement 13 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that the board owner can transfer ownership to an existing member.
        /// Covers Requirement 13.
        /// </summary>
        public void TestTransferOwnership()
        {
            Console.WriteLine("Running TestTransferOwnership (Requirement 13)...");

            // Y - owner transfers ownership to MEMBER who already joined in setup
            Response transferRes = ParseResponse(boardService.TransferBoardOwnership(OWNER, BOARD_NAME, MEMBER));

            // Y - transfer back so remaining tests still have OWNER as the owner
            boardService.TransferBoardOwnership(MEMBER, BOARD_NAME, OWNER);

            if (transferRes.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Ownership transferred successfully.");
            else
                Console.WriteLine($"-> FAILED: TransferBoardOwnership returned error: {transferRes.ErrorMessage}");
        }

        /// <summary>
        /// Tests that a non-owner cannot transfer ownership.
        /// Covers Requirement 13.
        /// </summary>
        public void TestNonOwnerCannotTransfer()
        {
            Console.WriteLine("Running TestNonOwnerCannotTransfer (Requirement 13, Edge Case)...");

            // Y - MEMBER is not the owner so this should fail
            Response res = ParseResponse(boardService.TransferBoardOwnership(MEMBER, BOARD_NAME, OUTSIDER));

            if (res.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Non-owner correctly blocked from transferring ownership.");
            else
                Console.WriteLine("-> FAILED: System allowed a non-owner to transfer board ownership!");
        }

        // ─── Requirement 14 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that the board owner cannot leave the board.
        /// Covers Requirement 14.
        /// </summary>
        public void TestOwnerCannotLeave()
        {
            Console.WriteLine("Running TestOwnerCannotLeave (Requirement 14)...");

            Response res = ParseResponse(boardService.LeaveBoard(OWNER, boardId));

            if (res.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Owner correctly blocked from leaving the board.");
            else
                Console.WriteLine("-> FAILED: System allowed the owner to leave their own board!");
        }

        // ─── Requirement 15 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that when a user leaves a board, their non-done assigned tasks become unassigned.
        /// Covers Requirement 15.
        /// </summary>
        public void TestLeaveUnassignsTasks()
        {
            Console.WriteLine("Running TestLeaveUnassignsTasks (Requirement 15)...");

            // Y - add a task and assign it to MEMBER, then MEMBER leaves
            taskService.AddTask(OWNER, BOARD_NAME, "Leave Test Task", "Desc", DateTime.Now.AddDays(5));
            // Y - task ID 0 is in backlog (column 0); assign it to MEMBER
            taskService.AssignTask(OWNER, BOARD_NAME, 0, 0, MEMBER);

            // Y - MEMBER leaves the board; task should become unassigned
            boardService.LeaveBoard(MEMBER, boardId);

            // A - get the backlog column and verify task 0's assignee is now null
            Response colRes = ParseResponse(boardService.GetColumn(OWNER, BOARD_NAME, 0));

            // A - MEMBER rejoins so later tests still work
            boardService.JoinBoard(MEMBER, boardId);

            if (colRes.ErrorMessage != null)
            {
                Console.WriteLine($"-> FAILED: Could not read column after member left. Error: {colRes.ErrorMessage}");
                return;
            }

            // A - parse the task list and find task ID 0; its Assignee must be null
            bool taskUnassigned = false;
            if (colRes.ReturnValue != null)
            {
                var tasks = JsonSerializer.Deserialize<JsonElement[]>(colRes.ReturnValue.ToString()!);
                if (tasks != null)
                {
                    foreach (var t in tasks)
                    {
                        if (t.TryGetProperty("Id", out var idProp) && idProp.GetInt32() == 0)
                        {
                            // A - Assignee should be null after member left (Requirement 15)
                            if (!t.TryGetProperty("Assignee", out var assigneeProp) ||
                                assigneeProp.ValueKind == JsonValueKind.Null)
                                taskUnassigned = true;
                        }
                    }
                }
            }

            if (taskUnassigned)
                Console.WriteLine("-> PASSED: Task assignee was correctly cleared to null after member left.");
            else
                Console.WriteLine("-> FAILED: Task assignee was NOT cleared after member left the board.");
        }

        // ─── Requirement 18 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that only board members can add tasks to the board.
        /// Covers Requirement 18.
        /// </summary>
        public void TestOnlyMemberCanAddTask()
        {
            Console.WriteLine("Running TestOnlyMemberCanAddTask (Requirement 18)...");

            // Y - OUTSIDER is not yet a member (before JoinBoard tests run) so this should fail
            string tempOutsider = "fresh_outsider@test.com";
            userService.Register(tempOutsider, PASSWORD);

            Response res = ParseResponse(taskService.AddTask(tempOutsider, BOARD_NAME, "Sneaky Task", "Desc", DateTime.Now));

            if (res.ErrorMessage != null)
                Console.WriteLine("-> PASSED: Non-member correctly blocked from adding a task.");
            else
                Console.WriteLine("-> FAILED: System allowed a non-member to add a task!");
        }

        // ─── Requirement 19 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that only the task's assignee can advance it to the next column.
        /// Covers Requirement 19.
        /// </summary>
        public void TestOnlyAssigneeCanAdvance()
        {
            Console.WriteLine("Running TestOnlyAssigneeCanAdvance (Requirement 19)...");

            // Y - add a new task and assign it to MEMBER
            taskService.AddTask(OWNER, BOARD_NAME, "Advance Test Task", "Desc", DateTime.Now.AddDays(3));
            int taskId = 1; // Y - second task added, so ID = 1
            taskService.AssignTask(OWNER, BOARD_NAME, 0, taskId, MEMBER);

            // Y - OWNER tries to advance — should fail because they are not the assignee
            Response ownerAdvance = ParseResponse(taskService.AdvanceTask(OWNER, BOARD_NAME, 0, taskId));

            // Y - MEMBER (the assignee) advances — should succeed
            Response memberAdvance = ParseResponse(taskService.AdvanceTask(MEMBER, BOARD_NAME, 0, taskId));

            if (ownerAdvance.ErrorMessage != null && memberAdvance.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Only the assignee can advance a task.");
            else
                Console.WriteLine($"-> FAILED: Owner advance error: {ownerAdvance.ErrorMessage}, Member advance error: {memberAdvance.ErrorMessage}");
        }

        // ─── Requirement 20 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that only the task's assignee or the board owner can edit a non-done task.
        /// A regular member who is not the assignee should be blocked.
        /// Covers Requirement 20.
        /// </summary>
        public void TestOnlyAssigneeOrOwnerCanEdit()
        {
            Console.WriteLine("Running TestOnlyAssigneeOrOwnerCanEdit (Requirement 20)...");

            // Y - add a task assigned to MEMBER; OUTSIDER (a member but not assignee) tries to edit
            taskService.AddTask(OWNER, BOARD_NAME, "Edit Guard Task", "Desc", DateTime.Now.AddDays(2));
            int taskId = 2;
            taskService.AssignTask(OWNER, BOARD_NAME, 0, taskId, MEMBER);

            // Y - OUTSIDER is a board member but NOT the assignee — edit should fail
            Response outsiderEdit = ParseResponse(taskService.UpdateTaskTitle(OUTSIDER, BOARD_NAME, 0, taskId, "Hacked Title"));

            // Y - MEMBER (assignee) edits — should succeed
            Response memberEdit = ParseResponse(taskService.UpdateTaskTitle(MEMBER, BOARD_NAME, 0, taskId, "Updated By Assignee"));

            // Y - OWNER (board owner) edits — should succeed
            Response ownerEdit = ParseResponse(taskService.UpdateTaskTitle(OWNER, BOARD_NAME, 0, taskId, "Updated By Owner"));

            if (outsiderEdit.ErrorMessage != null && memberEdit.ErrorMessage == null && ownerEdit.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Only assignee and board owner can edit a non-done task.");
            else
                Console.WriteLine($"-> FAILED: Outsider error: {outsiderEdit.ErrorMessage}, Member error: {memberEdit.ErrorMessage}, Owner error: {ownerEdit.ErrorMessage}");
        }

        // ─── Requirement 22 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that InProgressTasks returns only tasks assigned to the calling user, not all in-progress tasks.
        /// Covers Requirement 22.
        /// </summary>
        public void TestInProgressTasksFiltersByAssignee()
        {
            Console.WriteLine("Running TestInProgressTasksFiltersByAssignee (Requirement 22)...");

            // Y - add a task, assign it to MEMBER, advance it to in-progress
            taskService.AddTask(OWNER, BOARD_NAME, "In Progress Task", "Desc", DateTime.Now.AddDays(1));
            int taskId = 3;
            taskService.AssignTask(OWNER, BOARD_NAME, 0, taskId, MEMBER);
            taskService.AdvanceTask(MEMBER, BOARD_NAME, 0, taskId);

            // Y - OWNER calls InProgressTasks — should NOT include the task assigned to MEMBER
            Response ownerRes = ParseResponse(taskService.InProgressTasks(OWNER));

            // Y - MEMBER calls InProgressTasks — should include the task assigned to them
            Response memberRes = ParseResponse(taskService.InProgressTasks(MEMBER));

            if (ownerRes.ErrorMessage != null || memberRes.ErrorMessage != null)
            {
                Console.WriteLine($"-> FAILED: Owner error: {ownerRes.ErrorMessage}, Member error: {memberRes.ErrorMessage}");
                return;
            }

            // A - verify OWNER's list is empty and MEMBER's list contains at least the task
            var ownerTasks = ownerRes.ReturnValue != null
                ? JsonSerializer.Deserialize<JsonElement[]>(ownerRes.ReturnValue.ToString()!) : null;
            var memberTasks = memberRes.ReturnValue != null
                ? JsonSerializer.Deserialize<JsonElement[]>(memberRes.ReturnValue.ToString()!) : null;

            bool ownerEmpty = ownerTasks == null || ownerTasks.Length == 0;
            bool memberHasTask = memberTasks != null && memberTasks.Length > 0;

            if (ownerEmpty && memberHasTask)
                Console.WriteLine("-> PASSED: InProgressTasks correctly filtered by assignee.");
            else
                Console.WriteLine($"-> FAILED: Owner task count={ownerTasks?.Length ?? 0} (expected 0), Member task count={memberTasks?.Length ?? 0} (expected >0).");
        }

        // ─── Requirement 23 ────────────────────────────────────────────────────────

        /// <summary>
        /// Tests that an unassigned task can be assigned by any board member, but a task that
        /// already has an assignee can only be reassigned by that assignee or the board owner.
        /// Covers Requirement 23.
        /// </summary>
        public void TestAssignTaskRules()
        {
            Console.WriteLine("Running TestAssignTaskRules (Requirement 23)...");

            // Y - add an unassigned task
            taskService.AddTask(OWNER, BOARD_NAME, "Assignment Rules Task", "Desc", DateTime.Now.AddDays(4));
            int taskId = 4;

            // Y - MEMBER (a board member) assigns the unassigned task to themselves — should succeed
            Response firstAssign = ParseResponse(taskService.AssignTask(MEMBER, BOARD_NAME, 0, taskId, MEMBER));

            // Y - OUTSIDER (a board member, not the assignee) tries to reassign — should fail
            Response outsiderReassign = ParseResponse(taskService.AssignTask(OUTSIDER, BOARD_NAME, 0, taskId, OUTSIDER));

            // Y - MEMBER (current assignee) reassigns to OUTSIDER — should succeed
            Response assigneeReassign = ParseResponse(taskService.AssignTask(MEMBER, BOARD_NAME, 0, taskId, OUTSIDER));

            if (firstAssign.ErrorMessage == null && outsiderReassign.ErrorMessage != null && assigneeReassign.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Task assignment permission rules enforced correctly.");
            else
                Console.WriteLine($"-> FAILED: First assign: {firstAssign.ErrorMessage}, Outsider reassign: {outsiderReassign.ErrorMessage}, Assignee reassign: {assigneeReassign.ErrorMessage}");
        }

        // ─── GetBoardName ──────────────────────────────────────────────────────────

        /// <summary>
        /// A - Tests that GetBoardName returns the correct name for a given board ID.
        /// </summary>
        public void TestGetBoardName()
        {
            Console.WriteLine("Running TestGetBoardName...");

            Response res = ParseResponse(boardService.GetBoardName(boardId));

            if (res.ErrorMessage == null && res.ReturnValue != null &&
                res.ReturnValue.ToString()!.Contains(BOARD_NAME, StringComparison.OrdinalIgnoreCase))
                Console.WriteLine("-> PASSED: GetBoardName returned the correct board name.");
            else
                Console.WriteLine($"-> FAILED: Error: {res.ErrorMessage}, Value: {res.ReturnValue}");
        }

        // ─── GetUserBoards ─────────────────────────────────────────────────────────

        /// <summary>
        /// A - Tests that GetUserBoards returns a list containing the board the user is a member of.
        /// </summary>
        public void TestGetUserBoards()
        {
            Console.WriteLine("Running TestGetUserBoards...");

            Response res = ParseResponse(boardService.GetUserBoards(MEMBER));

            if (res.ErrorMessage != null)
            {
                Console.WriteLine($"-> FAILED: GetUserBoards returned error: {res.ErrorMessage}");
                return;
            }

            // A - verify the known boardId appears in MEMBER's board list
            var ids = res.ReturnValue != null
                ? JsonSerializer.Deserialize<int[]>(res.ReturnValue.ToString()!) : null;
            bool containsBoard = ids != null && Array.Exists(ids, id => id == boardId);

            if (containsBoard)
                Console.WriteLine("-> PASSED: GetUserBoards correctly returned the user's board ID.");
            else
                Console.WriteLine($"-> FAILED: Expected board ID {boardId} not found in list.");
        }

        // ─── LeaveBoard success ────────────────────────────────────────────────────

        /// <summary>
        /// A - Tests that a regular member (non-owner) can successfully leave a board.
        /// Covers Requirement 14 (positive case).
        /// </summary>
        public void TestLeaveBoardSuccess()
        {
            Console.WriteLine("Running TestLeaveBoardSuccess (Requirement 14, success case)...");

            // A - OUTSIDER is a member; they leave the board
            Response leaveRes = ParseResponse(boardService.LeaveBoard(OUTSIDER, boardId));

            // A - OUTSIDER rejoins so they don't affect later test state
            boardService.JoinBoard(OUTSIDER, boardId);

            if (leaveRes.ErrorMessage == null)
                Console.WriteLine("-> PASSED: Regular member successfully left the board.");
            else
                Console.WriteLine($"-> FAILED: LeaveBoard returned error: {leaveRes.ErrorMessage}");
        }

        // ─── Setup & RunAll ────────────────────────────────────────────────────────

        /// <summary>
        /// Runs all Milestone 2 membership and assignment acceptance tests.
        /// </summary>
        public void RunAll()
        {
            Console.WriteLine("=== STARTING MEMBERSHIP ACCEPTANCE TESTS ===");

            // Y - register three users: one owner, one member, one outsider
            userService.Register(OWNER, PASSWORD);
            userService.Register(MEMBER, PASSWORD);
            userService.Register(OUTSIDER, PASSWORD);

            // Y - OWNER creates the board; board is assigned a global ID internally
            boardService.CreateBoard(OWNER, BOARD_NAME);

            // Y - retrieve the board ID from the owner's board list so we can use JoinBoard/LeaveBoard
            string boardsJson = boardService.GetUserBoards(OWNER);
            Response boardsRes = ParseResponse(boardsJson);
            if (boardsRes.ErrorMessage == null && boardsRes.ReturnValue != null)
            {
                // Y - ReturnValue is a JSON array of integers; parse the first one
                var ids = JsonSerializer.Deserialize<int[]>(boardsRes.ReturnValue.ToString()!);
                boardId = ids != null && ids.Length > 0 ? ids[0] : -1;
            }

            // Y - MEMBER joins the board so they are a member for all subsequent tests
            boardService.JoinBoard(MEMBER, boardId);
            boardService.JoinBoard(OUTSIDER, boardId);

            TestOnlyOwnerCanDelete();

            // A - TestOnlyOwnerCanDelete deleted and recreated the board, so boardId is now stale.
            // Refresh it and re-add MEMBER and OUTSIDER to the new board before continuing.
            string freshBoardsJson = boardService.GetUserBoards(OWNER);
            Response freshBoardsRes = ParseResponse(freshBoardsJson);
            if (freshBoardsRes.ErrorMessage == null && freshBoardsRes.ReturnValue != null)
            {
                var freshIds = JsonSerializer.Deserialize<int[]>(freshBoardsRes.ReturnValue.ToString()!);
                boardId = freshIds != null && freshIds.Length > 0 ? freshIds[0] : -1;
            }
            // A - re-join MEMBER only; TestJoinBoard will handle re-joining OUTSIDER
            boardService.JoinBoard(MEMBER, boardId);

            TestJoinBoard();
            TestJoinNonExistentBoard();
            TestTransferOwnership();
            TestNonOwnerCannotTransfer();
            TestOwnerCannotLeave();
            TestLeaveUnassignsTasks();
            TestOnlyMemberCanAddTask();
            TestOnlyAssigneeCanAdvance();
            TestOnlyAssigneeOrOwnerCanEdit();
            TestInProgressTasksFiltersByAssignee();
            TestAssignTaskRules();
            TestGetBoardName();
            TestGetUserBoards();
            TestLeaveBoardSuccess();

            Console.WriteLine("=== FINISHED MEMBERSHIP ACCEPTANCE TESTS ===\n");
        }
    }
}
