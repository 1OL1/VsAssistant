using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace VsAssistant.Helpers
{
    internal static class LoggingHelper
    {
        #region Constants
        private static readonly string logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VsAssistant", "Logs");
        #endregion

        #region Methods
        /// <summary>
        /// Creates a logger factory that writes to a daily rolling file in %LocalAppData%\VsAssistant\Logs.
        /// Disposing the factory flushes and closes the file.
        /// </summary>
        public static ILoggerFactory CreateLoggerFactory()
        {
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
        #endregion
    }
}
