using System.Collections.Generic;

namespace IdleTapGame.Economy
{
    /// <summary>
    /// Default MVP content. Tune freely or later move to ScriptableObjects.
    /// </summary>
    public static class GeneratorCatalog
    {
        public static List<Generator> CreateDefault()
        {
            return new List<Generator>
            {
                new Generator("miner",   "Rudar",   15d,       1.15d, 0.2d),
                new Generator("farm",    "Farma",   120d,      1.15d, 1.0d),
                new Generator("factory", "Tovarna", 1300d,     1.15d, 8.0d),
                new Generator("bank",    "Banka",   14000d,    1.15d, 47.0d),
                new Generator("temple",  "Tempelj", 200000d,   1.15d, 260.0d),
                new Generator("castle",  "Grad",    3300000d,  1.15d, 1400.0d),
            };
        }
    }
}
