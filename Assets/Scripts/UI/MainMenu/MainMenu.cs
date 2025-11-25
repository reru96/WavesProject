using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string mainLvlName = "mainGame";
    public string creditLvlName = "Credits";

    [SerializeField] private CanvasGroup mainMenuCanvaGroup;
    [SerializeField] private CanvasGroup optionsMenuCanvaGroup;

    public void SwapUi()
    {
        if (mainMenuCanvaGroup.interactable)
        {
            OpenMainMenu();
        }
        else
        {
            OpenOptionsMenu();
        }
    }

    private void OpenMainMenu()
    {
        if (optionsMenuCanvaGroup.interactable)
        {
            optionsMenuCanvaGroup.interactable = false;
            optionsMenuCanvaGroup.blocksRaycasts = false;
            optionsMenuCanvaGroup.alpha = 0f;
        }
        mainMenuCanvaGroup.interactable = true;
        mainMenuCanvaGroup.blocksRaycasts = true;
        mainMenuCanvaGroup.alpha = 1f;
    }

    private void OpenOptionsMenu()
    {
        if (optionsMenuCanvaGroup.interactable)
        {
            mainMenuCanvaGroup.interactable = false;
            mainMenuCanvaGroup.blocksRaycasts = false;
            mainMenuCanvaGroup.alpha = 0f;
        }
        optionsMenuCanvaGroup.interactable = true;
        optionsMenuCanvaGroup.blocksRaycasts = true;
        optionsMenuCanvaGroup.alpha = 1f;
    }
    public void LoadMainLevel()
    {
        SceneManager.LoadScene(mainLvlName);
    }
    public void LoadCreditLevel()
    {
        SceneManager.LoadScene(creditLvlName);
    }
    public void QuitGame() 
    {
#if UNITY_EDITOR
        Debug.Log("Cannot quit while in editor");
            #endif
            Application.Quit();
    }

}
