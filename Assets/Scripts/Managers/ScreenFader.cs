using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private bool useEasing = true;

    private Coroutine currentFade;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetFade(1f);
            FadeIn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartFade(1f, onComplete);
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartFade(0f, onComplete);
    }

    public void FadeToScene(string sceneName)
    {
        FadeOut(() =>
        {
            SceneManager.LoadScene(sceneName);
            FadeIn();
        });
    }


    public void SetFade(float alpha)
    {
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, Mathf.Clamp01(alpha));
    }

    private void StartFade(float targetAlpha, System.Action onComplete)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeRoutine(targetAlpha, onComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, System.Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);

            if (useEasing)
                t = t * t * (3f - 2f * t);

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha);
        onComplete?.Invoke();
        currentFade = null;
    }
}
