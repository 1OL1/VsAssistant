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
using VsAssistant.Settings;

namespace VsAssistant.Locations
{
    /// <summary>
    /// Interaction logic for LocWindow.xaml
    /// </summary>
    public partial class LocWindow : Window
    {
        private static readonly int[] OpacityLevels = { 100, 80, 60, 40, 20 };

        private readonly SettingsService settingsService;

        private LocViewModel ViewModel => (LocViewModel)this.Root.DataContext;
        private LocWindowSettings Settings => settingsService.Current.LocWindow;

        internal LocWindow(LocationDataService dataService, SettingsService settingsService)
        {
            InitializeComponent();

            this.settingsService = settingsService;
            this.Root.DataContext = new LocViewModel(dataService);

            foreach (var percent in OpacityLevels)
            {
                var item = new MenuItem { Header = $"{percent}%", Tag = percent / 100.0 };
                item.Click += OpacityMenuItem_Click;
                this.OpacityMenu.Items.Add(item);
            }
            ApplyOpacity(Settings.Opacity);

            RestorePosition();
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
                // Returns when the mouse button is released
                this.DragMove();

                Settings.Left = this.Left;
                Settings.Top = this.Top;
                settingsService.Save();
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
            ApplyOpacity((double)((MenuItem)sender).Tag);

            Settings.Opacity = this.Opacity;
            settingsService.Save();
        }

        private void ApplyOpacity(double opacity)
        {
            // A hand-edited settings file may hold a value outside the menu range
            this.Opacity = Math.Clamp(opacity, 0.2, 1);

            foreach (MenuItem item in this.OpacityMenu.Items)
            {
                item.IsChecked = (double)item.Tag == this.Opacity;
            }
        }

        private void RestorePosition()
        {
            var workArea = SystemParameters.WorkArea;
            this.Left = workArea.Right - this.Width;
            this.Top = workArea.Bottom - this.Height;

            if (Settings.Left is double left && Settings.Top is double top)
            {
                // The saved position is skipped if it is no longer fully on screen, e.g. after a monitor was disconnected
                var saved = new Rect(left, top, this.Width, this.Height);
                var screen = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
                if (screen.Contains(saved))
                {
                    this.Left = left;
                    this.Top = top;
                }
            }
        }
    }
}
