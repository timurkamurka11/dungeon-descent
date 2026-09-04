using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DungeonDescent.Player
{
    public sealed class PlayerAnimationController : MonoBehaviour
    {
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        private AnimationClipPlayable idlePlayable;
        private AnimationClipPlayable walkPlayable;
        private AnimationClipPlayable runPlayable;
        private AnimationClipPlayable actionPlayable;
        private readonly Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>(StringComparer.OrdinalIgnoreCase);

        private AnimationClip idleClip;
        private AnimationClip walkClip;
        private AnimationClip runClip;
        private AnimationClip jumpClip;
        private AnimationClip dodgeClip;
        private AnimationClip hitClip;
        private AnimationClip deathClip;
        private AnimationClip blockClip;
        private AnimationClip[] lightAttacks;
        private AnimationClip heavyAttack;
        private float targetSpeed;
        private float smoothedSpeed;
        private float actionElapsed;
        private float actionDuration;
        private float actionWeight;
        private bool holdAction;

        public bool IsConfigured => graph.IsValid();

        public void Configure(RiggedPlayerVisual visual)
        {
            DisposeGraph();
            clips.Clear();
            if (visual == null || visual.Animator == null)
            {
                Debug.LogError("DUNGEON DESCENT: cannot configure player animation without the rigged KayKit visual.");
                return;
            }

            foreach (var clip in visual.Clips)
                if (clip != null && !clips.ContainsKey(clip.name)) clips.Add(clip.name, clip);

            idleClip = Find("Idle");
            walkClip = Find("Walking_A", "Walking_B");
            runClip = Find("Running_A", "Running_B");
            jumpClip = Find("Jump_Full_Short", "Jump_Start");
            dodgeClip = Find("Dodge_Forward");
            hitClip = Find("Hit_A", "Hit_B");
            deathClip = Find("Death_A", "Death_B");
            blockClip = Find("Blocking", "Block");
            lightAttacks = new[]
            {
                Find("1H_Melee_Attack_Chop"),
                Find("1H_Melee_Attack_Slice_Diagonal"),
                Find("1H_Melee_Attack_Slice_Horizontal")
            };
            heavyAttack = Find("1H_Melee_Attack_Stab", "1H_Melee_Attack_Chop");

            if (idleClip == null || walkClip == null || runClip == null || jumpClip == null || lightAttacks[0] == null)
            {
                Debug.LogError("DUNGEON DESCENT: KayKit animation contract is incomplete. Idle/Walk/Run/Jump/1H attack are required.");
                return;
            }

            visual.Animator.applyRootMotion = false;
            visual.Animator.runtimeAnimatorController = null;
            graph = PlayableGraph.Create("Dungeon Descent Player Animation Graph");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            mixer = AnimationMixerPlayable.Create(graph, 4, true);
            idlePlayable = CreateLoop(idleClip);
            walkPlayable = CreateLoop(walkClip);
            runPlayable = CreateLoop(runClip);
            graph.Connect(idlePlayable, 0, mixer, 0);
            graph.Connect(walkPlayable, 0, mixer, 1);
            graph.Connect(runPlayable, 0, mixer, 2);
            mixer.SetInputWeight(0, 1f);
            mixer.SetInputWeight(1, 0f);
            mixer.SetInputWeight(2, 0f);
            mixer.SetInputWeight(3, 0f);
            var output = AnimationPlayableOutput.Create(graph, "Player Animation", visual.Animator);
            output.SetSourcePlayable(mixer);
            graph.Play();
        }

        private AnimationClipPlayable CreateLoop(AnimationClip clip)
        {
            var playable = AnimationClipPlayable.Create(graph, clip);
            playable.SetApplyFootIK(true);
            playable.SetTime(0d);
            return playable;
        }

        private AnimationClip Find(params string[] aliases)
        {
            foreach (var alias in aliases)
                if (clips.TryGetValue(alias, out var exact)) return exact;
            foreach (var pair in clips)
                foreach (var alias in aliases)
                    if (pair.Key.IndexOf(alias, StringComparison.OrdinalIgnoreCase) >= 0) return pair.Value;
            return null;
        }

        public void SetLocomotion(float speed01, bool grounded)
        {
            targetSpeed = grounded ? Mathf.Clamp01(speed01) : Mathf.Min(targetSpeed, .55f);
        }

        private void Update()
        {
            if (!graph.IsValid()) return;
            smoothedSpeed = Mathf.MoveTowards(smoothedSpeed, targetSpeed, Time.deltaTime * 4.5f);

            var idleWeight = 1f - Mathf.InverseLerp(.04f, .24f, smoothedSpeed);
            var runWeight = Mathf.InverseLerp(.58f, .94f, smoothedSpeed);
            var walkWeight = Mathf.Clamp01(1f - idleWeight - runWeight);

            var desiredAction = 0f;
            if (actionPlayable.IsValid())
            {
                actionElapsed += Time.deltaTime;
                var fadeIn = Mathf.Clamp01(actionElapsed / .07f);
                var fadeOut = holdAction ? 1f : Mathf.Clamp01((actionDuration - actionElapsed) / .12f);
                desiredAction = Mathf.Min(fadeIn, fadeOut);
                if (!holdAction && actionElapsed >= actionDuration) ClearAction();
            }
            actionWeight = Mathf.MoveTowards(actionWeight, desiredAction, Time.deltaTime * 18f);
            var baseWeight = 1f - actionWeight;
            mixer.SetInputWeight(0, idleWeight * baseWeight);
            mixer.SetInputWeight(1, walkWeight * baseWeight);
            mixer.SetInputWeight(2, runWeight * baseWeight);
            mixer.SetInputWeight(3, actionPlayable.IsValid() ? actionWeight : 0f);
        }

        public void PlayJump() => PlayAction(jumpClip, false, 1f);
        public void PlayDodge() => PlayAction(dodgeClip, false, 1.08f);
        public void PlayHit() => PlayAction(hitClip, false, 1f);
        public void PlayDeath() => PlayAction(deathClip, true, 1f);

        public void PlayAttack(bool heavy, int comboIndex)
        {
            AnimationClip clip;
            if (heavy) clip = heavyAttack;
            else
            {
                var index = Mathf.Clamp(comboIndex - 1, 0, lightAttacks.Length - 1);
                clip = lightAttacks[index] ?? lightAttacks[0];
            }
            PlayAction(clip, false, heavy ? .92f : 1.12f);
        }

        public void SetBlocking(bool active)
        {
            if (active) PlayAction(blockClip, true, 1f);
            else if (holdAction) ClearAction();
        }

        private void PlayAction(AnimationClip clip, bool hold, float speed)
        {
            if (!graph.IsValid() || clip == null) return;
            ClearAction();
            actionPlayable = AnimationClipPlayable.Create(graph, clip);
            actionPlayable.SetApplyFootIK(true);
            actionPlayable.SetTime(0d);
            actionPlayable.SetSpeed(speed);
            graph.Connect(actionPlayable, 0, mixer, 3);
            actionElapsed = 0f;
            actionDuration = Mathf.Max(.08f, clip.length / Mathf.Max(.01f, speed));
            actionWeight = 0f;
            holdAction = hold;
        }

        private void ClearAction()
        {
            if (!graph.IsValid()) return;
            if (actionPlayable.IsValid())
            {
                graph.Disconnect(mixer, 3);
                actionPlayable.Destroy();
            }
            actionWeight = 0f;
            actionElapsed = 0f;
            actionDuration = 0f;
            holdAction = false;
            if (mixer.IsValid()) mixer.SetInputWeight(3, 0f);
        }

        public void ResetPose() => ClearAction();

        private void OnDestroy() => DisposeGraph();

        private void DisposeGraph()
        {
            if (graph.IsValid()) graph.Destroy();
            actionPlayable = default;
            mixer = default;
        }
    }
}
