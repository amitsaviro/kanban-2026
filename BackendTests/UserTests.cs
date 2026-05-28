using System;
using System.Text.Json;
using Backend.ServiceLayer;

namespace BackendTests
{
    /// <summary>
    /// Provides a comprehensive suite of acceptance tests for the UserService functionality.
    /// Covers Requirement 1 (Registration), Requirement 2 (Password Rules), Requirement 3 (Email Rules),
    /// and Requirement 7 (Login/Logout).
    /// </summary>
    public class UserTests
    {
        private UserService userService = new UserService();

        /// <summary>
        /// Parses a raw JSON response string into a structured Response object.
        /// </summary>
        private Response ParseResponse(string json)
        {
            return JsonSerializer.Deserialize<Response>(json)!;
        }

        /// <summary>
        /// Tests valid user registration.
        /// Covers Requirement 1 and 6.
        /// </summary>
        public void TestValidRegistration()
        {
            Console.WriteLine("Running TestValidRegistration...");
            Response res = ParseResponse(userService.Register("valid_user1@test.com", "ValidPass123"));
            
            if (res.ErrorMessage == null) Console.WriteLine("-> PASSED: Valid user registered successfully.");
            else Console.WriteLine($"-> FAILED: Registration failed. Error: {res.ErrorMessage}");
        }

        /// <summary>
        /// Tests that duplicate emails are rejected.
        /// Covers Requirement 1.
        /// </summary>
        public void TestDuplicateRegistration()
        {
            Console.WriteLine("Running TestDuplicateRegistration (Edge Case)...");
            userService.Register("dup_user@test.com", "Password123");
            
            // Attempt to register with the exact same email (testing case insensitivity if needed)
            Response res = ParseResponse(userService.Register("DUP_user@test.com", "Password123"));
            
            if (res.ErrorMessage != null) Console.WriteLine("-> PASSED: Duplicate registration correctly blocked.");
            else Console.WriteLine("-> FAILED: System allowed registering the same email twice!");
        }

        /// <summary>
        /// Tests that invalid email formats are rejected.
        /// Covers Requirement 3.
        /// </summary>
        public void TestInvalidEmailFormat()
        {
            Console.WriteLine("Running TestInvalidEmailFormat (Edge Case)...");
            Response res1 = ParseResponse(userService.Register("invalidemail.com", "Password123"));
            Response res2 = ParseResponse(userService.Register("missing@domain", "Password123"));

            if (res1.ErrorMessage != null && res2.ErrorMessage != null) 
                Console.WriteLine("-> PASSED: Invalid emails correctly blocked.");
            else 
                Console.WriteLine("-> FAILED: System allowed invalid email formats!");
        }

        /// <summary>
        /// Tests that password complexity rules are enforced (Length, Upper, Lower, Number).
        /// Covers Requirement 2.
        /// </summary>
        public void TestInvalidPasswords()
        {
            Console.WriteLine("Running TestInvalidPasswords (Edge Cases)...");
            
            // 1. Too short (< 6)
            Response resShort = ParseResponse(userService.Register("pass1@test.com", "Ab1"));
            // 2. No Uppercase
            Response resNoUpper = ParseResponse(userService.Register("pass2@test.com", "nouppercase1"));
            // 3. No Lowercase
            Response resNoLower = ParseResponse(userService.Register("pass3@test.com", "NOLOWERCASE1"));
            // 4. No Digit
            Response resNoDigit = ParseResponse(userService.Register("pass4@test.com", "NoDigitsHere"));

            if (resShort.ErrorMessage != null && resNoUpper.ErrorMessage != null && 
                resNoLower.ErrorMessage != null && resNoDigit.ErrorMessage != null)
            {
                Console.WriteLine("-> PASSED: All password complexity rules enforced successfully.");
            }
            else
            {
                Console.WriteLine("-> FAILED: One or more invalid passwords were accepted!");
            }
        }

        /// <summary>
        /// Tests the login and logout flow, ensuring state is tracked.
        /// Covers Requirement 7.
        /// </summary>
        public void TestLoginLogoutFlow()
        {
            Console.WriteLine("Running TestLoginLogoutFlow...");
            string email = "flow_user@test.com";
            string pass = "FlowPass123";
            
            userService.Register(email, pass);
            // Registration logs the user in automatically. Let's log them out first.
            Response logoutRes1 = ParseResponse(userService.Logout(email));
            
            // Now try to log in
            Response loginRes = ParseResponse(userService.Login(email, pass));
            
            // Try to log in with wrong password
            Response badLoginRes = ParseResponse(userService.Login(email, "WrongPass123"));

            if (logoutRes1.ErrorMessage == null && loginRes.ErrorMessage == null && badLoginRes.ErrorMessage != null)
            {
                Console.WriteLine("-> PASSED: Login and Logout logic works as expected.");
            }
            else
            {
                Console.WriteLine($"-> FAILED: Flow error. Logout1: {logoutRes1.ErrorMessage}, Login: {loginRes.ErrorMessage}, BadLogin: {badLoginRes.ErrorMessage}");
            }
        }

        /// <summary>
        /// Runs all Acceptance Tests for User functionality.
        /// </summary>
        public void RunAll()
        {
            Console.WriteLine("=== STARTING USER ACCEPTANCE TESTS ===");
            TestValidRegistration();
            TestDuplicateRegistration();
            TestInvalidEmailFormat();
            TestInvalidPasswords();
            TestLoginLogoutFlow();
            Console.WriteLine("=== FINISHED USER ACCEPTANCE TESTS ===\n");
        }
    }
}