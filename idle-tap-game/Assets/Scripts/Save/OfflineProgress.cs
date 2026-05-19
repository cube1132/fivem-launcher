using System;

namespace IdleTapGame.Save
{
    public struct OfflineResult
    {
        public bool HasEarnings;
        public double Earned;
        public double SecondsAway;
    }

    /// <summary>
    /// Grants passive income accumulated while the app was closed,
    /// capped at <see cref="MaxOfflineSeconds"/>.
    /// </summary>
    public static class OfflineProgress
    {
        public const double MaxOfflineSeconds = 8d * 3600d; // 8 hour cap

        public static OfflineResult Calculate(long lastSaveUnixSeconds, double productionPerSecond)
        {
            var result = new OfflineResult();

            if (lastSaveUnixSeconds <= 0 || productionPerSecond <= 0d)
                return result;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            double away = now - lastSaveUnixSeconds;
            if (away <= 1d)
                return result;

            away = Math.Min(away, MaxOfflineSeconds);
            result.SecondsAway = away;
            result.Earned = away * productionPerSecond;
            result.HasEarnings = result.Earned > 0d;
            return result;
        }
    }
}
