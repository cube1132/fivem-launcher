using System;

namespace IdleTapGame.Economy
{
    /// <summary>
    /// A passive producer. Cost grows geometrically with the number owned.
    /// </summary>
    [Serializable]
    public class Generator
    {
        public string Id;
        public string DisplayName;
        public double BaseCost;
        public double CostMultiplier;
        public double BaseProduction; // gold per second per unit owned
        public int Owned;

        public Generator(string id, string displayName, double baseCost, double costMultiplier, double baseProduction)
        {
            Id = id;
            DisplayName = displayName;
            BaseCost = baseCost;
            CostMultiplier = costMultiplier;
            BaseProduction = baseProduction;
            Owned = 0;
        }

        public double NextCost => BaseCost * Math.Pow(CostMultiplier, Owned);

        public double ProductionPerSecond => BaseProduction * Owned;
    }
}
