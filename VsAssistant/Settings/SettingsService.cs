using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace VsAssistant.Settings
{
    /// <summary>
    /// Loads user settings from %LocalAppData%\VsAssistant\settings.json and saves them back.
    /// One instance is shared by the whole application.
    /// </summary>
    internal class SettingsService
    {
        #region Constants
        private static readonly string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VsAssistant", "settings.json");
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        #endregion

        #region Members
        private readonly ILogger<SettingsService> logger;
        #endregion

        #region Properties
        public UserSettings Current { get; }
        #endregion

        public SettingsService(ILogger<SettingsService> logger)
        {
            this.logger = logger;
            Current = this.Load();
        }

        #region Methods
        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile)!);

                // Written to a temp file first, so a failed write never leaves a broken settings file
                var tempFile = settingsFile + ".tmp";
                File.WriteAllText(tempFile, JsonSerializer.Serialize(Current, jsonOptions));
                File.Move(tempFile, settingsFile, overwrite: true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save settings to {File}", settingsFile);
            }
        }
        #endregion

        #region Implementation
        private UserSettings Load()
        {
            if (!File.Exists(settingsFile))
            {
                return new UserSettings();
            }

            try
            {
                return JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(settingsFile)) ?? new UserSettings();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to load settings from {File}, using defaults", settingsFile);
                return new UserSettings();
            }
        }
        #endregion
    }
}
