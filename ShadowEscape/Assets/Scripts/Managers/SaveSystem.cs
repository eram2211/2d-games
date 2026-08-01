using System.IO;
using UnityEngine;

namespace ShadowEscape.Managers
{
    /// <summary>
    /// Static utility for saving/loading SaveData as JSON in Application.persistentDataPath.
    /// Not attached to any GameObject -- called directly as SaveSystem.Save(...) / .Load().
    /// </summary>
    public static class SaveSystem
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "shadowescape_save.json");

        public static void Save(SaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (IOException e)
            {
                Debug.LogError($"SaveSystem: failed to save - {e.Message}");
            }
        }

        public static SaveData Load()
        {
            if (!File.Exists(SavePath)) return null;

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (IOException e)
            {
                Debug.LogError($"SaveSystem: failed to load - {e.Message}");
                return null;
            }
        }

        public static bool HasSave() => File.Exists(SavePath);

        public static void DeleteSave()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
        }
    }
}
