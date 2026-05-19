using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace IdleTapGame.Save
{
    /// <summary>
    /// JSON persistence to <see cref="Application.persistentDataPath"/>.
    /// Stamps the save time so offline progress can be computed on load.
    /// </summary>
    public static class SaveSystem
    {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static SaveData Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return null;

                string json = File.ReadAllText(FilePath);
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SaveSystem] Load failed: " + e.Message);
                return null;
            }
        }

        public static void Save(SaveData data)
        {
            try
            {
                data.LastSaveUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SaveSystem] Save failed: " + e.Message);
            }
        }
    }
}
