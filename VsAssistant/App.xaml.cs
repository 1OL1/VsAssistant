using Microsoft.Extensions.Logging;
using System.Windows;
using VsAssistant.Data.Services;
using VsAssistant.Helpers;
using VsAssistant.Locations;

namespace VsAssistant
{
    public partial class App : System.Windows.Application
    {
        #region Constants
        private const string trayText = "ВС Ассистент";
        #endregion

        #region Members
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<App> logger;
        private readonly LocationDataService locationDataService;

        private NotifyIcon? trayIcon;
        #endregion

        public App()
        {
            loggerFactory = LoggingHelper.CreateLoggerFactory();
            logger = loggerFactory.CreateLogger<App>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Shared by all forms that show location data
            locationDataService = new LocationDataService(loggerFactory.CreateLogger<LocationDataService>());
        }

        #region Overrides
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            logger.LogInformation("Application started, version {Version}", typeof(App).Assembly.GetName().Version);

            trayIcon = TrayHelper.CreateTrayIcon(trayText, Resource.vs,
                new ToolStripMenuItem("Локи", null, LocationsMenuItem_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Вьіход", null, ExitMenuItem_Click));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Without disposing, the icon stays in the tray until the mouse moves over it
            trayIcon?.Dispose();

            logger.LogInformation("Application exited with code {ExitCode}", e.ApplicationExitCode);

            // Flushes and closes the log file
            loggerFactory.Dispose();

            base.OnExit(e);
        }
        #endregion

        #region Event Handlers
        private void LocationsMenuItem_Click(object? sender, EventArgs e)
        {
            this.ShowSingleWindow(() => new LocWindow(locationDataService));
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            this.Shutdown();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception, terminating: {IsTerminating}", e.IsTerminating);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            logger.LogError(e.Exception, "Unobserved task exception");
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Shows a new window of the given type, or brings the already open one to front.
        /// </summary>
        private void ShowSingleWindow<TWindow>(Func<TWindow> createWindow) where TWindow : Window
        {
            var window = Windows.OfType<TWindow>().FirstOrDefault();
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
