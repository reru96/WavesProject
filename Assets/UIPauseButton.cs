using UnityEngine;
using UnityEngine.UI;

public class UIPauseButton : MonoBehaviour
{
    private Button buttonComponent;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup buttoncanvas;

    void Awake()
    {
        buttonComponent = GetComponent<Button>();
    }

    private void OnEnable()
    {
        RespawnManager.OnGameOver += DisableButton;
    }

    private void OnDisable()
    {
        RespawnManager.OnGameOver += DisableButton;
    }

    void Start()
    {  
        buttonComponent.onClick.AddListener(TimeSetter.Instance.TogglePause);
        TimeSetter.OnGamePaused.AddListener(ShowCanvasGroup);
        TimeSetter.OnGameResumed.AddListener(HideCanvasGroup);
        HideCanvasGroup();
    }

    public void DisableButton()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void ShowCanvasGroup()
    {
      canvasGroup.alpha = 1f;
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    }
    private void HideCanvasGroup()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        TimeSetter.OnGamePaused.RemoveListener(ShowCanvasGroup);
        TimeSetter.OnGameResumed.RemoveListener(HideCanvasGroup);
    }
}
