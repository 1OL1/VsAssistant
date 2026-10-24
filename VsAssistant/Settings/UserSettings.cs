namespace VsAssistant.Settings
{
    /// <summary>
    /// Everything the user can change, stored as JSON between runs.
    /// </summary>
    public class UserSettings
    {
        public WindowSettings LocWindow { get; set; } = new WindowSettings();
        public WindowSettings CastleWindow { get; set; } = new WindowSettings();
        public WindowSettings BloodLogWindow { get; set; } = new WindowSettings();
    }

    public class WindowSettings
    {
        /// <summary>
        /// Window position; null until the user moves the window.
        /// </summary>
        public double? Left { get; set; }
        public double? Top { get; set; }

        /// <summary>
        /// Window size; only kept for resizable windows, null until the user resizes it.
        /// </summary>
        public double? Width { get; set; }
        public double? Height { get; set; }

        public double Opacity { get; set; } = 1;
    }
}
