using System;
using System.Collections.Generic;

namespace Backend.BusinessLayer
{
    /// <summary>
    /// Represents a system user, managing their authentication status and personal boards.
    /// </summary>
    public class User
    {
        // Private fields protecting the internal state of the entity
        private string _email;
        private string _password;
        private bool _isLoggedIn;
        private Dictionary<string, Board> _boards;


        /// <summary>
        /// Initializes a new instance of the User class - ctor
        /// Precondition: Email and password must be validated by the Facade prior to instantiation.
        /// Postcondition: User is created and allocated an empty dictionary of boards.
        /// </summary>
        /// <param name="email">The validated, lowercase unique email address.</param>
        /// <param name="password">The validated password that meets complexity rules.</param>
        public User(string email, string password)
        {
            _email = email;
            _password = password;
            
            // The user is automatically logged in upon successful registration
            _isLoggedIn = false; 
            
            // New users have no boards by default. Dictionary prevents duplicate board names.
            _boards = new Dictionary<string, Board>(); 
        }


        /// <summary>
        /// Gets the unique email address of the user.
        /// </summary>
        public string Email
        {
            get
            {
                return _email;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is currently logged into the system.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
        }


        /// <summary>
        /// Authenticates the user with the provided password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <exception cref="InvalidOperationException">Thrown if user is already logged in or password mismatch.</exception>
        public void Login(string password)
        {
            if (_isLoggedIn)
            {
                throw new InvalidOperationException("User is already logged in.");
            }
            
            if (_password != password)
            {
                throw new InvalidOperationException("Password mismatch.");
            }

            _isLoggedIn = true;
        }

        /// <summary>
        /// Logs the user out of the system.
        /// </summary>
        public void Logout()
        {
            if (!_isLoggedIn)
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            _isLoggedIn = false;
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