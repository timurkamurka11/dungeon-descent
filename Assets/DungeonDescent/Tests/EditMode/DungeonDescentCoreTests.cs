using NUnit.Framework;
using DungeonDescent.Save;
using DungeonDescent.Combat;
using DungeonDescent.Progression;
using DungeonDescent.UI;
using DungeonDescent.World;
using UnityEngine;

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
        public void Health_DamagePublishesChangedValue()
        {
            var health = new HealthModel(100f);
            var current = 100f;
            var maximum = 100f;
            health.Changed += (c, m) => { current = c; maximum = m; };
            health.ApplyDamage(25f);
            Assert.AreEqual(75f, current, 0.001f);
            Assert.AreEqual(100f, maximum, 0.001f);
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
        public void HudValue_NormalizesAndClamps()
        {
            Assert.AreEqual(.75f, HudValue.Normalized(75f, 100f), .0001f);
            Assert.AreEqual(1f, HudValue.Normalized(130f, 100f), .0001f);
            Assert.AreEqual(0f, HudValue.Normalized(-5f, 100f), .0001f);
            Assert.AreEqual(0f, HudValue.Normalized(50f, 0f), .0001f);
        }

        [Test]
        public void MeleeTargeting_AcceptsFrontTargetAndRejectsBehindOrTooFar()
        {
            var origin = Vector3.zero;
            var forward = Vector3.forward;
            Assert.IsTrue(MeleeTargeting.IsInsideMeleeArc(origin, forward, new Vector3(0f, 0f, 1.6f), 2.35f, .10f));
            Assert.IsFalse(MeleeTargeting.IsInsideMeleeArc(origin, forward, new Vector3(0f, 0f, -1.0f), 2.35f, .10f));
            Assert.IsFalse(MeleeTargeting.IsInsideMeleeArc(origin, forward, new Vector3(0f, 0f, 3.0f), 2.35f, .10f));
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
