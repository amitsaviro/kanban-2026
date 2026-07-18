using System;
using System.Windows.Input;

namespace Frontend.Infrastructure
{
    /// <summary>
    /// A generic <see cref="ICommand"/> implementation that delegates its
    /// <see cref="Execute"/> and <see cref="CanExecute"/> logic to delegates
    /// supplied by the ViewModel. Lets Views bind buttons directly to
    /// ViewModel actions (e.g. <c>Command="{Binding LoginCommand}"</c>)
    /// instead of using code-behind Click handlers.
    /// </summary>
    // A- this is the standard MVVM "command" building block: every button/action in the
    // A- app exposes an ICommand property on its ViewModel built from this class, so the
    // A- View never calls Backend/Service code directly - it only raises a bound command
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Creates a new RelayCommand.
        /// </summary>
        /// <param name="execute">The action to run when the command is invoked.</param>
        /// <param name="canExecute">
        /// Optional predicate that determines whether the command can currently run
        /// (e.g. disables a "Login" button while the email/password fields are empty).
        /// If null, the command is always enabled.
        /// </param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Raised when the result of <see cref="CanExecute"/> may have changed,
        /// so WPF can re-query it (e.g. to enable/disable a bound button).
        /// </summary>
        // A- wired to CommandManager.RequerySuggested so bound buttons re-evaluate
        // A- CanExecute automatically on most UI events (typing, focus change, clicks)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Returns whether the command can currently execute.
        /// </summary>
        /// <param name="parameter">Optional command parameter from the binding.</param>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Runs the command's action.
        /// </summary>
        /// <param name="parameter">Optional command parameter from the binding.</param>
        public void Execute(object parameter) => _execute(parameter);
    }
}
