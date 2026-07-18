using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Frontend.Infrastructure
{
    /// <summary>
    /// Base class for all ViewModels. Implements INotifyPropertyChanged so that
    /// WPF data bindings in the Views automatically refresh whenever a bound
    /// property's value changes.
    /// </summary>
    // A- every ViewModel in this project inherits from this class instead of implementing
    // A- INotifyPropertyChanged itself, so change notification is written once and reused everywhere
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised whenever a bound property changes, so the WPF binding engine
        /// can update the UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Options used whenever a subclass deserializes a service-layer response string.
        /// </summary>
        // A- was previously a separate static JsonOptions class; the project's rules say
        // A- "classes, class members, and methods should not be static, except for loggers", so
        // A- this is now a plain instance field every ViewModel gets for free by inheriting from here.
        protected readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the given property.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property that changed. Supplied automatically by the compiler
        /// via <see cref="CallerMemberNameAttribute"/> when called from within the property's setter.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets a backing field to a new value and raises <see cref="PropertyChanged"/>
        /// only if the value actually changed.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field.</param>
        /// <param name="value">The new value to assign.</param>
        /// <param name="propertyName">Name of the property being set (auto-filled by the compiler).</param>
        /// <returns>True if the value changed and the field was updated; false otherwise.</returns>
        // A- centralizing "if changed, assign + notify" here keeps every ViewModel property setter to one line
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
