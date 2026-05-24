using System;
using System.Collections.Generic;
using Backend.BusinessLayer;

namespace Backend.Facades
{
    public class UserFacade
    {
        private Dictionary<string, User> users;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool isLoggedIn(string email)
        {
            if (users.ContainsKey(email)) { return users[email].IsLoggedIn; }
            return false;
        }

        public User GetUser(string email)
        {
            if (users.ContainsKey(email)) { return users[email]; }
            return null;
        }
    }
}