using UnityEngine;
using UnityEngine.Rendering.Universal;

// Generatore visivo: trasforma i keyframe in variazioni di luce (intensità, colore, emissione)
public class LightSynth : MonoBehaviour
{
    [Header("Riferimenti")]
    public WaveTimelineData data;
    public bool previewMode = true;

    [Header("Output")]
    public Light targetLight;              // luce classica (3D)
    public Light2D targetLight2D;          // opzionale per URP 2D
    public Renderer emissiveRenderer;      // opzionale (per materiale)
    public Color baseColor = Color.cyan;

    private float _time;

    void Update()
    {
        if (!previewMode || data == null) return;

        _time += Time.deltaTime;
        if (_time > data.totalLength) _time -= data.totalLength;

        // Valori interpolati
        float amp = data.EvaluateAmplitude(_time);
        float freq = data.EvaluateFrequency(_time);

        // Trasformo la "frequenza" in una velocità di pulsazione
        float oscillation = Mathf.Sin(Time.time * Mathf.Lerp(2f, 12f, freq));
        float intensity = Mathf.Clamp01(amp + 0.5f * oscillation);

        // Applicazione alla luce
        if (targetLight) targetLight.intensity = intensity * 5f;
        if (targetLight2D) targetLight2D.intensity = intensity * 3f;

        if (emissiveRenderer)
        {
            Color emissive = baseColor * intensity * 2f;
            emissiveRenderer.material.SetColor("_EmissionColor", emissive);
            DynamicGI.SetEmissive(emissiveRenderer, emissive);
        }
    }

    public void SetPreview(bool on)
    {
        previewMode = on;
        _time = 0f;
    }
}
