using UnityEngine;
using UnityEngine.UI;

public class UIPauseButton : MonoBehaviour
{
    private Button buttonComponent;
    [SerializeField] private CanvasGroup canvasGroup;

    void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnDisable()
    {

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveListener(TimeSetter.Instance.TogglePause);
        }

    }

    void Start()
    {
        if (TimeSetter.Instance != null)
        {
            buttonComponent.onClick.AddListener(TimeSetter.Instance.TogglePause);
            TimeSetter.OnGamePaused.AddListener(ShowCanvasGroup);
            TimeSetter.OnGameResumed.AddListener(HideCanvasGroup);
        }

    }

    private void ShowCanvasGroup()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideCanvasGroup()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        if (TimeSetter.Instance != null)
        {
            TimeSetter.OnGamePaused.RemoveListener(ShowCanvasGroup);
            TimeSetter.OnGameResumed.RemoveListener(HideCanvasGroup);
        }
    }
}
