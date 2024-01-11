using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;
using CustomExtensions.Collections;
using UnityEngine.UIElements;
using FishNet.Broadcast;
using Unity.VisualScripting;
using static Unity.VisualScripting.Member;
using Unity.VisualScripting.FullSerializer;

[Serializable]
public struct ClipConfig : ICacheable<string> {
    public string Key { get => clip.name; }

    public Purpose purpose;
    
    [ConditionalField(nameof(purpose), true, (int) Purpose.Unknown)]
    public AudioClip clip;

    [ConditionalField(nameof(purpose), true, (int) Purpose.Unknown)]
    [Range(0f, 1f)] public float volume;

    [ConditionalField(nameof(purpose), (int) Purpose.Music, (int) Purpose.LocalisedSFX)]
    public Advanced advanced;

    [Serializable]
    public enum Purpose : int {
        Unknown       = 0,
        Music         = 1, // If played again, won't start over from scratch
        SFX           = 2,
        LocalisedSFX  = 3,
    }

    [Serializable]
    public struct Advanced { 
        public bool loop;

        //[Range(0f, 1f)] public float pitch;
        [Range(0f, 1f)] public float spatialBlend;
    }
}

public class AudioManager : MonoBehaviour, ICache<string, ClipConfig> {
    public static AudioManager Instance { get; private set; }

    // ==================== Configuration ====================
    [SerializeField, DisableOnPlay] List<ClipConfig> audioClipConfigs;


    // ==================== Audio Cache ====================
    readonly Dictionary<string, ClipConfig> audioCache = new();
    readonly Dictionary<ClipConfig.Purpose, AudioSource> audioSources = new();

    public void Register(ClipConfig config) {
        var key = config.Key;
        audioCache[key] = config;
    }

    public bool TryGet(string clipName, out ClipConfig config) {
        return audioCache.TryGetValue(clipName, out config);
    }

    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;

            Init();
        }
    }

    void Init() {
        // Cache all configs
        audioClipConfigs
            .Where(c => {
                return c.clip != null
                    && c.purpose != ClipConfig.Purpose.Unknown;
            })
            .ForEach(c => {
                Register(c);
            });

        // Create audio sources
        audioSources[ClipConfig.Purpose.Music] = this.gameObject.AddComponent<AudioSource>();
        audioSources[ClipConfig.Purpose.SFX] = this.gameObject.AddComponent<AudioSource>();
        audioSources[ClipConfig.Purpose.LocalisedSFX] = this.gameObject.AddComponent<AudioSource>();
    }

    // ==================== Public API =====================
    public static void PlayClip(AudioClip clip, bool restart = true) => PlayClip(clip.name, restart);
    public static void PlayClip(string clipName, bool restart = true) {
        if (Instance && !string.IsNullOrEmpty(clipName)) {
            var found = Instance.TryGet(clipName, out var config);

            // If found and we have an audio source for it
            if (found && Instance.audioSources.TryGetValue(config.purpose, out var source)) {
                switch (config.purpose) {
                    case ClipConfig.Purpose.Music:
                        ConfigAudioSource(source, config);

                        // Don't do anything if we don't want to restart and it's the same track
                        if (!restart && source.clip == config.clip) break;
                        source.Stop(); source.Play();
                        break;

                    case ClipConfig.Purpose.SFX:
                    case ClipConfig.Purpose.LocalisedSFX:
                        // Just play the audio
                        source.PlayOneShot(config.clip, config.volume);
                        break;

                    default:
                        Debug.LogError($"AudioManager.PlayClip(): Unsupported clip purpose: '{config.purpose}'");
                        break;
                }
            }
            else {
                Debug.LogError($"AudioManager.PlayClip(): Couldn't locate settings for audio clip '{clipName}'");
            }
        }
    }

    public static void PlayClipAt(AudioClip clip, Vector3 position) => PlayClipAt(clip.name, position);
    public static void PlayClipAt(string clipName, Vector3 position) {
        if (Instance && !string.IsNullOrEmpty(clipName)) {
            var found = Instance.TryGet(clipName, out var config);

            // If found and we have an audio source for it
            if (found && Instance.audioSources.TryGetValue(config.purpose, out var source)) {
                switch (config.purpose) {
                    case ClipConfig.Purpose.SFX:
                    case ClipConfig.Purpose.LocalisedSFX:
                        // Just play the audio
                        AudioSource.PlayClipAtPoint(config.clip, position, config.volume);
                        break;

                    default:
                        Debug.LogError($"AudioManager.PlayClipAt(): Unsupported clip purpose: '{config.purpose}'");
                        break;
                }
            }
            else {
                Debug.LogError($"AudioManager.PlayClipAt(): Couldn't locate settings for audio clip '{clipName}'");
            }
        }
    }

    public static void PlayClipOn(AudioClip clip, AudioSource source, bool restart = true) => PlayClipOn(clip.name, source, restart);
    public static void PlayClipOn(string clipName, AudioSource source, bool restart = true) {
        if (Instance && !string.IsNullOrEmpty(clipName)) {
            var found = Instance.TryGet(clipName, out var config);
            if (found) {
                ConfigAudioSource(source, config);
                switch (config.purpose) {
                    case ClipConfig.Purpose.Music:
                        // Don't do anything if we don't want to restart and it's the same track
                        if (!restart && source.clip == config.clip) break;
                        source.Stop(); source.Play();
                        break;

                    case ClipConfig.Purpose.SFX:
                    case ClipConfig.Purpose.LocalisedSFX:
                        source.Stop(); source.Play();
                        break;

                    default:
                        Debug.LogError($"AudioManager.PlayClipOn(): Unsupported clip purpose: '{config.purpose}'");
                        break;
                }
            }
            else {
                Debug.LogError($"AudioManager.PlayClipOn(): Couldn't locate settings for audio clip '{clipName}'");
            }
        }
    }
    public static void ConfigAudioSource(AudioSource source, ClipConfig config) {
        source.clip = config.clip;
        source.volume = config.volume;
        //source.pitch = config.advanced.pitch;
        source.spatialBlend = config.advanced.spatialBlend;
        source.loop = config.advanced.loop;
    }
}

