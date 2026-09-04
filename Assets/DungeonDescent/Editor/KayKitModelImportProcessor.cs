using System;
using UnityEditor;

namespace DungeonDescent.Editor
{
    public sealed class KayKitModelImportProcessor : AssetPostprocessor
    {
        private const string KnightPath = "Assets/DungeonDescent/Resources/Models/Hero/KayKit/Knight.fbx";

        private void OnPreprocessModel()
        {
            if (!string.Equals(assetPath, KnightPath, StringComparison.OrdinalIgnoreCase)) return;
            var importer = (ModelImporter)assetImporter;
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.resampleCurves = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;

            var clips = importer.defaultClipAnimations;
            for (var i = 0; i < clips.Length; i++)
            {
                var name = clips[i].name ?? string.Empty;
                var loop = name.Equals("Idle", StringComparison.OrdinalIgnoreCase)
                           || name.StartsWith("Walking_", StringComparison.OrdinalIgnoreCase)
                           || name.StartsWith("Running_", StringComparison.OrdinalIgnoreCase)
                           || name.Equals("Blocking", StringComparison.OrdinalIgnoreCase);
                clips[i].loopTime = loop;
                clips[i].loopPose = loop;
            }
            importer.clipAnimations = clips;
        }
    }
}
