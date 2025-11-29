using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level1UI : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private CanvasGroup optionsMenu;

    [Header("Resolution, Fullscreen")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider;
    private Resolution[] resolutions;

    [Header("UI Sliders")]
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        HideOptionsMenu();
        SetupResolutions();
        SetupFullscreen();
        SetupBrightness();
        SetUpVolume();
    }

    public void SetupResolutions()
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

    public void SetupFullscreen()
    {
        fullscreenToggle.isOn = GameManager.Instance.GetSavedFullscreen();
        fullscreenToggle.onValueChanged.AddListener(b =>
        {
            GameManager.Instance.SetFullscreen(b);
        });
    }

    public void SetupBrightness()
    {
        brightnessSlider.value = GameManager.Instance.GetBrightness();
        brightnessSlider.onValueChanged.AddListener(v =>
        {
            GameManager.Instance.SetBrightness(v);
        });
    }

    public void ApplyVolume(string type, float value)
    {

        if (type == "Music")
            AudioManager.Instance.SetMusicVolume(value);
        else if (type == "Sfx")
            AudioManager.Instance.SetSfxVolume(value);

        PlayerPrefs.SetFloat($"{type}Volume", value);
    }

    public void SetUpVolume()
    {
        float sfxVol = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
        sfxSlider.value = sfxVol;
        ApplyVolume("Sfx", sfxVol);
        sfxSlider.onValueChanged.AddListener(v => ApplyVolume("Sfx", v));
    }


    public void ShowOptionsMenu()
    {
        optionsMenu.alpha = 1f;
        optionsMenu.blocksRaycasts = true;
        optionsMenu.interactable = true;
    }

    public void HideOptionsMenu()
    {
        optionsMenu.alpha = 0f;
        optionsMenu.blocksRaycasts = false;
        optionsMenu.interactable = false;
    }

    public void GoToMenu()
    {
        Debug.Log("Tasto premuto");
        SceneManager.LoadScene("StartMenu");
    }

    public void Retry()
    {
        SceneManager.LoadScene("Leve1");
    }  
}
