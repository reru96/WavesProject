using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeSetter : MonoBehaviour
{
    public static TimeSetter Instance { get; private set; }
    public static UnityEvent OnGamePaused = new UnityEvent();
    public static UnityEvent OnGameResumed = new UnityEvent();
    private bool isPaused = false;
    private Coroutine slowMotion;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            SetPause(true);
        }
        else
        {
            SetPause(false);
        }
    }

    private void SetPause(bool pauseState)
    {
        if (pauseState)
        {
            Time.timeScale = 0f;
            OnGamePaused.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            OnGameResumed.Invoke();
        }
    }

    public void SlowMotionForImpact(float impactTime = 0.05f, float slowScale = 0.1f)
    {
        if (isPaused)
            return;

        if (slowMotion != null)
        {
            StopCoroutine(slowMotion);
            Time.timeScale = 1f; 
        }

        slowMotion = StartCoroutine(DoSlowMotion(impactTime, slowScale));
    }

    private IEnumerator DoSlowMotion(float impactTime, float slowScale)
    {
        Time.timeScale = slowScale;

        float timer = 0f;
        while (timer < impactTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        slowMotion = null;
    }
}
