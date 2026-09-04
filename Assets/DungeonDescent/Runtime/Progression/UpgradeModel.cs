using DungeonDescent.Save;

namespace DungeonDescent.Progression
{
    public enum UpgradeKind { MaxHealth, MaxStamina, PotionCapacity, WeaponDamage }

    public sealed class UpgradeModel
    {
        public int GetCost(UpgradeKind kind, SaveData data)
        {
            var level = GetLevel(kind, data);
            var baseCost = kind == UpgradeKind.PotionCapacity ? 180 : 100;
            return baseCost + level * (kind == UpgradeKind.PotionCapacity ? 140 : 80);
        }

        public bool TryPurchase(UpgradeKind kind, SaveData data)
        {
            if (data == null) return false;
            var cost = GetCost(kind, data);
            if (data.Essence < cost) return false;
            if (kind == UpgradeKind.PotionCapacity && data.PotionCapacity >= 7) return false;
            data.Essence -= cost;
            switch (kind)
            {
                case UpgradeKind.MaxHealth: data.HealthUpgradeLevel++; data.MaxHealth += 15f; break;
                case UpgradeKind.MaxStamina: data.StaminaUpgradeLevel++; data.MaxStamina += 10f; break;
                case UpgradeKind.PotionCapacity: data.PotionUpgradeLevel++; data.PotionCapacity += 1; break;
                case UpgradeKind.WeaponDamage: data.WeaponUpgradeLevel++; break;
            }
            return true;
        }

        private static int GetLevel(UpgradeKind kind, SaveData data)
        {
            switch (kind)
            {
                case UpgradeKind.MaxHealth: return data.HealthUpgradeLevel;
                case UpgradeKind.MaxStamina: return data.StaminaUpgradeLevel;
                case UpgradeKind.PotionCapacity: return data.PotionUpgradeLevel;
                case UpgradeKind.WeaponDamage: return data.WeaponUpgradeLevel;
                default: return 0;
            }
        }
    }
}
