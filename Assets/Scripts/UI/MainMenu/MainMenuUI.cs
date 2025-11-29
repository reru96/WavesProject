using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using UnityEngine.InputSystem;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup optionsMenu;
    [SerializeField] private CanvasGroup creditsMenu;

   

    private bool _isOnCredits = false;

    private void Start()
    {
        ShowMainMenu();
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
        if (mainMenu != null && mainMenu.alpha > 0)
        {
            mainMenu.alpha = 0;
            mainMenu.blocksRaycasts = false;
            mainMenu.interactable = false;
        }
    }

    public void ShowOptionsMenu()
    {
        HideMainMenu();
        HideCreditsMenu();
        optionsMenu.alpha = 1f;
        optionsMenu.blocksRaycasts = true;
        optionsMenu.interactable = true;
    }

    public void HideOptionsMenu()
    {
        if (optionsMenu != null && optionsMenu.alpha > 0)
        {
            optionsMenu.alpha = 0f;
            optionsMenu.blocksRaycasts = false;
            optionsMenu.interactable = false;
        }
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
        if (creditsMenu != null && creditsMenu.alpha > 0)
        {
            creditsMenu.alpha = 0f;
            creditsMenu.blocksRaycasts = false;
            creditsMenu.interactable = false;

            _isOnCredits = false;
        }
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
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Sorry, I can't quit on unity editor!");
#endif
        Application.Quit();
    }

    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
