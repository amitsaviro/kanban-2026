using System.Windows.Controls;
using Frontend.ViewModels;

namespace Frontend.Views
{
    /// <summary>
    /// Code-behind for AuthView.xaml. Deliberately empty aside from the PasswordBox
    /// forwarding below - all real logic lives in <see cref="AuthViewModel"/>.
    /// </summary>
    public partial class AuthView : UserControl
    {
        public AuthView()
        {
            InitializeComponent();
        }

        // A- the only code-behind logic in this View: WPF does not allow binding PasswordBox.Password
        // A- directly (to avoid the plaintext password being kept alive in the binding engine), so this
        // A- event is the standard way to copy it into the ViewModel. It contains no business logic -
        // A- it only forwards a value, exactly like a binding would if WPF permitted one here.
        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is AuthViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
