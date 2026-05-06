using System;

namespace Backend.ServiceLayer
{

    public class UserService
    {
        private UserFacade _userFacade; 

        /// <summary> Registers a new user to the system. </summary>
        /// <param name="email">Unique email address.</param>
        /// <param name="password">Must be 6-20 chars, with 1 uppercase, 1 lowercase, 1 number.</param>
        /// <returns>JSON string containing the registered user details or error message.</returns>
        /// <exception cref="ArgumentException">Thrown if email/password format is invalid or email is taken.</exception>
        public string Register(string email, string password) => throw new NotImplementedException();

        /// <summary> Logs in a user. </summary>
        /// <param name="email">Registered user email.</param>
        /// <param name="password">User password.</param>
        /// <returns>JSON string confirming login or error message.</returns>
        /// <exception cref="ArgumentException">Thrown if login credentials are incorrect.</exception>
        public string Login(string email, string password) => throw new NotImplementedException();

        /// <summary> Logs out a user. </summary>
        /// <param name="email">User email.</param>
        /// <returns>JSON string confirming logout.</returns>
        /// <exception cref="ArgumentException">Thrown if user is not logged in.</exception>
        public string Logout(string email) => throw new NotImplementedException();
    }
}