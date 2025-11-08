using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SfxVolume";

    [Header("Audio Sources ")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Libraries")]
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] private SceneLibrary sceneMusicLibrary;

    private readonly Dictionary<string, AudioClip> audioDict = new();
    private readonly Dictionary<string, string> sceneMusicDict = new();

    protected override void Awake()
    {
        base.Awake();
        SetupAudioSources();
        LoadAudioLibraries();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            if (audioMixer != null)
                musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            if (audioMixer != null)
                sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sfx")[0];
        }
    }

    private void LoadAudioLibraries()
    {
        if (audioLibrary != null)
        {
            foreach (var entry in audioLibrary.clips)
            {
                if (!audioDict.ContainsKey(entry.key))
                    audioDict.Add(entry.key, entry.clip);
            }
        }

        if (sceneMusicLibrary != null)
        {
            foreach (var entry in sceneMusicLibrary.sceneMusics)
            {
                if (!sceneMusicDict.ContainsKey(entry.sceneName))
                    sceneMusicDict.Add(entry.sceneName, entry.musicKey);
            }
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneMusicDict.TryGetValue(scene.name, out var musicKey))
        {
            PlayMusic(musicKey);
        }
    }


    public void PlayMusic(string key, float volume = 1f)
    {
        if (audioDict.TryGetValue(key, out var clip))
        {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Clip '{key}' non trovata.");
        }
    }

    public void StopMusic() => musicSource?.Stop();

    public void PlaySfx(string key, float volume = 1f)
    {
        if (audioDict.TryGetValue(key, out var clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Sfx '{key}' non trovata.");
        }
    }

    public AudioClip GetClip(string key) =>
        audioDict.TryGetValue(key, out var clip) ? clip : null;

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(musicVolumeParam, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void SetSfxVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(sfxVolumeParam, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
