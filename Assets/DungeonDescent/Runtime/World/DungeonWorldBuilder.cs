using System.Collections;
using DungeonDescent.Audio;
using DungeonDescent.Boss;
using DungeonDescent.Core;
using DungeonDescent.Enemies;
using DungeonDescent.Interactables;
using DungeonDescent.Player;
using DungeonDescent.Presentation;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;

namespace DungeonDescent.World
{
    public sealed class DungeonWorldBuilder : MonoBehaviour
    {
        public static DungeonWorldBuilder Instance { get; private set; }
        private Transform contentRoot;
        public Transform EnemyRoot { get; private set; }
        private NavMeshSurface navSurface;
        public Vector3 SafeSpawn => new Vector3(0f,.06f,-6.2f);
        public Quaternion SafeRotation => Quaternion.LookRotation(Vector3.forward);

        private void Awake(){Instance=this;navSurface=GetComponent<NavMeshSurface>();if(navSurface==null)navSurface=gameObject.AddComponent<NavMeshSurface>();}
        public void BuildWorld()
        {
            if(contentRoot!=null)return;
            contentRoot=new GameObject("DUNGEON DESCENT - Authored World").transform;contentRoot.SetParent(transform,false);
            EnemyRoot=new GameObject("Runtime Enemies").transform;EnemyRoot.SetParent(contentRoot,false);
            BuildGlobalLighting();BuildSafeRoom();BuildDescent();BuildOldCatacombs();BuildFloodedDepths();BuildForgottenTemple();BuildBossArena();
            StartCoroutine(BakeNavigationNextFrame());
        }
        private IEnumerator BakeNavigationNextFrame(){yield return null;navSurface.BuildNavMesh();}

        private void BuildGlobalLighting()
        {
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.009f;RenderSettings.fogColor=new Color(.025f,.032f,.04f);RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.12f,.11f,.10f);RenderSettings.ambientEquatorColor=new Color(.055f,.065f,.07f);RenderSettings.ambientGroundColor=new Color(.018f,.02f,.022f);
            var go=new GameObject("Ancient World Moonlight");go.transform.SetParent(contentRoot,false);go.transform.rotation=Quaternion.Euler(52,-32,0);var l=go.AddComponent<Light>();l.type=LightType.Directional;l.intensity=.38f;l.color=new Color(.43f,.52f,.65f);l.shadows=LightShadows.Soft;
        }

        private GameObject Shape(string name,Vector3 pos,Vector3 scale,Material mat,Mesh mesh=null,bool collision=true,Transform parent=null)
        {
            var go=VisualFactory.Form(name,parent??contentRoot,mesh??ProceduralMeshFactory.Box,mat,pos,scale,Vector3.zero);go.layer=8;
            if(collision){var col=go.AddComponent<BoxCollider>();col.size=Vector3.one;}
            return go;
        }
        private GameObject ShapeRot(string name,Vector3 pos,Vector3 scale,Vector3 euler,Material mat,Mesh mesh=null,bool collision=true,Transform parent=null)
        {
            var go=VisualFactory.Form(name,parent??contentRoot,mesh??ProceduralMeshFactory.Box,mat,pos,scale,euler);go.layer=8;if(collision){var col=go.AddComponent<BoxCollider>();col.size=Vector3.one;}return go;
        }
        private void RoomShell(string name,Vector3 center,Vector3 size,Material material,float wallHeight=4.6f)
        {
            var root=new GameObject(name);root.transform.SetParent(contentRoot,false);
            Shape("Carved Stone Floor",center+Vector3.down*.35f,new Vector3(size.x,.7f,size.z),material,null,true,root.transform);
            Shape("Left Masonry Wall",center+new Vector3(-size.x*.5f,wallHeight*.5f,0),new Vector3(.65f,wallHeight,size.z),material,null,true,root.transform);
            Shape("Right Masonry Wall",center+new Vector3(size.x*.5f,wallHeight*.5f,0),new Vector3(.65f,wallHeight,size.z),material,null,true,root.transform);
            for(float z=-size.z*.42f;z<=size.z*.42f;z+=5.4f){Column(root.transform,center+new Vector3(-size.x*.5f+.28f,wallHeight*.47f,z),wallHeight*.9f,.42f);Column(root.transform,center+new Vector3(size.x*.5f-.28f,wallHeight*.47f,z),wallHeight*.9f,.42f);}
        }
        private void Column(Transform parent,Vector3 pos,float height,float radius)
        {
            var shaft=Shape("Ancient Column",pos,new Vector3(radius*2,height,radius*2),MaterialLibrary.Stone,ProceduralMeshFactory.Cylinder,true,parent);
            Shape("Column Capital",pos+Vector3.up*(height*.5f),new Vector3(radius*2.8f,.24f,radius*2.8f),MaterialLibrary.Stone,null,true,parent);
            Shape("Column Base",pos-Vector3.up*(height*.5f),new Vector3(radius*2.6f,.20f,radius*2.6f),MaterialLibrary.Stone,null,true,parent);
        }
        private void Arch(Transform parent,Vector3 center,float width,float height,Material mat)
        {
            Shape("Arch Left",center+new Vector3(-width*.5f,height*.42f,0),new Vector3(.58f,height*.84f,.72f),mat,null,true,parent);
            Shape("Arch Right",center+new Vector3(width*.5f,height*.42f,0),new Vector3(.58f,height*.84f,.72f),mat,null,true,parent);
            ShapeRot("Arch Crown L",center+new Vector3(-width*.25f,height*.86f,0),new Vector3(width*.56f,.50f,.72f),new Vector3(0,0,-13),mat,null,true,parent);
            ShapeRot("Arch Crown R",center+new Vector3(width*.25f,height*.86f,0),new Vector3(width*.56f,.50f,.72f),new Vector3(0,0,13),mat,null,true,parent);
        }
        private void Torch(Transform parent,Vector3 position,bool cold=false)
        {
            Shape("Forged Torch Bracket",position,new Vector3(.09f,.52f,.09f),MaterialLibrary.Metal,ProceduralMeshFactory.Cylinder,false,parent);
            var flame=Shape("Living Flame",position+Vector3.up*.42f,new Vector3(.18f,.42f,.18f),cold?MaterialLibrary.MagicBlue:MaterialLibrary.Fire,ProceduralMeshFactory.Cone,false,parent);
            var light=flame.AddComponent<Light>();light.type=LightType.Point;light.range=cold?5f:6.5f;light.intensity=cold?2.1f:2.5f;light.color=cold?new Color(.18f,.55f,1f):new Color(1f,.42f,.14f);light.shadows=LightShadows.Soft;
            var ps=flame.AddComponent<ParticleSystem>();var main=ps.main;main.startLifetime=.8f;main.startSpeed=.35f;main.startSize=.055f;main.maxParticles=26;main.startColor=light.color;var emission=ps.emission;emission.rateOverTime=12f;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Cone;shape.radius=.04f;shape.angle=12f;
        }
        private void DustVolume(Transform parent,Vector3 center,Vector3 scale,Color color)
        {
            var go=new GameObject("Ambient Floating Dust");go.transform.SetParent(parent,false);go.transform.position=center;var ps=go.AddComponent<ParticleSystem>();var main=ps.main;main.startLifetime=11f;main.startSpeed=.035f;main.startSize=.025f;main.maxParticles=160;main.startColor=color;var emission=ps.emission;emission.rateOverTime=10f;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=scale;
        }
        private void CreateZone(Vector3 center,Vector3 size,int floor,MusicState music,string ambience)
        {var go=new GameObject("Atmosphere Zone "+floor);go.transform.SetParent(contentRoot,false);go.transform.position=center;var c=go.AddComponent<BoxCollider>();c.isTrigger=true;c.size=size;go.AddComponent<ZoneTrigger>().Configure(floor,music,ambience);}

        private void BuildSafeRoom()
        {
            // Safe Room — warm permanent home above the dungeon.
            var root=new GameObject("Safe Room");root.transform.SetParent(contentRoot,false);
            Shape("Safe Room Flagstone",new Vector3(0,-.3f,0),new Vector3(12,.6f,20),MaterialLibrary.Stone,null,true,root.transform);
            Shape("Safe Left Wall",new Vector3(-6,2.6f,0),new Vector3(.75f,5.8f,20),MaterialLibrary.Stone,null,true,root.transform);
            Shape("Safe Right Wall",new Vector3(6,2.6f,0),new Vector3(.75f,5.8f,20),MaterialLibrary.Stone,null,true,root.transform);
            Shape("Safe Back Wall",new Vector3(0,2.6f,-10),new Vector3(12,5.8f,.75f),MaterialLibrary.Stone,null,true,root.transform);
            for(float z=-8;z<=8;z+=4){Shape("Oak Ceiling Beam",new Vector3(0,4.65f,z),new Vector3(11.4f,.28f,.42f),MaterialLibrary.Wood,null,true,root.transform);}
            for(float x=-4.5f;x<=4.5f;x+=3){Shape("Old Timber Upright",new Vector3(x,2.1f,-9.55f),new Vector3(.32f,4.2f,.35f),MaterialLibrary.Wood,null,true,root.transform);}
            // Fireplace, real light and ember particles.
            Shape("Hearth Base",new Vector3(-4.7f,.35f,-2f),new Vector3(1.8f,.7f,1.1f),MaterialLibrary.Stone,null,true,root.transform);
            Shape("Hearth Left",new Vector3(-5.45f,1.35f,-2.15f),new Vector3(.32f,2.2f,.75f),MaterialLibrary.Stone,null,true,root.transform);Shape("Hearth Right",new Vector3(-3.95f,1.35f,-2.15f),new Vector3(.32f,2.2f,.75f),MaterialLibrary.Stone,null,true,root.transform);Shape("Hearth Mantle",new Vector3(-4.7f,2.35f,-2.15f),new Vector3(1.85f,.34f,.75f),MaterialLibrary.Stone,null,true,root.transform);
            for(int i=0;i<4;i++)ShapeRot("Burning Log",new Vector3(-4.7f+(i-1.5f)*.18f,.43f,-1.9f),new Vector3(.15f,.85f,.15f),new Vector3(78,0,(i-1.5f)*13),MaterialLibrary.Wood,ProceduralMeshFactory.Cylinder,false,root.transform);
            var fire=Shape("Fire Core",new Vector3(-4.7f,.82f,-1.85f),new Vector3(.50f,.95f,.50f),MaterialLibrary.Fire,ProceduralMeshFactory.Cone,false,root.transform);var fLight=fire.AddComponent<Light>();fLight.type=LightType.Point;fLight.range=8.2f;fLight.intensity=3.4f;fLight.color=new Color(1f,.37f,.12f);fLight.shadows=LightShadows.Soft;
            DustVolume(root.transform,new Vector3(0,2f,-1),new Vector3(10,4,15),new Color(.9f,.65f,.35f,.15f));
            // Upgrade table with physical props.
            var table=Shape("Upgrade Workbench",new Vector3(3.85f,.78f,-3.4f),new Vector3(3f,.22f,1.25f),MaterialLibrary.Wood,null,true,root.transform);for(int s=-1;s<=1;s+=2)for(int z=-1;z<=1;z+=2)Shape("Workbench Leg",new Vector3(3.85f+s*1.15f,.37f,-3.4f+z*.42f),new Vector3(.22f,.75f,.22f),MaterialLibrary.Wood,null,true,root.transform);table.AddComponent<UpgradeTableInteractable>();
            ShapeRot("Forging Hammer",new Vector3(3.4f,1.02f,-3.35f),new Vector3(.12f,.85f,.12f),new Vector3(0,0,67),MaterialLibrary.Wood,ProceduralMeshFactory.Cylinder,false,root.transform);Shape("Hammer Head",new Vector3(3.05f,1.15f,-3.35f),new Vector3(.62f,.24f,.24f),MaterialLibrary.Metal,null,false,root.transform);
            // Rest point and storage.
            var rest=Shape("Rest Bench",new Vector3(-3.8f,.45f,5.3f),new Vector3(3.2f,.48f,1.1f),MaterialLibrary.Wood,null,true,root.transform);rest.AddComponent<RestPointInteractable>();Shape("Rest Cushion",new Vector3(-3.8f,.74f,5.3f),new Vector3(2.8f,.22f,.85f),MaterialLibrary.Cloth,null,false,root.transform);
            BuildChest(root.transform,new Vector3(3.8f,.55f,4.7f),0,0,false);
            Torch(root.transform,new Vector3(-5.55f,2.15f,4.2f));Torch(root.transform,new Vector3(5.55f,2.15f,4.2f));
            BuildDungeonDoor(root.transform);
            CreateZone(new Vector3(0,2,0),new Vector3(12,6,20),0,MusicState.SafeRoom,"fireplace");
        }
        private void BuildDungeonDoor(Transform root)
        {
            Arch(root,new Vector3(0,0,9.45f),4.4f,5f,MaterialLibrary.Stone);
            var left=Shape("Dungeon Gate Left",new Vector3(-1.05f,2.1f,9.4f),new Vector3(2.05f,4.25f,.28f),MaterialLibrary.Black,null,true,root);var right=Shape("Dungeon Gate Right",new Vector3(1.05f,2.1f,9.4f),new Vector3(2.05f,4.25f,.28f),MaterialLibrary.Black,null,true,root);
            left.transform.SetParent(root,true);right.transform.SetParent(root,true);Shape("Gate Rune",new Vector3(0,2.25f,9.12f),new Vector3(.45f,.92f,.06f),MaterialLibrary.MagicBlue,ProceduralMeshFactory.Sphere,false,root);
            var controller=new GameObject("Dungeon Door Interaction");controller.transform.SetParent(root,false);controller.transform.position=new Vector3(0,1.2f,8.7f);var c=controller.AddComponent<BoxCollider>();c.isTrigger=true;c.size=new Vector3(4.5f,2.4f,2f);controller.AddComponent<DungeonDoor>().Configure(left.transform,right.transform);
        }

        private void BuildDescent()
        {
            // Descent — physical stair sequence, no loading teleport.
            var root=new GameObject("Descent");root.transform.SetParent(contentRoot,false);
            var startZ=10.7f;const int steps=50;for(int i=0;i<steps;i++){float z=startZ+i*1.18f;float y=-i*.235f;Shape("Worn Stair "+i,new Vector3(0,y-.18f,z),new Vector3(5.4f,.42f,1.25f),MaterialLibrary.Stone,null,true,root.transform);if(i%5==0){Shape("Descent Left Wall",new Vector3(-3.0f,y+2.1f,z+2.2f),new Vector3(.65f,5.2f,6.2f),MaterialLibrary.Stone,null,true,root.transform);Shape("Descent Right Wall",new Vector3(3.0f,y+2.1f,z+2.2f),new Vector3(.65f,5.2f,6.2f),MaterialLibrary.Stone,null,true,root.transform);}if(i%9==4)Torch(root.transform,new Vector3(i%18<9?-2.62f:2.62f,y+1.9f,z),i>33);if(i%7==2)ShapeRot("Hanging Chain",new Vector3(-2.1f+(i%3)*2.1f,y+3.2f,z),new Vector3(.07f,1.8f,.07f),new Vector3(0,0,(i%2==0?3:-4)),MaterialLibrary.Metal,ProceduralMeshFactory.Cylinder,false,root.transform);}
            DustVolume(root.transform,new Vector3(0,-5.8f,39),new Vector3(5,10,50),new Color(.48f,.58f,.66f,.12f));
        }

        private void BuildOldCatacombs()
        {
            // Old Catacombs — first combat floor.
            var root=new GameObject("Floor 1 - Old Catacombs");root.transform.SetParent(contentRoot,false);var y=-11.72f;
            RoomShell("Old Catacombs Hall",new Vector3(0,y,82),new Vector3(13,1,28),MaterialLibrary.Stone,5f);RoomShell("Catacomb Mini Arena",new Vector3(0,y,106),new Vector3(15,1,20),MaterialLibrary.Stone,5.4f);Arch(root.transform,new Vector3(0,y,69.5f),4.2f,4.7f,MaterialLibrary.Stone);Arch(root.transform,new Vector3(0,y,94f),4.2f,4.7f,MaterialLibrary.Stone);
            for(int i=0;i<7;i++){var side=i%2==0?-1:1;Shape("Sarcophagus",new Vector3(side*4.25f,y+.42f,73+i*3.1f),new Vector3(1.6f,.78f,2.55f),MaterialLibrary.Stone,null,true,root.transform);Shape("Sarcophagus Lid",new Vector3(side*4.25f,y+.86f,73+i*3.1f),new Vector3(1.72f,.18f,2.68f),MaterialLibrary.Moss,null,false,root.transform);}
            Torch(root.transform,new Vector3(-5.7f,y+2f,76));Torch(root.transform,new Vector3(5.7f,y+2f,86));
            BuildEncounter("catacombs-entry",new Vector3(0,y+.8f,77),new Vector3(10,3,10),new[]{EnemyArchetype.GraveRat,EnemyArchetype.GraveRat,EnemyArchetype.HollowSkeleton},new[]{new Vector3(-2,0,2),new Vector3(2,0,3),new Vector3(0,0,5)},null);
            var gate=BuildGate(new Vector3(0,y+1.65f,96.2f),root.transform);gate.SetActive(false);
            BuildEncounter("catacombs-mini-arena",new Vector3(0,y+.8f,101),new Vector3(11,3,10),new[]{EnemyArchetype.HollowSkeleton,EnemyArchetype.CryptCrawler,EnemyArchetype.HollowSkeleton},new[]{new Vector3(-3,0,2),new Vector3(0,0,4),new Vector3(3,0,2)},gate);
            BuildChest(root.transform,new Vector3(0,y+.55f,110),30,25,true);CreateZone(new Vector3(0,y+2,82),new Vector3(13,6,32),1,MusicState.Exploration,"dungeon_wind");
            BuildTransitionStairs(root.transform,new Vector3(0,y,116),24,1.1f,.26f);
        }

        private void BuildFloodedDepths()
        {
            // Flooded Depths — wet stone, channels, cold light and limited visibility.
            var root=new GameObject("Floor 2 - Flooded Depths");root.transform.SetParent(contentRoot,false);var y=-17.96f;
            RoomShell("Flooded Gallery",new Vector3(0,y,153),new Vector3(15,1,34),MaterialLibrary.Stone,5.4f);RoomShell("Flooded Elite Vault",new Vector3(0,y,178),new Vector3(16,1,18),MaterialLibrary.Stone,5.8f);
            Shape("Left Water Channel",new Vector3(-4.4f,y+.02f,153),new Vector3(4.4f,.13f,31),MaterialLibrary.Water,null,false,root.transform);Shape("Right Water Channel",new Vector3(4.4f,y+.02f,153),new Vector3(4.4f,.13f,31),MaterialLibrary.Water,null,false,root.transform);Shape("Raised Causeway",new Vector3(0,y+.12f,153),new Vector3(3.6f,.42f,34),MaterialLibrary.Stone,null,true,root.transform);
            for(int i=0;i<6;i++){ShapeRot("Broken Drain Pipe",new Vector3(i%2==0?-6.7f:6.7f,y+1.4f,140+i*5.2f),new Vector3(.55f,2.4f,.55f),new Vector3(90,0,0),MaterialLibrary.Metal,ProceduralMeshFactory.Cylinder,false,root.transform);Torch(root.transform,new Vector3(i%2==0?-6.7f:6.7f,y+2.5f,142+i*5.0f),true);}
            DustVolume(root.transform,new Vector3(0,y+1.1f,153),new Vector3(14,2,32),new Color(.2f,.65f,.72f,.12f));
            BuildEncounter("flooded-depths",new Vector3(0,y+.8f,150),new Vector3(12,3,14),new[]{EnemyArchetype.GraveRat,EnemyArchetype.CryptCrawler,EnemyArchetype.CryptCrawler,EnemyArchetype.HollowSkeleton},new[]{new Vector3(-3,0,2),new Vector3(3,0,3),new Vector3(-2,0,6),new Vector3(2,0,8)},null);
            var gate=BuildGate(new Vector3(0,y+1.6f,169),root.transform);gate.SetActive(false);BuildEncounter("flooded-elite",new Vector3(0,y+.8f,172),new Vector3(12,3,9),new[]{EnemyArchetype.EliteSkeleton,EnemyArchetype.CryptCrawler},new[]{new Vector3(0,0,3),new Vector3(3,0,4)},gate);BuildChest(root.transform,new Vector3(0,y+.55f,181),55,45,true);
            CreateZone(new Vector3(0,y+2,153),new Vector3(15,6,38),2,MusicState.Exploration,"dungeon_wind");BuildTransitionStairs(root.transform,new Vector3(0,y,187),28,1.05f,.26f);
        }

        private void BuildForgottenTemple()
        {
            // Forgotten Temple — monumental ancient civilization below the catacombs.
            var root=new GameObject("Floor 3 - Forgotten Temple");root.transform.SetParent(contentRoot,false);var y=-25.24f;
            RoomShell("Forgotten Temple Nave",new Vector3(0,y,222),new Vector3(20,1,42),MaterialLibrary.Stone,8f);
            for(int row=0;row<2;row++)for(int i=0;i<6;i++){float x=row==0?-7.2f:7.2f;float z=206+i*6.2f;Column(root.transform,new Vector3(x,y+3.4f,z),6.4f,.72f);Shape("Glowing Temple Rune",new Vector3(x*.78f,y+.08f,z),new Vector3(1.1f,.07f,1.1f),MaterialLibrary.MagicBlue,ProceduralMeshFactory.Sphere,false,root.transform);}
            Shape("Central Ritual Dais",new Vector3(0,y+.25f,225),new Vector3(7f,.62f,7f),MaterialLibrary.Stone,null,true,root.transform);Shape("Ancient Seal",new Vector3(0,y+.6f,225),new Vector3(4.2f,.08f,4.2f),MaterialLibrary.MagicBlue,ProceduralMeshFactory.Sphere,false,root.transform);
            BuildEncounter("forgotten-temple",new Vector3(0,y+.8f,217),new Vector3(17,4,18),new[]{EnemyArchetype.Cultist,EnemyArchetype.HollowSkeleton,EnemyArchetype.Cultist,EnemyArchetype.EliteSkeleton},new[]{new Vector3(-5,0,3),new Vector3(0,0,6),new Vector3(5,0,4),new Vector3(0,0,10)},null);
            for(int i=0;i<5;i++)Torch(root.transform,new Vector3(i%2==0?-9.2f:9.2f,y+3.1f,206+i*7f),true);
            BuildChest(root.transform,new Vector3(0,y+.55f,240),85,60,true);CreateZone(new Vector3(0,y+3,222),new Vector3(20,9,45),3,MusicState.Exploration,"dungeon_wind");
            BuildTransitionStairs(root.transform,new Vector3(0,y,244),14,1.25f,.20f);
        }

        private void BuildBossArena()
        {
            // Boss Arena — THE CRYPT WARDEN.
            var root=new GameObject("Boss Arena");root.transform.SetParent(contentRoot,false);var y=-28.04f;
            RoomShell("Crypt Warden Sanctum",new Vector3(0,y,270),new Vector3(24,1,32),MaterialLibrary.Stone,9f);Arch(root.transform,new Vector3(0,y,254.7f),7f,7.8f,MaterialLibrary.Black);
            for(int i=0;i<8;i++){float a=i*Mathf.PI*2/8;var p=new Vector3(Mathf.Cos(a)*9.2f,y+3.4f,270+Mathf.Sin(a)*11f);Column(root.transform,p,6.4f,.75f);}
            Shape("Boss Ritual Floor",new Vector3(0,y+.06f,270),new Vector3(11f,.08f,11f),MaterialLibrary.Blood,ProceduralMeshFactory.Sphere,false,root.transform);for(int i=0;i<6;i++)Torch(root.transform,new Vector3((i%2==0?-10.5f:10.5f),y+3.4f,260+i*4.2f),true);
            var boss=SpawnBoss(new Vector3(0,y+.08f,274),root.transform);
            var altar=Shape("Extraction Altar",new Vector3(0,y+.65f,282),new Vector3(2.6f,1.35f,2.2f),MaterialLibrary.Black,null,true,root.transform);altar.AddComponent<ExtractionAltar>();Shape("Altar Soul",new Vector3(0,y+1.75f,282),Vector3.one*.62f,MaterialLibrary.MagicBlue,ProceduralMeshFactory.Sphere,false,root.transform);
            CreateZone(new Vector3(0,y+3,270),new Vector3(24,9,34),3,MusicState.Boss,"dungeon_wind");
        }

        private GameObject SpawnBoss(Vector3 pos,Transform parent)
        {
            var go=new GameObject("THE CRYPT WARDEN");go.transform.SetParent(EnemyRoot);go.transform.position=pos;var body=go.AddComponent<CapsuleCollider>();body.radius=.55f;body.height=2.4f;body.center=Vector3.up*1.2f;go.AddComponent<EnemyHealth>();var rig=VisualFactory.BuildWarden(go.transform);var boss=go.AddComponent<CryptWardenController>();boss.Configure(720f,rig);return go;
        }
        private GameObject BuildGate(Vector3 pos,Transform parent){var gate=new GameObject("Encounter Seal Gate");gate.transform.SetParent(parent,false);gate.transform.position=pos;Shape("Seal Bar L",Vector3.zero,new Vector3(.24f,4.2f,6f),MaterialLibrary.MagicBlue,null,true,gate.transform);Shape("Seal Bar R",Vector3.zero,new Vector3(6f,4.2f,.18f),MaterialLibrary.Black,null,true,gate.transform);return gate;}
        private void BuildEncounter(string id,Vector3 center,Vector3 size,EnemyArchetype[] types,Vector3[] offsets,GameObject gate)
        {var go=new GameObject("Encounter - "+id);go.transform.SetParent(contentRoot,false);go.transform.position=center;var c=go.AddComponent<BoxCollider>();c.isTrigger=true;c.size=size;go.AddComponent<EncounterController>().Configure(id,types,offsets,gate);}
        private void BuildTransitionStairs(Transform parent,Vector3 start,int count,float dz,float drop)
        {for(int i=0;i<count;i++)Shape("Deep Stair "+i,start+new Vector3(0,-i*drop-.18f,i*dz),new Vector3(5.4f,.42f,dz*1.06f),MaterialLibrary.Stone,null,true,parent);}
        private GameObject BuildChest(Transform parent,Vector3 pos,int essence,int gold,bool lootable)
        {
            var root=new GameObject(lootable?"Ancient Loot Chest":"Storage Chest");root.transform.SetParent(parent,false);root.transform.position=pos;Shape("Chest Base",Vector3.zero,new Vector3(1.55f,.72f,1.05f),MaterialLibrary.Wood,null,true,root.transform);var lid=Shape("Chest Lid",new Vector3(0,.52f,-.42f),new Vector3(1.6f,.42f,1.08f),MaterialLibrary.Wood,null,false,root.transform);Shape("Chest Iron Band",new Vector3(0,.1f,.54f),new Vector3(.28f,.74f,.06f),MaterialLibrary.Metal,null,false,root.transform);if(lootable){var trigger=root.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(2.2f,2f,2f);root.AddComponent<ChestInteractable>().Configure(lid.transform,essence,gold);}return root;
        }

        public void ReturnPlayerToSafeRoom(bool loseRun)
        {
            if(loseRun&&GameSession.Instance!=null&&GameSession.Instance.RunActive)GameSession.Instance.FinishRun(false);
            var player=Object.FindFirstObjectByType<PlayerController>();if(player!=null){player.TeleportSafe(SafeSpawn,SafeRotation);var v=player.GetComponent<DungeonDescent.Combat.PlayerVitals>();v?.RestoreForSafeRoom();}
            StartCoroutine(RebuildRuntime());
        }
        private IEnumerator RebuildRuntime(){if(contentRoot!=null)Destroy(contentRoot.gameObject);contentRoot=null;EnemyRoot=null;yield return null;BuildWorld();AudioManager.Instance?.SetMusic(MusicState.SafeRoom,1.6f);AudioManager.Instance?.SetAmbience("fireplace",.5f);}
    }
}
