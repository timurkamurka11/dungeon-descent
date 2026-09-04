using System.Collections;
using DungeonDescent.Save;
using UnityEngine;

namespace DungeonDescent.Audio
{
    public enum MusicState { None, SafeRoom, Exploration, Combat, Elite, Boss }

    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private AudioSource musicA, musicB, ambience, sfx;
        private bool usingA = true;
        private MusicState state;
        private SaveData settings;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            settings = SaveManager.Load();
            musicA = MakeSource("Music A", true, settings.MusicVolume);
            musicB = MakeSource("Music B", true, 0f);
            ambience = MakeSource("Ambience", true, settings.SfxVolume * .55f);
            sfx = MakeSource("SFX", false, settings.SfxVolume);
        }

        private AudioSource MakeSource(string label, bool loop, float volume)
        {
            var go = new GameObject(label); go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>(); src.loop = loop; src.playOnAwake = false; src.volume = volume; src.spatialBlend = 0f;
            return src;
        }

        public void SetMusic(MusicState next, float fadeSeconds = 1.25f)
        {
            if (next == state) return;
            state = next;
            var clip = LoadMusic(next);
            var incoming = usingA ? musicB : musicA;
            var outgoing = usingA ? musicA : musicB;
            usingA = !usingA;
            incoming.clip = clip; incoming.volume = 0f;
            if (clip != null) incoming.Play();
            StopAllCoroutines();
            StartCoroutine(Crossfade(outgoing, incoming, Mathf.Max(.05f, fadeSeconds)));
        }

        public void SetAmbience(string clipName, float volume = .4f)
        {
            var clip = Resources.Load<AudioClip>("Audio/" + clipName);
            if (ambience.clip == clip && ambience.isPlaying) return;
            ambience.clip = clip; ambience.volume = settings.SfxVolume * volume;
            if (clip != null) ambience.Play(); else ambience.Stop();
        }

        public void PlaySfx(string clipName, float volume = 1f)
        {
            var clip = Resources.Load<AudioClip>("Audio/" + clipName);
            if (clip != null) sfx.PlayOneShot(clip, Mathf.Clamp01(volume) * settings.SfxVolume);
        }

        public void ApplyVolumes(float master, float music, float effects)
        {
            AudioListener.volume = Mathf.Clamp01(master);
            settings.MasterVolume = master; settings.MusicVolume = music; settings.SfxVolume = effects;
            var active = usingA ? musicA : musicB;
            active.volume = music;
            sfx.volume = effects;
            ambience.volume = effects * .55f;
        }

        private AudioClip LoadMusic(MusicState s)
        {
            string n;
            switch (s)
            {
                case MusicState.SafeRoom: n = "safe_room"; break;
                case MusicState.Exploration: n = "exploration"; break;
                case MusicState.Combat: case MusicState.Elite: n = "combat"; break;
                case MusicState.Boss: n = "boss"; break;
                default: return null;
            }
            return Resources.Load<AudioClip>("Audio/" + n);
        }

        private IEnumerator Crossfade(AudioSource outgoing, AudioSource incoming, float duration)
        {
            var t = 0f; var startOut = outgoing.volume; var target = settings.MusicVolume;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime; var a = Mathf.Clamp01(t / duration);
                outgoing.volume = Mathf.Lerp(startOut, 0f, a); incoming.volume = Mathf.Lerp(0f, target, a);
                yield return null;
            }
            outgoing.Stop(); outgoing.clip = null; incoming.volume = target;
        }
    }
}
