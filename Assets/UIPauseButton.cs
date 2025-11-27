using UnityEngine;
using UnityEngine.UI;

public class UIPauseButton : MonoBehaviour
{
    private Button buttonComponent;
    [SerializeField] private CanvasGroup canvasGroup;

    void Awake()
    {
        buttonComponent = GetComponent<Button>();
    }

    void Start()
    {  
        buttonComponent.onClick.AddListener(TimeSetter.Instance.TogglePause);
        TimeSetter.OnGamePaused.AddListener(ShowCanvasGroup);
        TimeSetter.OnGameResumed.AddListener(HideCanvasGroup);
        HideCanvasGroup();
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
