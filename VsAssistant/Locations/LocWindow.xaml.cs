using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private static readonly int[] OpacityLevels = { 100, 80, 60, 40, 20 };

        // Kept across window instances, the tray creates a new window each time
        private static double savedOpacity = 1;

        private LocViewModel ViewModel => (LocViewModel)this.Root.DataContext;

        internal LocWindow(LocationDataService dataService)
        {
            InitializeComponent();

            this.Root.DataContext = new LocViewModel(dataService);

            foreach (var percent in OpacityLevels)
            {
                var item = new MenuItem { Header = $"{percent}%", Tag = percent / 100.0 };
                item.Click += OpacityMenuItem_Click;
                this.OpacityMenu.Items.Add(item);
            }
            SetOpacity(savedOpacity);

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

        private void OpacityButton_Click(object sender, RoutedEventArgs e)
        {
            this.OpacityMenu.PlacementTarget = this.OpacityButton;
            this.OpacityMenu.Placement = PlacementMode.Bottom;
            this.OpacityMenu.IsOpen = true;
        }

        private void OpacityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetOpacity((double)((MenuItem)sender).Tag);
        }

        private void SetOpacity(double opacity)
        {
            this.Opacity = savedOpacity = opacity;

            foreach (MenuItem item in this.OpacityMenu.Items)
            {
                item.IsChecked = (double)item.Tag == opacity;
            }
        }
    }
}
