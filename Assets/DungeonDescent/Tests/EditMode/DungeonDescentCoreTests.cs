using NUnit.Framework;
using DungeonDescent.Save;
using DungeonDescent.Combat;
using DungeonDescent.Progression;
using DungeonDescent.World;

namespace DungeonDescent.Tests.EditMode
{
    public sealed class DungeonDescentCoreTests
    {
        [Test]
        public void NewSave_HasVersionAndStarterResources()
        {
            var data = SaveData.CreateDefault();
            Assert.GreaterOrEqual(data.Version, 1);
            Assert.AreEqual(3, data.PotionCapacity);
            Assert.Greater(data.MaxHealth, 0);
            Assert.Greater(data.MaxStamina, 0);
        }

        [Test]
        public void Health_DamageClampsAndRaisesDeathOnce()
        {
            var health = new HealthModel(100f);
            var deaths = 0;
            health.Died += () => deaths++;
            health.ApplyDamage(70f);
            health.ApplyDamage(70f);
            health.ApplyDamage(10f);
            Assert.AreEqual(0f, health.Current, 0.001f);
            Assert.AreEqual(1, deaths);
        }

        [Test]
        public void Stamina_UsesCostAndRegeneratesAfterDelay()
        {
            var stamina = new StaminaModel(100f, 20f, 0.5f);
            Assert.IsTrue(stamina.TrySpend(30f));
            stamina.Tick(0.25f);
            Assert.AreEqual(70f, stamina.Current, 0.001f);
            stamina.Tick(0.5f);
            Assert.Greater(stamina.Current, 70f);
        }

        [Test]
        public void UpgradeModel_IncreasesPermanentStatsAndCost()
        {
            var data = SaveData.CreateDefault();
            data.Essence = 1000;
            var service = new UpgradeModel();
            var firstCost = service.GetCost(UpgradeKind.MaxHealth, data);
            Assert.IsTrue(service.TryPurchase(UpgradeKind.MaxHealth, data));
            Assert.Greater(data.MaxHealth, SaveData.DefaultMaxHealth);
            Assert.Greater(service.GetCost(UpgradeKind.MaxHealth, data), firstCost);
        }

        [Test]
        public void SeededRunLayout_IsDeterministicAndContainsRequiredStages()
        {
            var a = RunLayout.Generate(1337);
            var b = RunLayout.Generate(1337);
            CollectionAssert.AreEqual(a.RoomIds, b.RoomIds);
            CollectionAssert.Contains(a.RoomIds, "safe-room");
            CollectionAssert.Contains(a.RoomIds, "floor-1-old-catacombs");
            CollectionAssert.Contains(a.RoomIds, "floor-2-flooded-depths");
            CollectionAssert.Contains(a.RoomIds, "floor-3-forgotten-temple");
            CollectionAssert.Contains(a.RoomIds, "boss-crypt-warden");
        }
    }
}
