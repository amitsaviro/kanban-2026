using System;
using Backend.ServiceLayer;

namespace BackendTests
{
    public class UserTests
    {
        private UserService userService = new UserService();

        /// <summary>
        /// Tests successful registration.
        /// Requirement 6.
        /// </summary>
        public void TestRegisterSuccess()
        {
            Console.WriteLine("Running TestRegisterSuccess...");

            string result = userService.Register(
                "yuval@test.com",
                "Password1"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests duplicate registration.
        /// Requirement 3.
        /// </summary>
        public void TestDuplicateRegister()
        {
            Console.WriteLine("Running TestDuplicateRegister...");

            userService.Register("dup@test.com", "Password1");

            string result = userService.Register(
                "dup@test.com",
                "Password1"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests invalid password registration.
        /// Requirement 2.
        /// </summary>
        public void TestInvalidPassword()
        {
            Console.WriteLine("Running TestInvalidPassword...");

            string result = userService.Register(
                "bad@test.com",
                "abc"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests successful login.
        /// Requirement 7.
        /// </summary>
        public void TestLoginSuccess()
        {
            Console.WriteLine("Running TestLoginSuccess...");

            userService.Register("login@test.com", "Password1");

            string result = userService.Login(
                "login@test.com",
                "Password1"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests login with wrong password.
        /// Requirement 7.
        /// </summary>
        public void TestWrongPassword()
        {
            Console.WriteLine("Running TestWrongPassword...");

            userService.Register("wrong@test.com", "Password1");

            string result = userService.Login(
                "wrong@test.com",
                "Wrong123"
            );

            Console.WriteLine(result);
        }

        /// <summary>
        /// Tests logout.
        /// Requirement 7.
        /// </summary>
        public void TestLogout()
        {
            Console.WriteLine("Running TestLogout...");

            userService.Register("logout@test.com", "Password1");

            string result = userService.Logout(
                "logout@test.com"
            );

            Console.WriteLine(result);
        }

        public void RunAll()
        {
            TestRegisterSuccess();
            TestDuplicateRegister();
            TestInvalidPassword();
            TestLoginSuccess();
            TestWrongPassword();
            TestLogout();
        }
    }
}