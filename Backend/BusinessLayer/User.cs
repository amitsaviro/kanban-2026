using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a system user, managing their authentication status and personal boards.
    /// </summary>
    public class User
    {
        private string _email;
        private string _password;
        private bool _isLoggedIn;
        private Dictionary<string, Board> _boards;

        /// <summary>
        /// Authenticates the user with the provided password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <exception cref="InvalidOperationException">Thrown if user is already logged in or password mismatch.</exception>
        public void Login(string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the user out of the system.
        /// </summary>
        public void Logout()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new board to the user's collection.
        /// </summary>
        /// <param name="boardName">The unique name of the board to add.</param>
        /// <exception cref="ArgumentException">Thrown if board name already exists.</exception>
        public void AddBoard(string boardName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a board from the user's collection.
        /// </summary>
        /// <param name="boardName">The name of the board to remove.</param>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        public void RemoveBoard(string boardName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a board by its name.
        /// </summary>
        /// <param name="boardName">The name of the board.</param>
        /// <returns>The Board object.</returns>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        public Board GetBoard(string boardName)
        {
            throw new NotImplementedException();
        }
    }
}