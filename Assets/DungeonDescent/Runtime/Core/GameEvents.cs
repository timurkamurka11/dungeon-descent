using System;
using UnityEngine;

namespace DungeonDescent.Core
{
    public static class GameEvents
    {
        public static event Action<int> FloorEntered;
        public static event Action<string> RoomCleared;
        public static event Action<int> EssenceChanged;
        public static event Action<int> GoldChanged;
        public static event Action<int, int> PotionsChanged;
        public static event Action<float, float> PlayerHealthChanged;
        public static event Action<float, float> PlayerStaminaChanged;
        public static event Action<string> InteractionPromptChanged;
        public static event Action<string, float, float> BossHealthChanged;
        public static event Action BossEnded;
        public static event Action RunEnded;

        public static void RaiseFloorEntered(int floor) => FloorEntered?.Invoke(floor);
        public static void RaiseRoomCleared(string id) => RoomCleared?.Invoke(id);
        public static void RaiseEssenceChanged(int value) => EssenceChanged?.Invoke(value);
        public static void RaiseGoldChanged(int value) => GoldChanged?.Invoke(value);
        public static void RaisePotionsChanged(int current, int max) => PotionsChanged?.Invoke(current, max);
        public static void RaisePlayerHealthChanged(float current, float max) => PlayerHealthChanged?.Invoke(current, max);
        public static void RaisePlayerStaminaChanged(float current, float max) => PlayerStaminaChanged?.Invoke(current, max);
        public static void RaiseInteractionPromptChanged(string text) => InteractionPromptChanged?.Invoke(text ?? string.Empty);
        public static void RaiseBossHealthChanged(string name, float current, float max) => BossHealthChanged?.Invoke(name, current, max);
        public static void RaiseBossEnded() => BossEnded?.Invoke();
        public static void RaiseRunEnded() => RunEnded?.Invoke();

        public static void ClearTransientUI()
        {
            InteractionPromptChanged?.Invoke(string.Empty);
            BossHealthChanged?.Invoke(string.Empty, 0f, 1f);
        }
    }
}
