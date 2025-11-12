using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private const string MainVolumeKey = "MainVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string MusicVolumeKey = "MusicVolume";

    private void Start()
    {
        // Carica i valori salvati o imposta default
        float mainVolume = PlayerPrefs.GetFloat(MainVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        // Imposta slider
        mainSlider.value = mainVolume;
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;

        // Applica ai mixer
        AudioManager.Instance.SetMainVolume(mainVolume);
        AudioManager.Instance.SetSfxVolume(sfxVolume);
        AudioManager.Instance.SetMusicVolume(musicVolume);

        // Listener con salvataggio
        mainSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    private void OnMainVolumeChanged(float value)
    {
        AudioManager.Instance.SetMainVolume(value);
        PlayerPrefs.SetFloat(MainVolumeKey, value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }
}