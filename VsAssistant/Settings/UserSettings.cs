namespace VsAssistant.Settings
{
    /// <summary>
    /// Everything the user can change, stored as JSON between runs.
    /// </summary>
    public class UserSettings
    {
        public LocWindowSettings LocWindow { get; set; } = new LocWindowSettings();
    }

    public class LocWindowSettings
    {
        /// <summary>
        /// Window position; null until the user moves the window.
        /// </summary>
        public double? Left { get; set; }
        public double? Top { get; set; }

        public double Opacity { get; set; } = 1;
    }
}
