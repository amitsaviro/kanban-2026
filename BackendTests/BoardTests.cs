using System;
using Backend.ServiceLayer;

namespace BackendTests
{
    public class BoardTests
    {
        private UserService userService = new UserService();
        private BoardService boardService = new BoardService();

        /// <summary>
        /// Tests successful board creation.
        /// Requirement 8.
        /// </summary>
        public void TestCreateBoard()
        {
            Console.WriteLine("Running TestCreateBoard...");

            userService.Register("board@test.com", "Password1");

            string result = boardService.CreateBoard(
                "board@test.com",
                "Work"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests duplicate board names.
        /// Requirement 10.
        /// </summary>
        public void TestDuplicateBoard()
        {
            Console.WriteLine("Running TestDuplicateBoard...");

            userService.Register("dupboard@test.com", "Password1");

            boardService.CreateBoard(
                "dupboard@test.com",
                "Work"
            );

            string result = boardService.CreateBoard(
                "dupboard@test.com",
                "Work"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests deleting a board.
        /// Requirement 8.
        /// </summary>
        public void TestDeleteBoard()
        {
            Console.WriteLine("Running TestDeleteBoard...");

            userService.Register("delete@test.com", "Password1");

            boardService.CreateBoard(
                "delete@test.com",
                "School"
            );

            string result = boardService.DeleteBoard(
                "delete@test.com",
                "School"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests limiting a column.
        /// Requirement 11.
        /// </summary>
        public void TestLimitColumn()
        {
            Console.WriteLine("Running TestLimitColumn...");

            userService.Register("limit@test.com", "Password1");

            boardService.CreateBoard(
                "limit@test.com",
                "Project"
            );

            string result = boardService.LimitColumn(
                "limit@test.com",
                "Project",
                0,
                5
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests invalid column ordinal.
        /// </summary>
        public void TestInvalidOrdinal()
        {
            Console.WriteLine("Running TestInvalidOrdinal...");

            userService.Register("ordinal@test.com", "Password1");

            boardService.CreateBoard(
                "ordinal@test.com",
                "Project"
            );

            string result = boardService.GetColumnName(
                "ordinal@test.com",
                "Project",
                99
            );

            Console.WriteLine(result);
        }

        public void RunAll()
        {
            TestCreateBoard();
            TestDuplicateBoard();
            TestDeleteBoard();
            TestLimitColumn();
            TestInvalidOrdinal();
        }
    }
}