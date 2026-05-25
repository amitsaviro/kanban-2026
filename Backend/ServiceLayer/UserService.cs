using System;
using Backend.Facades;
using System.Text.Json; // The official C# library for JSON serialization

namespace Backend.ServiceLayer
{

    public class UserService
    {
        private UserFacade _userFacade; 

        /// <summary>
        /// Initializes a new instance of the UserService class and its underlying Facade.
        /// </summary>
        public UserService()
        {
            _userFacade = new UserFacade();
        }

        /// <summary> Registers a new user to the system. </summary>
        /// <param name="email">Unique email address.</param>
        /// <param name="password">Must be 6-20 chars, with 1 uppercase, 1 lowercase, 1 number.</param>
        /// <returns>An empty response, unless an error occurs</returns>
        /// <exception cref="ArgumentException">Thrown if email/password format is invalid or email is taken.</exception>
        public string Register(string email, string password)
        {
            try
            {
                // Try to register the user via the Facade
                _userFacade.Register(email, password);

                // On success, return an empty response (both fields are null)
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                // If the Facade threw an exception, catch it and return its message in the JSON
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary> Logs in a user. </summary>
        /// <param name="email">Registered user email.</param>
        /// <param name="password">User password.</param>
        /// <returns>JSON string confirming login or error message.</returns>
        /// <exception cref="ArgumentException">Thrown if login credentials are incorrect.</exception>
        public string Login(string email, string password)
        {
            try
            {
                _userFacade.Login(email, password);

                // According to GradingService requirements, successful login must return the email in ReturnValue
                var response = new { ErrorMessage = (string)null, ReturnValue = email };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary> Logs out a user. </summary>
        /// <param name="email">User email.</param>
        /// <returns>An empty response, unless an error occurs </returns>
        /// <exception cref="ArgumentException">Thrown if user is not logged in.</exception>
        public string Logout(string email)
        {
            try
            {
                _userFacade.Logout(email);

                // On success, return an empty response
                var response = new { ErrorMessage = (string)null, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                var response = new { ErrorMessage = ex.Message, ReturnValue = (object)null };
                return JsonSerializer.Serialize(response);
            }
        }    }
}