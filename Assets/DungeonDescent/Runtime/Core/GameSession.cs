using DungeonDescent.Save;
using UnityEngine;

namespace DungeonDescent.Core
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public SaveData Save { get; private set; }
        public int RunSeed { get; private set; }
        public bool RunActive { get; private set; }
        public int RunEssence { get; private set; }
        public int RunGold { get; private set; }
        public bool BossDefeatedThisRun { get; private set; }
        public int DisplayEssence => (Save?.Essence ?? 0) + RunEssence;
        public int DisplayGold => (Save?.Gold ?? 0) + RunGold;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject); Load();
        }
        public void Load(){Save=SaveManager.Load();RunEssence=0;RunGold=0;PublishCurrency();}
        public void NewGame(){Save=SaveData.CreateDefault();RunEssence=0;RunGold=0;SaveManager.Save(Save);PublishCurrency();}
        public void BeginRun(int? seed=null){RunSeed=seed??Random.Range(1000,int.MaxValue/2);RunActive=true;RunEssence=0;RunGold=0;BossDefeatedThisRun=false;PublishCurrency();}
        public void FinishRun(bool extracted)
        {
            if(extracted){Save.Essence=Mathf.Max(0,Save.Essence+RunEssence);Save.Gold=Mathf.Max(0,Save.Gold+RunGold);SaveManager.Save(Save);}
            RunEssence=0;RunGold=0;RunActive=false;BossDefeatedThisRun=false;PublishCurrency();GameEvents.RaiseRunEnded();
        }
        public void AddEssence(int amount){if(RunActive)RunEssence=Mathf.Max(0,RunEssence+amount);else Save.Essence=Mathf.Max(0,Save.Essence+amount);PublishCurrency();}
        public void AddGold(int amount){if(RunActive)RunGold=Mathf.Max(0,RunGold+amount);else Save.Gold=Mathf.Max(0,Save.Gold+amount);PublishCurrency();}
        public void MarkFloor(int floor){Save.DeepestFloor=Mathf.Max(Save.DeepestFloor,floor);GameEvents.RaiseFloorEntered(floor);}
        public void MarkBossDefeated(){Save.CryptWardenDefeated=true;BossDefeatedThisRun=true;Save.DeepestFloor=Mathf.Max(Save.DeepestFloor,3);}
        public void Persist()=>SaveManager.Save(Save);
        private void PublishCurrency(){if(Save==null)return;GameEvents.RaiseEssenceChanged(DisplayEssence);GameEvents.RaiseGoldChanged(DisplayGold);}
    }
}
