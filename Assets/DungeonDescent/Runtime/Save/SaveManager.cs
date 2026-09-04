using System;
using System.IO;
using UnityEngine;

namespace DungeonDescent.Save
{
    public static class SaveManager
    {
        public const string FileName = "dungeon_descent_save_v1.json";
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static SaveData Load()
        {
            try
            {
                if (!File.Exists(SavePath)) return SaveData.CreateDefault();
                var json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<SaveData>(json);
                if (data == null) return SaveData.CreateDefault();
                data.MigrateInPlace();
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DUNGEON DESCENT save recovery: {ex.Message}");
                TryBackupCorrupt();
                return SaveData.CreateDefault();
            }
        }

        public static void Save(SaveData data)
        {
            if (data == null) return;
            data.MigrateInPlace();
            var json = JsonUtility.ToJson(data, true);
            var temp = SavePath + ".tmp";
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath) ?? Application.persistentDataPath);
            File.WriteAllText(temp, json);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(temp, SavePath);
        }

        public static void Delete()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
        }

        private static void TryBackupCorrupt()
        {
            try
            {
                if (!File.Exists(SavePath)) return;
                var backup = SavePath + ".corrupt-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                File.Copy(SavePath, backup, true);
            }
            catch { }
        }
    }
}
