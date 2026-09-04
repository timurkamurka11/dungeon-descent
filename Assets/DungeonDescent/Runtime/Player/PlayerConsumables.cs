using DungeonDescent.Audio;
using DungeonDescent.Combat;
using DungeonDescent.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.Player
{
    [RequireComponent(typeof(PlayerVitals))]
    public sealed class PlayerConsumables : MonoBehaviour
    {
        private PlayerVitals vitals;private int current;public int Current=>current;public int Capacity=>GameSession.Instance?.Save.PotionCapacity??3;
        private void Awake(){vitals=GetComponent<PlayerVitals>();}
        private void Start(){Refill();}
        private void Update(){if(Keyboard.current!=null&&Keyboard.current.rKey.wasPressedThisFrame)UseHealingPotion();}
        public void Refill(){current=Capacity;GameEvents.RaisePotionsChanged(current,Capacity);}
        public bool UseHealingPotion(){if(current<=0||!vitals.IsAlive||vitals.CurrentHealth>=vitals.MaxHealth)return false;if(!vitals.Heal(58f))return false;current--;AudioManager.Instance?.PlaySfx("heal",1f);GameEvents.RaisePotionsChanged(current,Capacity);return true;}
    }
}
