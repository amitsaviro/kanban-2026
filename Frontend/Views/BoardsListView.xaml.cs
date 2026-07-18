using System.Windows;
using System.Windows.Controls;
using Frontend.Models;
using Frontend.ViewModels;

namespace Frontend.Views
{
    /// <summary>
    /// Code-behind for BoardsListView.xaml. Deliberately empty aside from the
    /// double-click forwarding below - all real logic lives in <see cref="BoardsListViewModel"/>.
    /// </summary>
    public partial class BoardsListView : UserControl
    {
        public BoardsListView()
        {
            InitializeComponent();
        }

        // A- WPF's ListView has no built-in "row double-clicked" command/binding, so this handler
        // A- exists only to translate that UI event into a call to the already-bound OpenBoardCommand -
        // A- it contains no business logic itself, exactly like clicking a Button would.
        private void BoardRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is ListViewItem item &&
                item.DataContext is BoardModel board &&
                DataContext is BoardsListViewModel vm)
            {
                vm.OpenBoardCommand.Execute(board);
            }
        }
    }
}
