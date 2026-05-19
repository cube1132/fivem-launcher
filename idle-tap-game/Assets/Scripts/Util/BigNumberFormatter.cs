using System;
using System.Globalization;

namespace IdleTapGame.Util
{
    /// <summary>
    /// Formats large idle-game numbers into short notation
    /// (1.5K, 2.30M, 1.00B, 1.00T, then aa, ab, ac ...).
    /// </summary>
    public static class BigNumberFormatter
    {
        private static readonly string[] ShortSuffixes = { "", "K", "M", "B", "T" };

        public static string Format(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "0";

            bool negative = value < 0d;
            value = Math.Abs(value);

            if (value < 1000d)
            {
                string small = value < 100d
                    ? value.ToString("0.#", CultureInfo.InvariantCulture)
                    : Math.Floor(value).ToString("0", CultureInfo.InvariantCulture);
                return negative ? "-" + small : small;
            }

            int tier = (int)Math.Floor(Math.Log10(value) / 3d);
            double scaled = value / Math.Pow(1000d, tier);

            // Guard against rounding artefacts such as "1000.00K".
            if (scaled >= 1000d)
            {
                scaled /= 1000d;
                tier++;
            }

            string formatted = scaled.ToString("0.00", CultureInfo.InvariantCulture) + GetSuffix(tier);
            return negative ? "-" + formatted : formatted;
        }

        private static string GetSuffix(int tier)
        {
            if (tier >= 0 && tier < ShortSuffixes.Length)
                return ShortSuffixes[tier];

            // Beyond T: aa, ab, ac ... az, ba, bb ...
            int index = tier - ShortSuffixes.Length;
            int first = index / 26;
            int second = index % 26;
            return string.Concat((char)('a' + first), (char)('a' + second));
        }
    }
}
