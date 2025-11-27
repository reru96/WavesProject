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
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.OnGameOver += DisableButton;
        }
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.OnGameOver -= DisableButton;
        }

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

        HideCanvasGroup();
    }

    public void DisableButton()
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("UIPauseButton: CanvasGroup è stato distrutto (probabilmente al Game Over). Impossibile disabilitare il bottone.");
            return;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
