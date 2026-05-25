using Backend.ServiceLayer;
using System;
using static System.Net.Mime.MediaTypeNames;

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

            string email = "yuval@test.com";
            string passward = "Password1";
            userService.Register(email, passward);
            userService.Login(email, passward);
            string result = boardService.CreateBoard(
                email,
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
            string email = "dupboard@test.com";
            string passward = "Password1";
            userService.Register(email,passward);
            userService.Login(email, passward);
            boardService.CreateBoard(
                email,
                "Work"
            );

            string result = boardService.CreateBoard(
                email,
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
            string email = "delete@test.com";
            string passward = "Password1";
            userService.Register(email,passward);
            userService.Login(email, passward);
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