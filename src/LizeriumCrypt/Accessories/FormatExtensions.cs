/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:00
 * Version: 1.0.1
 */

namespace LizeriumCrypt.Accessories
{
    public static class FormatExtensions
    {
        public static long ParseInt64(this string value)
        {
            long int64;

            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    int64 = 0L;
                }
                else
                {
                    long parsed = 0L;
                    int64 = long.TryParse(value, out parsed) ? parsed : 0L;
                }
            }
            catch
            {
                int64 = 0L;
            }

            return int64;
        }

        public static int ParseInt32(this string value)
        {
            int int32;

            try
            {
                int32 = int.TryParse(value, out var num) ? num : 0;
            }
            catch
            {
                int32 = 0;
            }

            return int32;
        }
    }
}
