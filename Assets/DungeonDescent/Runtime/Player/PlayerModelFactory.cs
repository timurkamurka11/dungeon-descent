using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonDescent.Player
{
    public sealed class RiggedPlayerVisual
    {
        public GameObject Root { get; }
        public Animator Animator { get; }
        public AnimationClip[] Clips { get; }

        public RiggedPlayerVisual(GameObject root, Animator animator, AnimationClip[] clips)
        {
            Root = root;
            Animator = animator;
            Clips = clips ?? Array.Empty<AnimationClip>();
        }
    }

    public static class PlayerModelFactory
    {
        public const string ResourcePath = "Models/Hero/KayKit/Knight";
        private const string TexturePath = "Models/Hero/KayKit/knight_texture";
        private const float TargetHeight = 1.78f;

        public static RiggedPlayerVisual Build(Transform parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var existing = parent.Find("Rigged Player Visual");
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(existing.gameObject);
            }

            var source = Resources.Load<GameObject>(ResourcePath);
            if (source == null)
            {
                Debug.LogError($"DUNGEON DESCENT: required rigged player model is missing at Resources/{ResourcePath}.fbx");
                return null;
            }

            var root = UnityEngine.Object.Instantiate(source, parent, false);
            root.name = "Rigged Player Visual";
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;

            foreach (var collider in root.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
            foreach (var body in root.GetComponentsInChildren<Rigidbody>(true))
            {
                body.isKinematic = true;
                body.detectCollisions = false;
            }

            ConfigureAccessoryVisibility(root);
            ConfigureUrpMaterial(root);
            NormalizeHeight(root.transform, parent);
            SetLayerRecursively(root, parent.gameObject.layer);

            var animator = root.GetComponentInChildren<Animator>(true);
            if (animator == null)
            {
                Debug.LogError("DUNGEON DESCENT: KayKit Knight imported without an Animator. Check KayKitModelImportProcessor.");
                UnityEngine.Object.Destroy(root);
                return null;
            }

            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            var clips = Resources.LoadAll<AnimationClip>(ResourcePath);
            ValidateRequiredClips(clips);
            return new RiggedPlayerVisual(root, animator, clips);
        }

        private static void ConfigureAccessoryVisibility(GameObject root)
        {
            var disabledNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Rectangle_Shield", "Round_Shield", "Spike_Shield", "1H_Sword_Offhand", "2H_Sword"
            };
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                if (disabledNames.Contains(renderer.gameObject.name)) renderer.enabled = false;
        }

        private static void ConfigureUrpMaterial(GameObject root)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var texture = Resources.Load<Texture2D>(TexturePath);
            if (shader == null || texture == null) return;

            var material = new Material(shader) { name = "KayKit Knight URP Runtime Material" };
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Smoothness", .32f);
            material.SetFloat("_Metallic", .08f);
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true)) renderer.sharedMaterial = material;
        }

        private static void NormalizeHeight(Transform root, Transform parent)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) return;
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            if (bounds.size.y > .01f)
            {
                var scale = TargetHeight / bounds.size.y;
                root.localScale *= scale;
            }

            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            root.localPosition += Vector3.up * (parent.position.y - bounds.min.y);
        }

        private static void ValidateRequiredClips(AnimationClip[] clips)
        {
            var required = new[] { "Idle", "Walking_A", "Running_A", "Jump_Full_Short", "1H_Melee_Attack_Slice_Horizontal" };
            foreach (var requiredName in required)
            {
                var found = false;
                foreach (var clip in clips)
                    if (string.Equals(clip.name, requiredName, StringComparison.OrdinalIgnoreCase)) { found = true; break; }
                if (!found) Debug.LogError($"DUNGEON DESCENT: KayKit Knight is missing required animation clip '{requiredName}'.");
            }
        }

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            root.layer = layer;
            foreach (Transform child in root.transform) SetLayerRecursively(child.gameObject, layer);
        }
    }
}
