using System;

namespace DungeonDescent.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentVersion = 1;
        public const float DefaultMaxHealth = 120f;
        public const float DefaultMaxStamina = 100f;

        public int Version = CurrentVersion;
        public float MaxHealth = DefaultMaxHealth;
        public float MaxStamina = DefaultMaxStamina;
        public int PotionCapacity = 3;
        public int Essence = 0;
        public int Gold = 0;
        public int DeepestFloor = 0;
        public bool CryptWardenDefeated = false;
        public int HealthUpgradeLevel = 0;
        public int StaminaUpgradeLevel = 0;
        public int PotionUpgradeLevel = 0;
        public int WeaponUpgradeLevel = 0;
        public float MasterVolume = 0.85f;
        public float MusicVolume = 0.65f;
        public float SfxVolume = 0.85f;
        public float CameraSensitivity = 1f;

        public static SaveData CreateDefault() => new SaveData();

        public void MigrateInPlace()
        {
            if (Version <= 0) Version = CurrentVersion;
            if (MaxHealth <= 0) MaxHealth = DefaultMaxHealth;
            if (MaxStamina <= 0) MaxStamina = DefaultMaxStamina;
            if (PotionCapacity <= 0) PotionCapacity = 3;
            Version = CurrentVersion;
        }
    }
}
