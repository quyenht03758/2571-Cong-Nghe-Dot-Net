namespace BT03
{
    internal static class Functions
    {
        /// <summary>
        /// Converts hour–minute–second to total seconds.
        /// </summary>
        public static int TotalSeconds(int hour, int minute, int second)
        {
            if (hour < 0 || minute < 0 || second < 0)
                throw new ArgumentOutOfRangeException("Time parts must be non-negative.");
            if (minute >= 60 || second >= 60)
                throw new ArgumentOutOfRangeException("Minutes/Seconds must be in 0..59.");

            checked
            {
                return hour * 3600 + minute * 60 + second;
            }
        }

        /// <summary>
        /// Formats a (h,m,s) nicely, padding 2 digits for mm:ss.
        /// </summary>
        public static string FormatHms(int h, int m, int s) =>
            $"{h}h:{m:00}m:{s:00}s";
    }
}
