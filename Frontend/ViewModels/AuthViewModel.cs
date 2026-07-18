using System;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for the Registration/Login screen (Requirement 29.a).
    /// Talks only to <see cref="UserService"/> - never to GradingService or any lower layer directly.
    /// </summary>
    public class AuthViewModel : ViewModelBase
    {
        private readonly UserService _userService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        /// <summary>
        /// Raised after a successful login or registration, carrying the logged-in user's email.
        /// </summary>
        // A- MainWindowViewModel subscribes to this event to know when to switch CurrentViewModel
        // A- from this AuthViewModel to the BoardsListViewModel - this keeps navigation logic out
        // A- of the View (no code-behind), matching MVVM.
        public event Action<string> AuthSucceeded;

        /// <summary>
        /// Creates the AuthViewModel.
        /// </summary>
        /// <param name="userService">The shared, DB-connected UserService built by the composition root.</param>
        public AuthViewModel(UserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(_ => Login(), _ => CanSubmit());
            RegisterCommand = new RelayCommand(_ => Register(), _ => CanSubmit());
        }

        /// <summary>The email typed into the form.</summary>
        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        /// <summary>The password typed into the form.</summary>
        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        /// <summary>The last error message to display, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the "Login" button.</summary>
        public RelayCommand LoginCommand { get; }

        /// <summary>Command bound to the "Register" button.</summary>
        public RelayCommand RegisterCommand { get; }

        // A- both buttons are disabled while either field is empty, so the user gets immediate
        // A- feedback instead of a server round-trip that will obviously fail (Usability, Requirement 28).
        private bool CanSubmit() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>
        /// Attempts to log in with the current <see cref="Email"/>/<see cref="Password"/>.
        /// </summary>
        private void Login()
        {
            string json = _userService.Login(Email, Password);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            ErrorMessage = string.Empty;
            AuthSucceeded?.Invoke(Email);
        }

        /// <summary>
        /// Attempts to register a new user with the current <see cref="Email"/>/<see cref="Password"/>.
        /// </summary>
        // A- Requirement 6: a successful registration also logs the user in immediately (no separate
        // A- login step needed) - this is already true on the Backend side (UserFacade.Register calls
        // A- Login internally), so here we just treat a successful Register the same as a successful Login.
        private void Register()
        {
            string json = _userService.Register(Email, Password);
            var response = JsonSerializer.Deserialize<ServiceResponse<object>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            ErrorMessage = string.Empty;
            AuthSucceeded?.Invoke(Email);
        }
    }
}
