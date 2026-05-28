using System;
using System.Text.Json;
using Backend.ServiceLayer;

namespace BackendTests
{
    /// <summary>
    /// Represents the standard JSON response structure returned by the Service Layer.
    /// </summary>
    public class Response
    {
        public string? ErrorMessage { get; set; }
        public object? ReturnValue { get; set; }
    }

    /// <summary>
    /// Provides a comprehensive suite of acceptance tests for the BoardService functionality,
    /// covering success cases, malformed inputs, and business logic edge cases.
    /// </summary>
    public class BoardTests
    {
        private UserService userService = new UserService();
        private BoardService boardService = new BoardService();
        private const string EMAIL = "yuval@test.com";
        private const string PASSWORD = "Password1";

        /// <summary>
        /// Parses a raw JSON response string into a structured Response object.
        /// </summary>
        /// <param name="json">The JSON string returned from a service method.</param>
        /// <returns>A non-null Response object containing the parsed fields.</returns>
        private Response ParseResponse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json)!;
        }

        /// <summary>
        /// Tests the successful creation of a board.
        /// Verifies that a valid user can create a board with a unique name.
        /// Covers Requirement 4 and Requirement 8.
        /// </summary>
        public void TestCreateBoard()
        {
            Console.WriteLine("Running TestCreateBoard (Success Case)...");

            string resultJson = boardService.CreateBoard(EMAIL, "WorkBoard");
            Response res = ParseResponse(resultJson);

            if (res.ErrorMessage == null)
            {
                Console.WriteLine("-> PASSED: Board created successfully without errors.");
            }
            else
            {
                Console.WriteLine($"-> FAILED: Expected success but got error: {res.ErrorMessage}");
            }
        }

        /// <summary>
        /// Tests that creating a board with a duplicate name for the same user is blocked.
        /// Verifies case-insensitivity of board names.
        /// Covers Requirement 10 and Requirement 20b.
        /// </summary>
        public void TestDuplicateBoard()
        {
            Console.WriteLine("Running TestDuplicateBoard (Edge Case)...");

            // Create the first board
            boardService.CreateBoard(EMAIL, "ProjectX");

            // Attempt to create a duplicate board with different casing
            string resultJson = boardService.CreateBoard(EMAIL, "projectx");
            Response res = ParseResponse(resultJson);

            if (res.ErrorMessage != null)
            {
                // Verify the error is actually about duplication, not a missing user
                if (res.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Test failed because the user context was missing, not because of duplication.");
                }
                else
                {
                    Console.WriteLine($"-> PASSED: Correctly blocked duplicate board name. Error caught: {res.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("-> FAILED: System allowed creation of a duplicate board name!");
            }
        }

        /// <summary>
        /// Tests that a board cannot be created with an invalid or empty name.
        /// Covers Requirement 8 and Requirement 20a (Malformed Input).
        /// </summary>
        public void TestCreateBoardWithInvalidName()
        {
            Console.WriteLine("Running TestCreateBoardWithInvalidName (Edge Case)...");

            string resultJson = boardService.CreateBoard(EMAIL, "   ");
            Response res = ParseResponse(resultJson);

            if (res.ErrorMessage != null)
            {
                if (res.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Test failed because the user context was missing, not because of name validation.");
                }
                else
                {
                    Console.WriteLine($"-> PASSED: Correctly blocked empty board name. Error caught: {res.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("-> FAILED: System allowed creating a board with a blank name!");
            }
        }

        /// <summary>
        /// Tests that board operations are blocked if the user is not logged in.
        /// Covers Requirement 7 and Requirement 20b (Logic Error).
        /// </summary>
        public void TestCreateBoardNotLoggedIn()
        {
            Console.WriteLine("Running TestCreateBoardNotLoggedIn (Edge Case)...");

            // Ensure the user is logged out
            userService.Logout(EMAIL);

            string resultJson = boardService.CreateBoard(EMAIL, "SecretBoard");
            Response res = ParseResponse(resultJson);

            // Log back in to preserve state for subsequent tests
            userService.Login(EMAIL, PASSWORD);

            if (res.ErrorMessage != null)
            {
                if (res.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Test failed because the user was not found at all, instead of throwing a login error.");
                }
                else
                {
                    Console.WriteLine($"-> PASSED: Correctly blocked action for a logged-out user. Error caught: {res.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("-> FAILED: System allowed board creation for a logged-out user!");
            }
        }

        /// <summary>
        /// Tests the successful deletion of a board and ensures subsequent access fails.
        /// Covers Requirement 8 and Requirement 20b.
        /// </summary>
        public void TestDeleteBoard()
        {
            Console.WriteLine("Running TestDeleteBoard (Success/Edge Case)...");

            boardService.CreateBoard(EMAIL, "TemporaryBoard");

            // First deletion should succeed
            string deleteJson = boardService.DeleteBoard(EMAIL, "TemporaryBoard");
            Response deleteRes = ParseResponse(deleteJson);

            // Second deletion of the same board should fail
            string deleteAgainJson = boardService.DeleteBoard(EMAIL, "TemporaryBoard");
            Response deleteAgainRes = ParseResponse(deleteAgainJson);

            if (deleteRes.ErrorMessage == null && deleteAgainRes.ErrorMessage != null)
            {
                if (deleteAgainRes.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Second deletion failed due to missing user context, not missing board.");
                }
                else
                {
                    Console.WriteLine("-> PASSED: Board deleted successfully, and duplicate deletion failed as expected.");
                }
            }
            else
            {
                Console.WriteLine($"-> FAILED: Deletion logic incorrect. First error: {deleteRes.ErrorMessage}, Second error: {deleteAgainRes.ErrorMessage}");
            }
        }

        /// <summary>
        /// Tests configuring column task limits and ensures validation rules are enforced.
        /// Covers Requirement 11, Requirement 12, and Requirement 20a.
        /// </summary>
        public void TestColumnLimits()
        {
            Console.WriteLine("Running TestColumnLimits (Success/Edge Case)...");

            string boardName = "LimitTestingBoard";
            boardService.CreateBoard(EMAIL, boardName);

            // Test 1: Verify default column limit via GetColumnLimit
            string defaultLimitJson = boardService.GetColumnLimit(EMAIL, boardName, 0);
            Response defaultLimitRes = ParseResponse(defaultLimitJson);
            
            // Test 2: Set a valid limit
            string validLimitJson = boardService.LimitColumn(EMAIL, boardName, 0, 5);
            Response validLimitRes = ParseResponse(validLimitJson);

            // Test 3: Attempt to set an invalid negative limit (below -1)
            string invalidLimitJson = boardService.LimitColumn(EMAIL, boardName, 0, -5);
            Response invalidLimitRes = ParseResponse(invalidLimitJson);

            if (defaultLimitRes.ErrorMessage == null && validLimitRes.ErrorMessage == null && invalidLimitRes.ErrorMessage != null)
            {
                if (invalidLimitRes.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Limit validation test failed due to missing user context.");
                }
                else
                {
                    Console.WriteLine("-> PASSED: Column limits initialized to default, updated successfully, and invalid inputs blocked.");
                }
            }
            else
            {
                Console.WriteLine($"-> FAILED: Column limit verification failed. Default error: {defaultLimitRes.ErrorMessage}, Valid limit error: {validLimitRes.ErrorMessage}, Invalid limit error: {invalidLimitRes.ErrorMessage}");
            }
        }

        /// <summary>
        /// Tests that providing out-of-bounds column ordinals fails gracefully.
        /// Covers Requirement 4 and Requirement 20a (Invalid Input Validation).
        /// </summary>
        public void TestInvalidOrdinal()
        {
            Console.WriteLine("Running TestInvalidOrdinal (Edge Case)...");

            string boardName = "OrdinalTestingBoard";
            boardService.CreateBoard(EMAIL, boardName);

            // Column ordinal 3 is invalid (only 0, 1, 2 are valid for Backlog, In Progress, Done)
            string resultJson = boardService.GetColumnName(EMAIL, boardName, 3);
            Response res = ParseResponse(resultJson);

            if (res.ErrorMessage != null)
            {
                if (res.ErrorMessage.Contains("No user found"))
                {
                    Console.WriteLine("-> FAILED: Test failed due to missing user context instead of ordinal validation.");
                }
                else
                {
                    Console.WriteLine($"-> PASSED: Out-of-bounds column ordinal blocked safely. Error caught: {res.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("-> FAILED: System allowed fetching information for an invalid column ordinal 3!");
            }
        }

        /// <summary>
        /// Executes all automated Board acceptance tests sequentially.
        /// Automatically synchronizes the user context before running the suite.
        /// </summary>
        public void RunAll()
        {
            Console.WriteLine("=== STARTING BOARD ACCEPTANCE TESTS ===");

            // Synchronize state: Register logs the user in automatically (Requirement 6).
            // Calling Login here is unnecessary and would fail with "User already logged in".
            userService.Register(EMAIL, PASSWORD);

            TestCreateBoard();
            TestDuplicateBoard();
            TestCreateBoardWithInvalidName();
            TestCreateBoardNotLoggedIn();
            TestDeleteBoard();
            TestColumnLimits();
            TestInvalidOrdinal();

            Console.WriteLine("=== FINISHED BOARD ACCEPTANCE TESTS ===\n");
        }
    }
}