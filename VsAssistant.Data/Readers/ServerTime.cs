using System.Globalization;

namespace VsAssistant.Data.Readers
{
    /// <summary>
    /// Converts game server (Kyiv) times to local time.
    /// </summary>
    internal static class ServerTime
    {
        #region Constants
        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        #endregion

        #region Methods
        public static DateTime? ParseToLocal(string? text)
        {
            if (DateTime.TryParseExact(text?.Trim(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var serverDate))
            {
                return TimeZoneInfo.ConvertTime(serverDate, TimeZone, TimeZoneInfo.Local);
            }

            return null;
        }
        #endregion
    }
}
