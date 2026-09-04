using DungeonDescent.Audio;
using DungeonDescent.Core;
using DungeonDescent.Player;
using DungeonDescent.Progression;
using DungeonDescent.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DungeonDescent.UI
{
    public sealed class GameUI:MonoBehaviour
    {
        public static GameUI Instance{get;private set;}
        private Font font;private Canvas canvas;private GameObject mainMenu,pauseMenu,settingsMenu,controlsMenu,upgradeMenu,deathPanel,hud,bossPanel;
        private Image healthFill,staminaFill,bossFill;private Text essenceText,goldText,potionText,promptText,bossName,bossValue,upgradeStatus;
        private PlayerController player;private bool paused;private bool submenuFromMain;private int resolutionIndex;private int fpsIndex;private readonly int[] fpsOptions={60,90,120,144,165,240,-1};private UpgradeModel upgrades=new UpgradeModel();

        private static readonly Color Back=new Color(.028f,.033f,.04f,.94f);private static readonly Color Panel=new Color(.055f,.06f,.07f,.96f);private static readonly Color Border=new Color(.49f,.37f,.22f,.92f);private static readonly Color TextMain=new Color(.88f,.84f,.72f,1f);private static readonly Color Red=new Color(.55f,.07f,.055f,1f);private static readonly Color Green=new Color(.15f,.42f,.25f,1f);private static readonly Color Blue=new Color(.08f,.26f,.42f,1f);

        public void Configure(PlayerController owner)
        {
            Instance=this;player=owner;font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");CreateEventSystem();CreateCanvas();BuildHud();BuildMainMenu();BuildPause();BuildSettings();BuildControls();BuildUpgrades();BuildDeath();Subscribe();player.SetMovementLocked(true);mainMenu.SetActive(true);hud.SetActive(false);
        }
        private void Update()
        {
            if(Keyboard.current==null)return;if(Keyboard.current.escapeKey.wasPressedThisFrame){if(settingsMenu.activeSelf||controlsMenu.activeSelf||upgradeMenu.activeSelf){CloseSubmenus();return;}if(!mainMenu.activeSelf)TogglePause();}
        }
        private void OnDestroy(){Unsubscribe();if(Instance==this)Instance=null;}

        private void CreateEventSystem(){if(Object.FindFirstObjectByType<EventSystem>()!=null)return;var e=new GameObject("UI Event System");e.AddComponent<EventSystem>();e.AddComponent<InputSystemUIInputModule>();}
        private void CreateCanvas(){var go=new GameObject("Dungeon Descent UI");canvas=go.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=go.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;go.AddComponent<GraphicRaycaster>();}
        private RectTransform Rect(string name,Transform parent,Vector2 anchorMin,Vector2 anchorMax,Vector2 offsetMin,Vector2 offsetMax)
        {var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=anchorMin;r.anchorMax=anchorMax;r.offsetMin=offsetMin;r.offsetMax=offsetMax;return r;}
        private Image PanelImage(string name,Transform parent,Vector2 min,Vector2 max,Vector2 omin,Vector2 omax,Color color)
        {var r=Rect(name,parent,min,max,omin,omax);var img=r.gameObject.AddComponent<Image>();img.color=color;return img;}
        private Text Label(string name,Transform parent,string value,int size,TextAnchor align,Color color)
        {var r=Rect(name,parent,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);var t=r.gameObject.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;t.resizeTextForBestFit=false;return t;}
        private Button ButtonLine(string label,Transform parent,Vector2 pos,System.Action action,float width=360f)
        {var r=Rect(label+" Button",parent,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(-width*.5f+pos.x,pos.y-28),new Vector2(width*.5f+pos.x,pos.y+28));var img=r.gameObject.AddComponent<Image>();img.color=new Color(.12f,.115f,.10f,.96f);var outline=r.gameObject.AddComponent<Outline>();outline.effectColor=Border;outline.effectDistance=new Vector2(1.5f,-1.5f);var b=r.gameObject.AddComponent<Button>();b.targetGraphic=img;b.onClick.AddListener(()=>action());var colors=b.colors;colors.highlightedColor=new Color(.24f,.18f,.10f,1);colors.pressedColor=new Color(.36f,.25f,.10f,1);b.colors=colors;Label("Text",r,label,22,TextAnchor.MiddleCenter,TextMain);return b;}
        private void Ornament(Transform parent,Vector2 pos,float size)
        {var tex=Resources.Load<Texture2D>("Icons/dungeon_rune");if(tex==null)return;var r=Rect("Rune Ornament",parent,new Vector2(.5f,.5f),new Vector2(.5f,.5f),pos-new Vector2(size,size)*.5f,pos+new Vector2(size,size)*.5f);var img=r.gameObject.AddComponent<Image>();img.sprite=Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(.5f,.5f));img.color=new Color(.45f,.60f,.67f,.38f);img.raycastTarget=false;}
        private GameObject FullPanel(string name,float width,float height)
        {var overlay=PanelImage(name,canvas.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero,new Color(.006f,.008f,.012f,.78f)).gameObject;var box=PanelImage("Carved Panel",overlay.transform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(-width*.5f,-height*.5f),new Vector2(width*.5f,height*.5f),Panel);var outline=box.gameObject.AddComponent<Outline>();outline.effectColor=Border;outline.effectDistance=new Vector2(2,-2);Ornament(box.transform,new Vector2(0,height*.30f),125);return overlay;}

        private void BuildHud()
        {
            hud=Rect("HUD",canvas.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero).gameObject;
            var stats=PanelImage("Vitals Frame",hud.transform,new Vector2(0,1),new Vector2(0,1),new Vector2(32,-178),new Vector2(520,-28),new Color(.025f,.03f,.035f,.78f));stats.gameObject.AddComponent<Outline>().effectColor=Border;
            Label("Title",stats.transform,"WARDEN'S DESCENT",16,TextAnchor.UpperLeft,new Color(.68f,.62f,.48f,1));
            healthFill=Bar(stats.transform,new Vector2(18,-78),new Vector2(455,26),Red,"HEALTH");staminaFill=Bar(stats.transform,new Vector2(18,-118),new Vector2(455,20),Green,"STAMINA");
            essenceText=SmallHudText(stats.transform,new Vector2(18,-145),"ESSENCE  0");goldText=SmallHudText(stats.transform,new Vector2(190,-145),"GOLD  0");potionText=SmallHudText(stats.transform,new Vector2(330,-145),"POTIONS  3/3");
            var promptFrame=PanelImage("Interaction",hud.transform,new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(-310,32),new Vector2(310,90),new Color(.02f,.025f,.03f,.80f));promptFrame.gameObject.AddComponent<Outline>().effectColor=new Color(.35f,.32f,.24f,.75f);promptText=Label("Prompt",promptFrame.transform,"",20,TextAnchor.MiddleCenter,TextMain);
            bossPanel=PanelImage("Boss Frame",hud.transform,new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(-430,-130),new Vector2(430,-40),new Color(.025f,.02f,.024f,.88f)).gameObject;bossName=Label("Boss Name",bossPanel.transform,"THE CRYPT WARDEN",22,TextAnchor.UpperCenter,TextMain);bossFill=Bar(bossPanel.transform,new Vector2(28,-67),new Vector2(804,18),new Color(.45f,.035f,.035f,1),"");bossValue=Label("Boss Value",bossPanel.transform,"",14,TextAnchor.LowerCenter,new Color(.72f,.66f,.55f,1));bossPanel.SetActive(false);
        }
        private Image Bar(Transform parent,Vector2 pos,Vector2 size,Color fill,string label)
        {var bg=Rect(label+" Bar",parent,new Vector2(0,1),new Vector2(0,1),pos-new Vector2(0,size.y),pos+new Vector2(size.x,0));var bgi=bg.gameObject.AddComponent<Image>();bgi.color=new Color(.055f,.055f,.05f,1);var f=Rect("Fill",bg,Vector2.zero,Vector2.one,new Vector2(3,3),new Vector2(-3,-3));var img=f.gameObject.AddComponent<Image>();img.color=fill;img.type=Image.Type.Filled;img.fillMethod=Image.FillMethod.Horizontal;img.fillAmount=1f;if(!string.IsNullOrEmpty(label)){var l=Label("Caption",bg,label,13,TextAnchor.MiddleLeft,TextMain);l.rectTransform.offsetMin=new Vector2(8,0);}return img;}
        private Text SmallHudText(Transform parent,Vector2 pos,string value){var r=Rect(value,parent,new Vector2(0,1),new Vector2(0,1),pos-new Vector2(0,22),pos+new Vector2(160,0));var t=r.gameObject.AddComponent<Text>();t.font=font;t.fontSize=14;t.color=TextMain;t.alignment=TextAnchor.MiddleLeft;t.text=value;return t;}

        private void BuildMainMenu()
        {
            mainMenu=FullPanel("Main Menu",620,700);var box=mainMenu.transform.Find("Carved Panel");var title=Label("Title",box,"DUNGEON\nDESCENT",54,TextAnchor.UpperCenter,TextMain);title.rectTransform.offsetMin=new Vector2(0,440);title.rectTransform.offsetMax=new Vector2(0,-70);var sub=Label("Subtitle",box,"HOW FAR DOWN WILL YOU DESCEND THIS TIME?",14,TextAnchor.UpperCenter,new Color(.54f,.60f,.62f,1));sub.rectTransform.offsetMin=new Vector2(0,375);sub.rectTransform.offsetMax=new Vector2(0,-205);
            ButtonLine("CONTINUE",box,new Vector2(0,50),()=>StartGame(false));ButtonLine("NEW GAME",box,new Vector2(0,-25),()=>StartGame(true));ButtonLine("SETTINGS",box,new Vector2(0,-100),()=>{submenuFromMain=true;mainMenu.SetActive(false);settingsMenu.SetActive(true);});ButtonLine("QUIT",box,new Vector2(0,-175),()=>Application.Quit());
        }
        private void BuildPause()
        {
            pauseMenu=FullPanel("Pause Menu",560,610);var box=pauseMenu.transform.Find("Carved Panel");var title=Label("Title",box,"PAUSED",42,TextAnchor.UpperCenter,TextMain);title.rectTransform.offsetMin=new Vector2(0,360);title.rectTransform.offsetMax=new Vector2(0,-80);ButtonLine("RESUME",box,new Vector2(0,75),TogglePause);ButtonLine("SETTINGS",box,new Vector2(0,0),()=>{submenuFromMain=false;pauseMenu.SetActive(false);settingsMenu.SetActive(true);});ButtonLine("CONTROLS",box,new Vector2(0,-75),()=>{submenuFromMain=false;pauseMenu.SetActive(false);controlsMenu.SetActive(true);});ButtonLine("RETURN TO MAIN MENU",box,new Vector2(0,-150),ReturnMain);pauseMenu.SetActive(false);
        }
        private void BuildControls()
        {
            controlsMenu=FullPanel("Controls",700,660);var box=controlsMenu.transform.Find("Carved Panel");var t=Label("Controls Text",box,"CONTROLS\n\nWASD   Move\nSHIFT   Sprint\nSPACE   Dodge / Evade\nLMB   Light Attack Combo\nF   Heavy Attack\nRMB   Block\nQ   Lock-on / Release\nE   Interact\nR   Healing Potion\nESC   Pause",23,TextAnchor.MiddleCenter,TextMain);t.rectTransform.offsetMin=new Vector2(40,90);t.rectTransform.offsetMax=new Vector2(-40,-80);ButtonLine("BACK",box,new Vector2(0,-245),CloseSubmenus);controlsMenu.SetActive(false);
        }
        private void BuildSettings()
        {
            settingsMenu=FullPanel("Settings",760,820);var box=settingsMenu.transform.Find("Carved Panel");var title=Label("Title",box,"SETTINGS",38,TextAnchor.UpperCenter,TextMain);title.rectTransform.offsetMin=new Vector2(0,600);title.rectTransform.offsetMax=new Vector2(0,-60);
            ButtonLine("RESOLUTION",box,new Vector2(0,225),CycleResolution,480);ButtonLine("FULLSCREEN",box,new Vector2(0,160),()=>Screen.fullScreen=!Screen.fullScreen,480);ButtonLine("QUALITY",box,new Vector2(0,95),()=>QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel()+1)%QualitySettings.names.Length,true),480);ButtonLine("VSYNC",box,new Vector2(0,30),()=>QualitySettings.vSyncCount=QualitySettings.vSyncCount==0?1:0,480);ButtonLine("FPS LIMIT",box,new Vector2(0,-35),CycleFps,480);ButtonLine("SHADOW DISTANCE",box,new Vector2(0,-100),()=>QualitySettings.shadowDistance=QualitySettings.shadowDistance>65?45:90,480);ButtonLine("EFFECT DETAIL",box,new Vector2(0,-165),()=>QualitySettings.lodBias=QualitySettings.lodBias>1.5f?1.1f:2f,480);
            CreateSlider(box,"MASTER",new Vector2(0,-240),GameSession.Instance?.Save.MasterVolume??.85f,v=>{var s=GameSession.Instance.Save;s.MasterVolume=v;ApplyAudioSettings();});CreateSlider(box,"MUSIC",new Vector2(0,-292),GameSession.Instance?.Save.MusicVolume??.65f,v=>{GameSession.Instance.Save.MusicVolume=v;ApplyAudioSettings();});CreateSlider(box,"SFX",new Vector2(0,-344),GameSession.Instance?.Save.SfxVolume??.85f,v=>{GameSession.Instance.Save.SfxVolume=v;ApplyAudioSettings();});CreateSlider(box,"CAMERA",new Vector2(0,-396),GameSession.Instance?.Save.CameraSensitivity??1f,v=>GameSession.Instance.Save.CameraSensitivity=Mathf.Lerp(.35f,2.2f,v));ButtonLine("SAVE & BACK",box,new Vector2(0,-485),()=>{GameSession.Instance?.Persist();CloseSubmenus();},480);settingsMenu.SetActive(false);
        }
        private void CreateSlider(Transform parent,string label,Vector2 pos,float value,UnityEngine.Events.UnityAction<float> changed)
        {var l=Rect(label,parent,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(-245,pos.y-18),new Vector2(-110,pos.y+18));var lt=l.gameObject.AddComponent<Text>();lt.font=font;lt.fontSize=16;lt.alignment=TextAnchor.MiddleLeft;lt.color=TextMain;lt.text=label;var r=Rect(label+" Slider",parent,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(-95,pos.y-13),new Vector2(245,pos.y+13));var bg=r.gameObject.AddComponent<Image>();bg.color=new Color(.08f,.075f,.065f,1);var fill=Rect("Fill",r,Vector2.zero,Vector2.one,new Vector2(3,3),new Vector2(-3,-3));var fi=fill.gameObject.AddComponent<Image>();fi.color=Border;var s=r.gameObject.AddComponent<Slider>();s.fillRect=fill;s.targetGraphic=fi;s.value=Mathf.Clamp01(value);s.onValueChanged.AddListener(changed);}
        private void BuildUpgrades()
        {
            upgradeMenu=FullPanel("Permanent Upgrades",720,720);var box=upgradeMenu.transform.Find("Carved Panel");var title=Label("Title",box,"PERMANENT UPGRADES",34,TextAnchor.UpperCenter,TextMain);title.rectTransform.offsetMin=new Vector2(0,500);title.rectTransform.offsetMax=new Vector2(0,-65);ButtonLine("VITALITY  +15 MAX HEALTH",box,new Vector2(0,140),()=>Buy(UpgradeKind.MaxHealth),500);ButtonLine("ENDURANCE  +10 MAX STAMINA",box,new Vector2(0,60),()=>Buy(UpgradeKind.MaxStamina),500);ButtonLine("FLASK BELT  +1 POTION",box,new Vector2(0,-20),()=>Buy(UpgradeKind.PotionCapacity),500);ButtonLine("RUNIC EDGE  +5 WEAPON DAMAGE",box,new Vector2(0,-100),()=>Buy(UpgradeKind.WeaponDamage),500);var statusRect=Rect("Upgrade Status",box,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(-280,-210),new Vector2(280,-155));upgradeStatus=statusRect.gameObject.AddComponent<Text>();upgradeStatus.font=font;upgradeStatus.fontSize=18;upgradeStatus.alignment=TextAnchor.MiddleCenter;upgradeStatus.color=TextMain;ButtonLine("CLOSE",box,new Vector2(0,-260),CloseSubmenus,500);upgradeMenu.SetActive(false);
        }
        private void BuildDeath(){deathPanel=PanelImage("Death",canvas.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero,new Color(.09f,0,0,.82f)).gameObject;var t=Label("Death Text",deathPanel.transform,"YOU FELL INTO THE DEPTHS\n\nThe dungeon keeps what you failed to extract.",46,TextAnchor.MiddleCenter,new Color(.82f,.74f,.66f,1));deathPanel.SetActive(false);}

        private void Subscribe(){GameEvents.PlayerHealthChanged+=OnHealth;GameEvents.PlayerStaminaChanged+=OnStamina;GameEvents.EssenceChanged+=OnEssence;GameEvents.GoldChanged+=OnGold;GameEvents.PotionsChanged+=OnPotions;GameEvents.InteractionPromptChanged+=OnPrompt;GameEvents.BossHealthChanged+=OnBoss;GameEvents.BossEnded+=OnBossEnded;}
        private void Unsubscribe(){GameEvents.PlayerHealthChanged-=OnHealth;GameEvents.PlayerStaminaChanged-=OnStamina;GameEvents.EssenceChanged-=OnEssence;GameEvents.GoldChanged-=OnGold;GameEvents.PotionsChanged-=OnPotions;GameEvents.InteractionPromptChanged-=OnPrompt;GameEvents.BossHealthChanged-=OnBoss;GameEvents.BossEnded-=OnBossEnded;}
        private void OnHealth(float c,float m){if(healthFill!=null)healthFill.fillAmount=m>0?c/m:0;}
        private void OnStamina(float c,float m){if(staminaFill!=null)staminaFill.fillAmount=m>0?c/m:0;}
        private void OnEssence(int v){if(essenceText!=null)essenceText.text="ESSENCE  "+v;}
        private void OnGold(int v){if(goldText!=null)goldText.text="GOLD  "+v;}
        private void OnPotions(int c,int m){if(potionText!=null)potionText.text=$"POTIONS  {c}/{m}";}
        private void OnPrompt(string s){if(promptText!=null){promptText.text=s;promptText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(s));}}
        private void OnBoss(string n,float c,float m){if(bossPanel==null)return;var show=!string.IsNullOrEmpty(n)&&c>0;if(!bossPanel.activeSelf&&show)bossPanel.SetActive(true);bossName.text=n;bossFill.fillAmount=m>0?c/m:0;bossValue.text=$"{Mathf.CeilToInt(c)} / {Mathf.CeilToInt(m)}";}
        private void OnBossEnded(){if(bossPanel!=null)bossPanel.SetActive(false);}

        private void StartGame(bool fresh){if(fresh)GameSession.Instance?.NewGame();mainMenu.SetActive(false);hud.SetActive(true);Time.timeScale=1f;paused=false;player?.SetMovementLocked(false);AudioManager.Instance?.SetMusic(MusicState.SafeRoom,1.5f);AudioManager.Instance?.SetAmbience("fireplace",.5f);var consumables=player?.GetComponent<PlayerConsumables>();consumables?.Refill();}
        private void TogglePause(){paused=!paused;pauseMenu.SetActive(paused);hud.SetActive(!paused);Time.timeScale=paused?0f:1f;player?.SetMovementLocked(paused);}
        private void ReturnMain(){paused=false;Time.timeScale=1f;pauseMenu.SetActive(false);hud.SetActive(false);mainMenu.SetActive(true);player?.SetMovementLocked(true);DungeonDescent.World.DungeonWorldBuilder.Instance?.ReturnPlayerToSafeRoom(true);}
        private void CloseSubmenus(){settingsMenu.SetActive(false);controlsMenu.SetActive(false);upgradeMenu.SetActive(false);if(paused){pauseMenu.SetActive(true);return;}if(submenuFromMain){mainMenu.SetActive(true);submenuFromMain=false;return;}hud.SetActive(true);player?.SetMovementLocked(false);}
        public void OpenUpgradeMenu(){if(GameSession.Instance==null||GameSession.Instance.RunActive)return;submenuFromMain=false;upgradeMenu.SetActive(true);hud.SetActive(false);player?.SetMovementLocked(true);upgradeStatus.text=$"Available Essence: {GameSession.Instance.Save.Essence}";}
        private void Buy(UpgradeKind kind){var data=GameSession.Instance.Save;var cost=upgrades.GetCost(kind,data);if(upgrades.TryPurchase(kind,data)){upgradeStatus.text=$"Upgraded. Cost paid: {cost}. Essence: {data.Essence}";GameSession.Instance.Persist();GameEvents.RaiseEssenceChanged(data.Essence);var vitals=player.GetComponent<DungeonDescent.Combat.PlayerVitals>();vitals.Configure(data.MaxHealth,data.MaxStamina);player.GetComponent<PlayerConsumables>()?.Refill();}else upgradeStatus.text=$"Requires {cost} Essence.";}
        public void ShowDeath(){deathPanel.SetActive(true);hud.SetActive(false);}public void HideDeath(){deathPanel.SetActive(false);hud.SetActive(true);}
        private void ApplyAudioSettings(){var s=GameSession.Instance.Save;AudioManager.Instance?.ApplyVolumes(s.MasterVolume,s.MusicVolume,s.SfxVolume);}
        private void CycleResolution(){var r=Screen.resolutions;if(r==null||r.Length==0)return;resolutionIndex=(resolutionIndex+1)%r.Length;Screen.SetResolution(r[resolutionIndex].width,r[resolutionIndex].height,Screen.fullScreenMode);}
        private void CycleFps(){fpsIndex=(fpsIndex+1)%fpsOptions.Length;Application.targetFrameRate=fpsOptions[fpsIndex];}
    }
}
