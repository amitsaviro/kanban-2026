using System;
using System.Collections.Generic;
using Backend.BusinessLayer;
using log4net;
using System.Text.RegularExpressions;

namespace Backend.Facades
{
    public class UserFacade
    {
        // Y - instance field so each GradingService / test suite gets its own isolated user store
        private Dictionary<string, User> _users = new Dictionary<string, User>();
        // Y - named constants replace magic numbers for password length (Requirement 2)
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 20;
        // The only static field allowed in the project, used strictly for logging purposes.
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Initializes a new instance of the UserFacade class.
        /// </summary>
        public UserFacade()
        {
        }
        

        /// <summary>
        /// Registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="password">The user password.</param>
        /// <remarks>
        /// Precondition: Email must be unique and valid. Password must meet complexity requirements (6-20 chars, uppercase, lowercase, number).
        /// Postcondition: A new user is created and registered in the system.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if email is invalid/exists, or password does not meet requirements.</exception>
        public void Register(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                log.Error("Registration failed: Provided email is null or empty.");
                throw new ArgumentException("Email cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                log.Error("Registration failed: Provided password is null or empty.");
                throw new ArgumentException("Password cannot be null or empty.");
            }

            // Normalize email to ensure case-insensitivity
            string cleanedEmail = email.Trim().ToLower();

            if (_users.ContainsKey(cleanedEmail))
            {
                log.Error($"Registration failed: Email '{email}' is already registered.");
                throw new ArgumentException($"The email '{email}' is already registered.");
            }

            if (!ValidateEmailFormat(cleanedEmail))
            {
                log.Error($"Registration failed: Email '{email}' has an invalid format.");
                throw new ArgumentException("The provided email format is invalid.");
            }

            try 
            {
                ValidatePasswordComplexity(password);
            }
            catch (ArgumentException ex)
            {
                log.Error($"Registration failed for {email}: {ex.Message}");
                throw; // Rethrow the exception after logging it
            }

            User newUser = new User(cleanedEmail, password);
            _users.Add(cleanedEmail, newUser);

            // Y - Requirement 6: user is automatically logged in right after registering, so they don't need to call Login again
            newUser.Login(password);

            log.Info($"User successfully registered with email: {cleanedEmail}");
        }


        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <remarks>
        /// Precondition: User exists and is not already logged in.
        /// Postcondition: User session is active.
        /// </remarks>
        /// <exception cref="Exception">Thrown if login credentials are incorrect or user is already logged in.</exception>
        public void Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                log.Error("Login failed: Provided email is null or empty.");
                throw new ArgumentException("Email cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                log.Error("Login failed: Provided password is null or empty.");
                throw new ArgumentException("Password cannot be null or empty.");
            }

            string cleanedEmail = email.Trim().ToLower();

            if (!_users.ContainsKey(cleanedEmail))//checking if this user exist
            {
                log.Error($"Login failed: The email '{email}' is not registered.");
                throw new ArgumentException("No user found with the provided email address.");
            }

            User userToLogin = _users[cleanedEmail];//take this user by email

            try
            {
                userToLogin.Login(password);//try to login
            }
            catch (Exception ex)
            {
                log.Error($"Login failed for user '{cleanedEmail}': {ex.Message}");
                throw;
            }

            log.Info($"User successfully logged in: {cleanedEmail}");
        }

        /// <summary>
        /// Retrieves a User object by email. Used internally by BoardFacade and TaskFacade.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The User object.</returns>
        /// <exception cref="ArgumentException">Thrown if no user with that email exists.</exception>
        public User GetUser(string email)
        {
            // Y - normalize email the same way Register does, so lookup is always consistent
            string cleanedEmail = email.Trim().ToLower();

            if (!_users.TryGetValue(cleanedEmail, out User user))
                throw new ArgumentException("No user found with the provided email address.");

            return user;
        }

        /// <summary>
        /// Retrieves a User and verifies they are currently logged in.
        /// Called by BoardFacade and TaskFacade before any board/task operation.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The logged-in User object.</returns>
        /// <exception cref="ArgumentException">Thrown if user does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if user exists but is not logged in.</exception>
        public User GetLoggedInUser(string email)
        {
            User user = GetUser(email);

            // Y - most operations require the user to be logged in; this check lives here so we don't repeat it everywhere
            if (!user.IsLoggedIn)
                throw new InvalidOperationException($"User '{email}' is not logged in.");

            return user;
        }

        /// <summary>
        /// Logs out a logged-in user.
        /// </summary>
        /// <param name="email">The email of the user to log out.</param>
        /// <remarks>
        /// Precondition: User is logged in.
        /// Postcondition: User session is terminated.
        /// </remarks>
        /// <exception cref="Exception">Thrown if user is not logged in.</exception>
        public void Logout(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                log.Error("Logout failed: Provided email is null or empty.");
                throw new ArgumentException("Email cannot be null or empty.");
            }

            string cleanedEmail = email.Trim().ToLower();

            if (!_users.ContainsKey(cleanedEmail))
            {
                log.Error($"Logout failed: The email '{email}' is not registered.");
                throw new ArgumentException("No user found with the provided email address.");
            }

            User userToLogout = _users[cleanedEmail];

            try
            {
                userToLogout.Logout();
            }
            catch (Exception ex)
            {
                log.Error($"Logout failed for user '{cleanedEmail}': {ex.Message}");
                throw;
            }

            log.Info($"User successfully logged out: {cleanedEmail}");
        }

        /// <summary>
        /// Checking if the password is valid
        /// Must be 6-20 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.
        /// </summary>
        private void ValidatePasswordComplexity(string password)
        {
            if (password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
            {
                throw new ArgumentException("Password length must be between 6 and 20 characters.");
            }

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) 
                {
                    hasUpper = true;
                }
                else if (char.IsLower(c)) 
                {
                    hasLower = true;
                }
                else if (char.IsDigit(c)) 
                {
                    hasDigit = true;
                }
            }

            if (!hasUpper || !hasLower || !hasDigit)
            {
                throw new ArgumentException("Password must contain at least one uppercase letter, one lowercase letter, and a numerical digit.");
            }
        }

        /// <summary>
        /// Validates the structure of the email address using a standard regular expression.
        /// </summary>
        private bool ValidateEmailFormat(string email)
        {
            //start with one char or more, @, more chars, dot and more chars - by regex
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"); 
            return regex.IsMatch(email);
        }

        public bool isLoggedIn(string email)
        {
            if (_users.ContainsKey(email)) { return _users[email].IsLoggedIn; }
            return false;
        }
    }
}