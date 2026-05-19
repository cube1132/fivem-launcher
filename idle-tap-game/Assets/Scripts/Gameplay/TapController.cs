using System;

namespace IdleTapGame.Gameplay
{
    /// <summary>
    /// Active income: gold gained per screen tap, with an upgradeable level.
    /// </summary>
    public class TapController
    {
        public int Level { get; private set; } = 1;

        public double TapPower => Level; // linear for the MVP

        public double UpgradeCost => 50d * Math.Pow(1.25d, Level - 1);

        public event Action TapPowerChanged;

        public void SetLevel(int level)
        {
            Level = level < 1 ? 1 : level;
            TapPowerChanged?.Invoke();
        }

        public void Upgrade()
        {
            Level++;
            TapPowerChanged?.Invoke();
        }
    }
}
