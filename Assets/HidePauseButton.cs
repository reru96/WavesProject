using UnityEngine;

public class HidePauseButton : MonoBehaviour
{
    public CanvasGroup pauseButton;

    public void Start()
    {
        Time.timeScale = 1.0f;
        ShowButton();
    }
    public void OnEnable()
    {
        RespawnManager.OnGameOver += HideButton;
    }

    public void OnDisable()
    {
        RespawnManager.OnGameOver -= HideButton;
    }
    public void HideButton()
    {
        pauseButton.alpha = 0f;
        pauseButton.interactable = false;
        pauseButton.blocksRaycasts = false;
    }

    public void ShowButton()
    {
        pauseButton.alpha = 1f;
        pauseButton.interactable = true;
        pauseButton.blocksRaycasts = true;
    }


}
