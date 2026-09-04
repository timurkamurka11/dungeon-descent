using UnityEngine;

namespace DungeonDescent.Presentation
{
    public sealed class CharacterVisualRig : MonoBehaviour
    {
        public Transform Root;
        public Transform SwordHand;
        public Transform LeftHand;
        public Transform Head;
        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftLeg;
        public Transform RightLeg;
    }

    public static class VisualFactory
    {
        public static GameObject Form(string name,Transform parent,Mesh mesh,Material material,Vector3 localPos,Vector3 localScale,Vector3 localEuler)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=localPos;go.transform.localScale=localScale;go.transform.localEulerAngles=localEuler;
            var mf=go.AddComponent<MeshFilter>();mf.sharedMesh=mesh;var mr=go.AddComponent<MeshRenderer>();mr.sharedMaterial=material;return go;
        }

        public static CharacterVisualRig BuildHero(Transform parent)
        {
            var root=new GameObject("Hero Visual Root");root.transform.SetParent(parent,false);
            Form("Torso Cuirass",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(0,1.15f,0),new Vector3(.75f,.92f,.46f),Vector3.zero);
            Form("Waist Leather",root.transform,ProceduralMeshFactory.Cylinder,MaterialLibrary.Leather,new Vector3(0,.75f,0),new Vector3(.58f,.24f,.58f),Vector3.zero);
            var head=Form("Helm",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Metal,new Vector3(0,1.78f,0),new Vector3(.52f,.54f,.50f),Vector3.zero);
            Form("Helm Crest",head.transform,ProceduralMeshFactory.Cone,MaterialLibrary.Black,new Vector3(0,.48f,-.03f),new Vector3(.18f,.48f,.18f),new Vector3(0,0,0));
            Form("Face Guard",head.transform,ProceduralMeshFactory.Box,MaterialLibrary.Black,new Vector3(0,-.05f,.45f),new Vector3(.68f,.22f,.08f),Vector3.zero);
            Form("Eye Slit",head.transform,ProceduralMeshFactory.Box,MaterialLibrary.Fire,new Vector3(0,.06f,.495f),new Vector3(.43f,.045f,.025f),Vector3.zero);
            var lShoulder=Form("Left Pauldron",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Metal,new Vector3(-.57f,1.38f,0),new Vector3(.45f,.32f,.52f),Vector3.zero);
            var rShoulder=Form("Right Pauldron",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Metal,new Vector3(.57f,1.38f,0),new Vector3(.45f,.32f,.52f),Vector3.zero);
            var lh=BuildArm(root.transform,"Left",-.55f);var rh=BuildArm(root.transform,"Right",.55f);
            var leftLeg=BuildLeg(root.transform,"Left",-.24f);var rightLeg=BuildLeg(root.transform,"Right",.24f);
            Form("Cape",root.transform,ProceduralMeshFactory.Box,MaterialLibrary.Cloth,new Vector3(0,1.05f,-.37f),new Vector3(.82f,1.35f,.055f),new Vector3(8,0,0));
            var sword=Form("Runed Longsword",rh,ProceduralMeshFactory.Box,MaterialLibrary.Metal,new Vector3(0,-.58f,.17f),new Vector3(.10f,1.28f,.065f),new Vector3(14,0,-8));
            Form("Sword Fuller",sword.transform,ProceduralMeshFactory.Box,MaterialLibrary.MagicBlue,new Vector3(0,0,.54f),new Vector3(.24f,.80f,.035f),Vector3.zero);
            Form("Crossguard",rh,ProceduralMeshFactory.Box,MaterialLibrary.Black,new Vector3(0,-.04f,.12f),new Vector3(.72f,.09f,.09f),new Vector3(0,0,-8));
            Form("Shield",lh,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(-.18f,-.28f,.22f),new Vector3(.75f,.95f,.14f),new Vector3(0,0,8));
            var rig=root.AddComponent<CharacterVisualRig>();rig.Root=root.transform;rig.SwordHand=rh;rig.LeftHand=lh;rig.Head=head.transform;rig.LeftArm=lh;rig.RightArm=rh;rig.LeftLeg=leftLeg;rig.RightLeg=rightLeg;return rig;
        }
        private static Transform BuildArm(Transform root,string side,float x)
        {
            var upper=Form(side+" Upper Arm",root,ProceduralMeshFactory.Cylinder,MaterialLibrary.Black,new Vector3(x,1.13f,0),new Vector3(.20f,.62f,.20f),new Vector3(0,0,x>0?-9:9));
            var hand=Form(side+" Gauntlet",root,ProceduralMeshFactory.Sphere,MaterialLibrary.Metal,new Vector3(x*1.10f,.73f,.04f),new Vector3(.24f,.28f,.22f),Vector3.zero);return hand.transform;
        }
        private static Transform BuildLeg(Transform root,string side,float x)
        {
            var leg=Form(side+" Greave",root,ProceduralMeshFactory.Cylinder,MaterialLibrary.Black,new Vector3(x,.38f,0),new Vector3(.26f,.68f,.28f),Vector3.zero);
            Form(side+" Sabaton",leg.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Metal,new Vector3(0,-.44f,.45f),new Vector3(1.22f,.53f,1.72f),Vector3.zero);
            return leg.transform;
        }

        public static CharacterVisualRig BuildSkeleton(Transform parent,bool elite=false)
        {
            var root=new GameObject(elite?"Elite Hollow Skeleton Visual":"Hollow Skeleton Visual");root.transform.SetParent(parent,false);
            var bone=elite?MaterialLibrary.MagicBlue:MaterialLibrary.Bone;
            Form("Rib Mass",root.transform,ProceduralMeshFactory.Sphere,bone,new Vector3(0,1.0f,0),new Vector3(.48f,.60f,.28f),Vector3.zero);
            var skull=Form("Skull",root.transform,ProceduralMeshFactory.Sphere,bone,new Vector3(0,1.62f,0),new Vector3(.44f,.48f,.42f),Vector3.zero);
            Form("Eye Hollow L",skull.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(-.16f,.06f,.42f),new Vector3(.13f,.12f,.08f),Vector3.zero);
            Form("Eye Hollow R",skull.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(.16f,.06f,.42f),new Vector3(.13f,.12f,.08f),Vector3.zero);
            var lh=BoneArm(root.transform,-.44f);var rh=BoneArm(root.transform,.44f);var ll=BoneLeg(root.transform,-.19f);var rl=BoneLeg(root.transform,.19f);
            Form("Rust Sword",rh,ProceduralMeshFactory.Box,MaterialLibrary.Metal,new Vector3(0,-.58f,.12f),new Vector3(.09f,1.25f,.06f),new Vector3(10,0,-8));
            var rig=root.AddComponent<CharacterVisualRig>();rig.Root=root.transform;rig.SwordHand=rh;rig.LeftHand=lh;rig.Head=skull.transform;rig.LeftArm=lh;rig.RightArm=rh;rig.LeftLeg=ll;rig.RightLeg=rl;return rig;
        }
        private static Transform BoneArm(Transform root,float x){Form("Upper Bone",root,ProceduralMeshFactory.Cylinder,MaterialLibrary.Bone,new Vector3(x,1.02f,0),new Vector3(.11f,.55f,.11f),new Vector3(0,0,x>0?-12:12));return Form("Bone Hand",root,ProceduralMeshFactory.Sphere,MaterialLibrary.Bone,new Vector3(x*1.12f,.65f,0),new Vector3(.16f,.18f,.14f),Vector3.zero).transform;}
        private static Transform BoneLeg(Transform root,float x){return Form("Leg Bone",root,ProceduralMeshFactory.Cylinder,MaterialLibrary.Bone,new Vector3(x,.38f,0),new Vector3(.12f,.72f,.12f),Vector3.zero).transform;}

        public static CharacterVisualRig BuildRat(Transform parent)
        {
            var root=new GameObject("Grave Rat Visual");root.transform.SetParent(parent,false);
            var body=Form("Rat Body",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Leather,new Vector3(0,.32f,0),new Vector3(.72f,.40f,1.05f),Vector3.zero);
            var head=Form("Rat Head",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(0,.36f,.58f),new Vector3(.52f,.42f,.62f),Vector3.zero);
            Form("Left Ear",head.transform,ProceduralMeshFactory.Cone,MaterialLibrary.Cloth,new Vector3(-.23f,.28f,0),new Vector3(.22f,.30f,.18f),new Vector3(0,0,-15));Form("Right Ear",head.transform,ProceduralMeshFactory.Cone,MaterialLibrary.Cloth,new Vector3(.23f,.28f,0),new Vector3(.22f,.30f,.18f),new Vector3(0,0,15));
            Form("Left Eye",head.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Fire,new Vector3(-.20f,.05f,.50f),new Vector3(.08f,.08f,.06f),Vector3.zero);Form("Right Eye",head.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Fire,new Vector3(.20f,.05f,.50f),new Vector3(.08f,.08f,.06f),Vector3.zero);
            Form("Tail",root.transform,ProceduralMeshFactory.Cylinder,MaterialLibrary.Cloth,new Vector3(0,.28f,-.72f),new Vector3(.09f,1.25f,.09f),new Vector3(70,0,0));
            var rig=root.AddComponent<CharacterVisualRig>();rig.Root=root.transform;rig.Head=head.transform;return rig;
        }

        public static CharacterVisualRig BuildCrawler(Transform parent)
        {
            var root=new GameObject("Crypt Crawler Visual");root.transform.SetParent(parent,false);
            var core=Form("Crawler Core",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(0,.52f,0),new Vector3(.75f,.50f,.82f),Vector3.zero);
            var head=Form("Crawler Mask",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Bone,new Vector3(0,.55f,.52f),new Vector3(.46f,.42f,.38f),new Vector3(18,0,0));
            for(int side=-1;side<=1;side+=2)for(int i=0;i<3;i++)Form("Crawler Limb",root.transform,ProceduralMeshFactory.Cylinder,MaterialLibrary.Black,new Vector3(side*(.42f+i*.08f),.30f,.2f-i*.32f),new Vector3(.10f,.85f,.10f),new Vector3(35+i*18,0,side*(45+i*10)));
            Form("Crawler Eye",head.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.MagicBlue,new Vector3(0,.02f,.46f),new Vector3(.14f,.10f,.06f),Vector3.zero);
            var rig=root.AddComponent<CharacterVisualRig>();rig.Root=root.transform;rig.Head=head.transform;return rig;
        }

        public static CharacterVisualRig BuildCultist(Transform parent)
        {
            var root=new GameObject("Temple Cultist Visual");root.transform.SetParent(parent,false);
            Form("Robe",root.transform,ProceduralMeshFactory.Cone,MaterialLibrary.Cloth,new Vector3(0,.72f,0),new Vector3(.78f,1.45f,.78f),Vector3.zero);
            var hood=Form("Deep Hood",root.transform,ProceduralMeshFactory.Cone,MaterialLibrary.Black,new Vector3(0,1.58f,0),new Vector3(.62f,.72f,.62f),Vector3.zero);
            Form("Veiled Face",hood.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Black,new Vector3(0,-.18f,.20f),new Vector3(.50f,.40f,.30f),Vector3.zero);
            var hand=Form("Staff Hand",root.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.Skin,new Vector3(.52f,.92f,.05f),new Vector3(.20f,.20f,.20f),Vector3.zero);
            Form("Ancient Staff",hand.transform,ProceduralMeshFactory.Cylinder,MaterialLibrary.Wood,new Vector3(0,-.52f,0),new Vector3(.09f,1.65f,.09f),Vector3.zero);
            Form("Staff Orb",hand.transform,ProceduralMeshFactory.Sphere,MaterialLibrary.MagicBlue,new Vector3(0,.46f,0),new Vector3(.28f,.28f,.28f),Vector3.zero);
            var rig=root.AddComponent<CharacterVisualRig>();rig.Root=root.transform;rig.SwordHand=hand.transform;rig.Head=hood.transform;return rig;
        }

        public static CharacterVisualRig BuildWarden(Transform parent)
        {
            var rig=BuildHero(parent);rig.Root.name="Crypt Warden Visual";rig.Root.localScale=Vector3.one*1.32f;
            Form("Warden Horn L",rig.Head,ProceduralMeshFactory.Cone,MaterialLibrary.Black,new Vector3(-.26f,.38f,-.02f),new Vector3(.16f,.52f,.16f),new Vector3(0,0,-24));
            Form("Warden Horn R",rig.Head,ProceduralMeshFactory.Cone,MaterialLibrary.Black,new Vector3(.26f,.38f,-.02f),new Vector3(.16f,.52f,.16f),new Vector3(0,0,24));
            Form("Warden Halo",rig.Head,ProceduralMeshFactory.Sphere,MaterialLibrary.MagicBlue,new Vector3(0,.10f,-.48f),new Vector3(.68f,.68f,.06f),Vector3.zero);
            return rig;
        }
    }
}
