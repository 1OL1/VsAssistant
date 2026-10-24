using Microsoft.Extensions.Logging;
using System.Windows;
using VsAssistant.Data.Services;
using VsAssistant.Helpers;
using VsAssistant.Settings;

namespace VsAssistant
{
    public partial class App : System.Windows.Application
    {
        #region Constants
        private const string SingleInstanceMutexName = @"Local\VsAssistant.SingleInstance";
        #endregion

        #region Members
        private readonly Mutex singleInstanceMutex;
        private readonly bool isFirstInstance;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<App> logger;
        private readonly LocationDataService locationDataService;

        private TrayHelper? trayHelper;
        #endregion

        public App()
        {
            // Handled in OnStartup: a message box here would run startup too early
            singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out isFirstInstance);

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

            if (!isFirstInstance)
            {
                System.Windows.MessageBox.Show("ВС Ассистент уже запущен.", "ВС Ассистент", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            logger.LogInformation("Application started, version {Version}", typeof(App).Assembly.GetName().Version);

            var settingsService = new SettingsService(loggerFactory.CreateLogger<SettingsService>());

            trayHelper = new TrayHelper(locationDataService, settingsService);
            trayHelper.ShowLocations();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayHelper?.Dispose();

            if (isFirstInstance)
            {
                logger.LogInformation("Application exited with code {ExitCode}", e.ApplicationExitCode);
                singleInstanceMutex.ReleaseMutex();
            }

            // Flushes and closes the log file
            loggerFactory.Dispose();
            singleInstanceMutex.Dispose();

            base.OnExit(e);
        }
        #endregion

        #region Event Handlers
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception, terminating: {IsTerminating}", e.IsTerminating);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            logger.LogError(e.Exception, "Unobserved task exception");
        }
        #endregion
    }
}
