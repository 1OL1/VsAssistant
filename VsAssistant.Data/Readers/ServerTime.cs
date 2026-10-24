using System.Globalization;

namespace VsAssistant.Data.Readers
{
    /// <summary>
    /// Converts game server (Kyiv) times to local time.
    /// </summary>
    public static class ServerTime
    {
        #region Constants
        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        #endregion

        #region Properties
        public static DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.Now, TimeZone));
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
