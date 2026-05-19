using System;
using System.Collections.Generic;

namespace IdleTapGame.Save
{
    [Serializable]
    public class GeneratorSave
    {
        public string Id;
        public int Owned;
    }

    [Serializable]
    public class SaveData
    {
        public double Gold;
        public int TapLevel = 1;
        public List<GeneratorSave> Generators = new List<GeneratorSave>();
        public long LastSaveUnixSeconds;
    }
}
