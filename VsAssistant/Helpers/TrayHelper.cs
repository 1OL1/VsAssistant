using System.Windows;
using VsAssistant.Data.Services;
using VsAssistant.Locations;
using VsAssistant.Settings;

namespace VsAssistant.Helpers
{
    /// <summary>
    /// Owns the tray icon with its context menu and opens forms from it.
    /// </summary>
    internal class TrayHelper : IDisposable
    {
        #region Members
        private readonly LocationDataService locationDataService;
        private readonly SettingsService settingsService;

        private readonly ContextMenuStrip menu;
        private readonly NotifyIcon trayIcon;
        #endregion

        public TrayHelper(LocationDataService locationDataService, SettingsService settingsService)
        {
            this.locationDataService = locationDataService;
            this.settingsService = settingsService;

            menu = new ContextMenuStrip();
            menu.Items.Add("Локи", null, LocationsMenuItem_Click);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Вьіход", null, ExitMenuItem_Click);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "ВС Ассистент";
            trayIcon.Icon = Resource.vs;
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
        }

        #region Methods
        public void ShowLocations()
        {
            ShowSingleWindow(() => new LocWindow(locationDataService, settingsService));
        }

        public void Dispose()
        {
            // Without disposing, the icon stays in the tray until the mouse moves over it
            trayIcon.Dispose();
            menu.Dispose();
        }
        #endregion

        #region Event Handlers
        private void LocationsMenuItem_Click(object? sender, EventArgs e)
        {
            ShowLocations();
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Shows a new window of the given type, or brings the already open one to front.
        /// </summary>
        private static void ShowSingleWindow<TWindow>(Func<TWindow> createWindow) where TWindow : Window
        {
            var window = System.Windows.Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                createWindow().Show();
            }
            else
            {
                window.Activate();
            }
        }
        #endregion
    }
}
