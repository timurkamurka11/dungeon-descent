using DungeonDescent.Audio;
using DungeonDescent.CameraSystem;
using DungeonDescent.Combat;
using DungeonDescent.Player;
using DungeonDescent.UI;
using DungeonDescent.World;
using UnityEngine;

namespace DungeonDescent.Core
{
    public sealed class DungeonGameBootstrap:MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrap()
        {
            if(Object.FindFirstObjectByType<DungeonGameBootstrap>()!=null)return;new GameObject("DUNGEON DESCENT - Runtime Bootstrap").AddComponent<DungeonGameBootstrap>();
        }
        private void Start()
        {
            var session=Ensure<GameSession>("Game Session");var audio=Ensure<AudioManager>("Audio Manager");
            var worldGo=new GameObject("Dungeon World Manager");var world=worldGo.AddComponent<DungeonWorldBuilder>();world.BuildWorld();
            var player=BuildPlayer(session,world);BuildCamera(player);BuildUI(player);audio.SetMusic(MusicState.SafeRoom,.2f);audio.SetAmbience("fireplace",.5f);
        }
        private static T Ensure<T>(string name) where T:Component
        {var found=Object.FindFirstObjectByType<T>();if(found!=null)return found;return new GameObject(name).AddComponent<T>();}
        private static PlayerController BuildPlayer(GameSession session,DungeonWorldBuilder world)
        {
            var go=new GameObject("Player - The Delver");go.tag="Player";go.layer=6;go.transform.SetPositionAndRotation(world.SafeSpawn,world.SafeRotation);
            go.AddComponent<CharacterController>();var vitals=go.AddComponent<PlayerVitals>();vitals.Configure(session.Save.MaxHealth,session.Save.MaxStamina);
            var controller=go.AddComponent<PlayerController>();var anim=go.AddComponent<PlayerAnimationController>();var combat=go.AddComponent<PlayerCombat>();go.AddComponent<PlayerInteraction>();go.AddComponent<PlayerConsumables>();go.AddComponent<PlayerLockOn>();go.AddComponent<PlayerLifecycle>();
            var visual=PlayerModelFactory.Build(go.transform);anim.Configure(visual);controller.Configure(anim);combat.Configure(anim);return controller;
        }
        private static void BuildCamera(PlayerController player){var go=new GameObject("Third Person Camera Rig");go.AddComponent<ThirdPersonCameraRig>().Configure(player);}
        private static void BuildUI(PlayerController player){var go=new GameObject("Game UI Controller");go.AddComponent<GameUI>().Configure(player);}
        public static void SetLayerRecursively(GameObject root,int layer){root.layer=layer;foreach(Transform child in root.transform)SetLayerRecursively(child.gameObject,layer);}
    }
}
