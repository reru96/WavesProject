using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Resolution, Fullscreen")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider;
    private Resolution[] resolutions;

    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private CanvasGroup optionsMenuCanva;

    private void Start()
    {
        SetupResolutions();
        SetupFullscreen();
        SetupBrightness();
        SetUpVolume();

        HideOptionsMenu();
    }

    public void HideOptionsMenu()
    {
        if (optionsMenuCanva == null)
        {
            optionsMenuCanva = GetComponent<CanvasGroup>();
        }
        if (optionsMenuCanva.alpha > 0f) 
        {
            optionsMenuCanva.alpha = 0f;
            optionsMenuCanva.interactable = false;
            optionsMenuCanva.blocksRaycasts = false;
        }
    }

    public void ShowOptionsMenu()
    {
        if (optionsMenuCanva == null)
        {
            optionsMenuCanva = GetComponent<CanvasGroup>();
        }
        else if (optionsMenuCanva.alpha == 1f) return;

        optionsMenuCanva.alpha = 1f;
        optionsMenuCanva.interactable = true;
        optionsMenuCanva.blocksRaycasts = true;

    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = GameManager.Instance.GetSavedResolutionIndex();

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        resolutionsDropdown.onValueChanged.AddListener(i =>
        {
            GameManager.Instance.SetResolution(i);
        });
    }

    private void SetupFullscreen()
    {
        fullscreenToggle.isOn = GameManager.Instance.GetSavedFullscreen();
        fullscreenToggle.onValueChanged.AddListener(b =>
        {
            GameManager.Instance.SetFullscreen(b);
        });
    }

    private void SetupBrightness()
    {
        brightnessSlider.value = GameManager.Instance.GetBrightness();
        brightnessSlider.onValueChanged.AddListener(v =>
        {
            GameManager.Instance.SetBrightness(v);
        });
    }

    private void ApplyVolume(string type, float value)
    {

        if (type == "Music")
            AudioManager.Instance.SetMusicVolume(value);
        else if (type == "Sfx")
            AudioManager.Instance.SetSfxVolume(value);

        PlayerPrefs.SetFloat($"{type}Volume", value);
    }

    public void SetUpVolume()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVol = PlayerPrefs.GetFloat("SfxVolume", 0.8f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        ApplyVolume("Music", musicVol);
        ApplyVolume("Sfx", sfxVol);

        musicSlider.onValueChanged.AddListener(v => ApplyVolume("Music", v));
        sfxSlider.onValueChanged.AddListener(v => ApplyVolume("Sfx", v));
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
