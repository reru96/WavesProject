using UnityEngine;

// Prendo i key della timeline e li "traslo" nello spazio: tempo : X, amp/freq : sinusoide modulata.
[RequireComponent(typeof(LineRenderer))]
public class WaveRendererFromTimeline : MonoBehaviour
{
    public WaveTimelineData data;
    public float widthWorld = 20f;   // quanto "spazio" occupa la sequenza
    public float baselineY = 0f;     // quota centrale dell'onda
    public int samples = 1024;       // punti della Linea (più alto = più liscio)

    private LineRenderer _lr;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = samples;
        _lr.useWorldSpace = true;
        _lr.enabled = false; // nascosto finché non si fa Play
    }

    public void Hide() => _lr.enabled = false;

    public void DrawFromTimeline()
    {
        if (data == null) return;

        _lr.enabled = true;

        for (int i = 0; i < samples; i++)
        {
            float tNorm = (float)i / (samples - 1);          // 0..1
            float time = tNorm * data.totalLength;           // tempo nella timeline

            float amp = data.EvaluateAmplitude(time);        // 0..1
            float freq01 = data.EvaluateFrequency(time);     // 0..1
            float cycles = Mathf.Lerp(0.25f, 6f, freq01);    // densità "visiva" lungo la larghezza
            float k = cycles * Mathf.PI * 2f / widthWorld;   // fattore per sin

            float x = Mathf.Lerp(-widthWorld * 0.5f, widthWorld * 0.5f, tNorm);
            float y = baselineY + amp * Mathf.Sin(x * k);

            _lr.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
