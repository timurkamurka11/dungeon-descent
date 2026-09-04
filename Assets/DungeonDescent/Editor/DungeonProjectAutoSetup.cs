#if UNITY_EDITOR
using System.IO;
using DungeonDescent.Data;
using DungeonDescent.Progression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DungeonDescent.Editor
{
    [InitializeOnLoad]
    public static class DungeonProjectAutoSetup
    {
        private const string PipelinePath="Assets/DungeonDescent/Config/DungeonDescent_URP.asset";
        static DungeonProjectAutoSetup(){EditorApplication.delayCall+=RunOncePerDomain;}
        [MenuItem("Dungeon Descent/Run Project Auto Setup")]
        public static void RunOncePerDomain()
        {
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            EnsureFolders();ConfigureProject();ConfigureTextures();EnsurePipeline();CreateConfigAssets();AssetDatabase.SaveAssets();
        }
        private static void EnsureFolders(){Directory.CreateDirectory("Assets/DungeonDescent/Config");Directory.CreateDirectory("Assets/DungeonDescent/Config/Enemies");Directory.CreateDirectory("Assets/DungeonDescent/Config/Biomes");}
        private static void ConfigureProject(){PlayerSettings.companyName="Dungeon Descent";PlayerSettings.productName="DUNGEON DESCENT";PlayerSettings.colorSpace=ColorSpace.Linear;QualitySettings.vSyncCount=1;Application.targetFrameRate=144;}
        private static void ConfigureTextures()
        {
            var normalPath="Assets/DungeonDescent/Art/Textures/stone_normal.png";var importer=AssetImporter.GetAtPath(normalPath) as TextureImporter;if(importer!=null&&importer.textureType!=TextureImporterType.NormalMap){importer.textureType=TextureImporterType.NormalMap;importer.SaveAndReimport();}
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/DungeonDescent/Art/Textures"})){var p=AssetDatabase.GUIDToAssetPath(guid);var ti=AssetImporter.GetAtPath(p) as TextureImporter;if(ti==null)continue;ti.maxTextureSize=2048;ti.textureCompression=TextureImporterCompression.CompressedHQ;ti.anisoLevel=4;ti.SaveAndReimport();}
        }
        private static void EnsurePipeline()
        {
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if(pipeline==null)
            {
                pipeline=ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                pipeline.name="Dungeon Descent URP";
                AssetDatabase.CreateAsset(pipeline,PipelinePath);
                var renderer=pipeline.LoadBuiltinRendererData(RendererType.UniversalRenderer);
                if(renderer!=null&&!AssetDatabase.Contains(renderer))AssetDatabase.AddObjectToAsset(renderer,pipeline);
                EditorUtility.SetDirty(pipeline);
            }
            GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
        }
        private static void CreateConfigAssets()
        {
            EnsureAsset<WeaponData>("Assets/DungeonDescent/Config/RunedLongsword.asset",w=>{w.Id="runed-longsword";w.DisplayName="Runed Longsword";w.BaseDamage=24;w.HeavyMultiplier=1.8f;w.Stagger=8;});
            MakeEnemy("grave-rat",38,5,9,4);MakeEnemy("hollow-skeleton",92,3.25f,17,10);MakeEnemy("crypt-crawler",68,4.4f,14,8);MakeEnemy("cultist",85,2.8f,18,14);
            MakeBiome("old-catacombs","THE OLD CATACOMBS",new Color(.18f,.19f,.19f),.012f);MakeBiome("flooded-depths","THE FLOODED DEPTHS",new Color(.06f,.18f,.20f),.018f);MakeBiome("forgotten-temple","THE FORGOTTEN TEMPLE",new Color(.06f,.10f,.16f),.014f);
            foreach(UpgradeKind kind in System.Enum.GetValues(typeof(UpgradeKind))){var path=$"Assets/DungeonDescent/Config/Upgrade_{kind}.asset";EnsureAsset<UpgradeData>(path,u=>{u.Kind=kind;u.DisplayName=kind.ToString();u.BaseCost=kind==UpgradeKind.PotionCapacity?180:100;});}
        }
        private static void MakeEnemy(string id,float hp,float speed,float damage,int reward){var path=$"Assets/DungeonDescent/Config/Enemies/{id}.asset";EnsureAsset<EnemyData>(path,e=>{e.Id=id;e.MaxHealth=hp;e.MoveSpeed=speed;e.AttackDamage=damage;e.EssenceReward=reward;});}
        private static void MakeBiome(string id,string display,Color fog,float density){var path=$"Assets/DungeonDescent/Config/Biomes/{id}.asset";EnsureAsset<DungeonBiomeData>(path,b=>{b.Id=id;b.DisplayName=display;b.FogColor=fog;b.FogDensity=density;b.AmbienceClip="dungeon_wind";});}
        private static void EnsureAsset<T>(string path,System.Action<T> init) where T:ScriptableObject
        {if(AssetDatabase.LoadAssetAtPath<T>(path)!=null)return;var asset=ScriptableObject.CreateInstance<T>();init(asset);AssetDatabase.CreateAsset(asset,path);EditorUtility.SetDirty(asset);}
    }
}
#endif
