using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup optionsMenu;
    [SerializeField] private CanvasGroup creditsMenu;
    [SerializeField] private CanvasGroup scoreMenu;
    [SerializeField] private CanvasGroup leaderboardPanel;

    [Header("Resolution, Fullscreen")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider;
    private Resolution[] resolutions;

    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider; 
    public List<TMP_Text> scoreTexts;

    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;



    private bool _isOnCredits = false;

    private void Start()
    {
        SetupResolutions();
        SetupFullscreen();
        SetupBrightness();
        SetUpVolume();
        ShowMainMenu();
        HideLeaderboard();
        UpdateLeaderboard();

    }

    private void Update()
    {
        if (_isOnCredits)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                ShowMainMenu();
            }
        }
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

    public void ShowMainMenu()
    {
        HideOptionsMenu();
        HideCreditsMenu();
        mainMenu.alpha = 1.0f;
        mainMenu.blocksRaycasts = true;
        mainMenu.interactable = true;
    }

    public void HideMainMenu()
    {
        if (mainMenu.alpha > 0)
        {
            mainMenu.alpha = 0;
            mainMenu.blocksRaycasts = false;
            mainMenu.interactable = false;
        }
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
    public void ShowScoreMenu()
    {
       scoreMenu.alpha = 1f;
       scoreMenu.blocksRaycasts = true;
       scoreMenu.interactable = true;
    }

    public void HideScoreMenu()
    {
      scoreMenu.alpha = 0f;
      scoreMenu.blocksRaycasts = false;
      scoreMenu.interactable = false;
    } 

    public void ShowCreditsMenu()
    {
        HideMainMenu();
        HideOptionsMenu();
        creditsMenu.alpha = 1f;
        creditsMenu.blocksRaycasts = true;
        creditsMenu.interactable = true;

        _isOnCredits = true;
    }

    public void HideCreditsMenu()
    {

        creditsMenu.alpha = 0f;
        creditsMenu.blocksRaycasts = false;
        creditsMenu.interactable = false;

        _isOnCredits = false;

    }

    public void NewGame()
    {
        SaveData newSave = new SaveData();
        SaveManager.Save(newSave);
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        SaveManager.Load();
        SceneManager.LoadScene("Level1");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void UpdateLeaderboard()
    {
        var scores = scoreManager.GetHighScores();

        for (int i = 0; i < scoreTexts.Count; i++)
        {
            if (i < scores.Count)
                scoreTexts[i].text = $"{i + 1}. {scores[i]}";
            else
                scoreTexts[i].text = $"{i + 1}. ---";
        }
    }

    public void ShowLeaderboard()
    {
        UpdateLeaderboard();

        leaderboardPanel.alpha = 1f;
        leaderboardPanel.blocksRaycasts = true;
        leaderboardPanel.interactable = true;
    }

    public void HideLeaderboard()
    {
        leaderboardPanel.alpha = 0f;
        leaderboardPanel.blocksRaycasts = false;
        leaderboardPanel.interactable = false;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Sorry, I can't quit on unity editor!");
#endif
        Application.Quit();
    }
}
