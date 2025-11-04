using UnityEngine;

// Niente fisica: seguo la curva con parametri chiari (deterministico = perfetto per puzzle).
public class BallOnWaveFromTimeline : MonoBehaviour
{
    public WaveTimelineData data;
    public float widthWorld = 20f;
    public float baselineY = 0f;
    public float travelSeconds = 6f;

    public bool IsPlaying { get; private set; }

    private float _t;

    public void ResetBall()
    {
        IsPlaying = false;
        _t = 0f;
        SetByT(0f);
    }

    public void Play()
    {
        IsPlaying = true;
        _t = 0f;
    }

    void Update()
    {
        if (!IsPlaying) return;

        _t += Time.deltaTime / Mathf.Max(0.01f, travelSeconds);
        if (_t >= 1f) { _t = 1f; IsPlaying = false; }

        SetByT(_t);
    }

    void SetByT(float tNorm)
    {
        float time = tNorm * data.totalLength;

        float amp = data.EvaluateAmplitude(time);
        float freq01 = data.EvaluateFrequency(time);
        float cycles = Mathf.Lerp(0.25f, 6f, freq01);
        float k = cycles * Mathf.PI * 2f / widthWorld;

        float x = Mathf.Lerp(-widthWorld * 0.5f, widthWorld * 0.5f, tNorm);
        float y = baselineY + amp * Mathf.Sin(x * k);

        transform.position = new Vector3(x, y, 0f);
    }
}
