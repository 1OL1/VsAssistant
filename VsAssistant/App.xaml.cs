using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;
using System.Windows.Media;
using VsAssistant.Data.Services;
using VsAssistant.Locations;

namespace VsAssistant
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? trayIcon;
        private ILoggerFactory? loggerFactory;
        private ILogger<App>? logger;
        private LocationDataService? locationDataService;

        protected override void OnStartup(StartupEventArgs e)
        {
            loggerFactory = CreateLoggerFactory();
            logger = loggerFactory.CreateLogger<App>();
            logger.LogInformation("Application started, version {Version}", typeof(App).Assembly.GetName().Version);

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception, terminating: {IsTerminating}", e.IsTerminating);
            TaskScheduler.UnobservedTaskException += (s, e) =>
                logger.LogError(e.Exception, "Unobserved task exception");

            locationDataService = new LocationDataService(loggerFactory.CreateLogger<LocationDataService>());

            trayIcon = new NotifyIcon();
            trayIcon.Text = "ВС Ассистент";
            trayIcon.Icon = Resource.vs;
            trayIcon.Visible = true;
            trayIcon.ContextMenuStrip = CreateTrayMenu();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            logger?.LogInformation("Application exited with code {ExitCode}", e.ApplicationExitCode);

            // Flushes and closes the log file
            loggerFactory?.Dispose();

            base.OnExit(e);
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            var logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VsAssistant", "Logs");

            var serilogLogger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File(Path.Combine(logFolder, "VsAssistant-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            return LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));
        }

        private ContextMenuStrip CreateTrayMenu()
        {
            var menu = new ContextMenuStrip();

            var loc = new ToolStripMenuItem("Локи");
            loc.Click += (s, e) => 
            {
                if (!Windows.OfType<LocWindow>().Any())
                {
                    new LocWindow(locationDataService!).Show();
                }
            };
            menu.Items.Add(loc);

            menu.Items.Add(new ToolStripSeparator());

            var exit = new ToolStripMenuItem("Вьіход");
            exit.Click += (s, e) => Current.Shutdown();

            menu.Items.Add(exit);

            return menu;
        }
    }

}
