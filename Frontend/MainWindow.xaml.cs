using System.Windows;
using Frontend.ViewModels;

namespace Frontend
{
    /// <summary>
    /// The application's single Window. Code-behind is intentionally minimal: it only
    /// wires up the DataContext it's given - all navigation and logic live in
    /// <see cref="MainWindowViewModel"/> and the screen-specific ViewModels it creates.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Creates the main window bound to the given ViewModel.
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel built by the composition root in App.xaml.cs.</param>
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
