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
            
            // Y - user starts as logged out; UserFacade.Register calls Login() right after creation to satisfy Requirement 6
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
        /// <exception cref="ArgumentException">Thrown if board name is empty or already exists.</exception>
        public void AddBoard(string boardName)
        {
            if (string.IsNullOrWhiteSpace(boardName))
                throw new ArgumentException("Board name cannot be empty.");

            // Y - ToLower() makes the key case-insensitive so "Work" and "work" are treated as the same board (Requirement 10)
            string key = boardName.Trim().ToLower();

            if (_boards.ContainsKey(key))
                throw new ArgumentException($"A board named '{boardName}' already exists.");

            // Y - create a new Board object and store it in the dictionary under the lowercase key
            _boards.Add(key, new Board(boardName));
        }

        /// <summary>
        /// Removes a board from the user's collection.
        /// </summary>
        /// <param name="boardName">The name of the board to remove.</param>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        public void RemoveBoard(string boardName)
        {
            // Y - same lowercase key as AddBoard so the lookup is also case-insensitive
            string key = boardName.Trim().ToLower();

            if (!_boards.ContainsKey(key))
                throw new ArgumentException($"Board '{boardName}' does not exist.");

            _boards.Remove(key);
        }

        /// <summary>
        /// Retrieves a board by its name.
        /// </summary>
        /// <param name="boardName">The name of the board.</param>
        /// <returns>The Board object.</returns>
        /// <exception cref="ArgumentException">Thrown if board does not exist.</exception>
        public Board GetBoard(string boardName)
        {
            string key = boardName.Trim().ToLower();

            // Y - TryGetValue is the safe way to look up a dictionary key in C#
            // it returns true/false and puts the value into the 'board' variable using the 'out' keyword
            // 'out' means: the method will fill this variable for you — it's an output parameter
            if (!_boards.TryGetValue(key, out Board board))
                throw new ArgumentException($"Board '{boardName}' does not exist.");

            return board;
        }

        /// <summary>
        /// Returns all boards belonging to this user.
        /// </summary>
        /// <returns>A collection of all the user's boards.</returns>
        public IEnumerable<Board> GetBoards()
        {
            // Y - IEnumerable<Board> is an interface meaning "something you can loop over"
            // returning _boards.Values gives the caller all boards without exposing the internal dictionary
            return _boards.Values;
        }
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
        }
    }
}