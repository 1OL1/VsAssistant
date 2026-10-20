using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VsAssistant.Data.Services;

namespace VsAssistant.Locations
{
    /// <summary>
    /// Interaction logic for LocWindow.xaml
    /// </summary>
    public partial class LocWindow : Window
    {
        private LocViewModel ViewModel => (LocViewModel)this.Root.DataContext;

        internal LocWindow(LocationDataService dataService)
        {
            InitializeComponent();

            this.Root.DataContext = new LocViewModel(dataService);

            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        protected override void OnClosed(EventArgs e)
        {
            // The service outlives the window, so the view model must unsubscribe from it
            this.ViewModel.Dispose();

            base.OnClosed(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
