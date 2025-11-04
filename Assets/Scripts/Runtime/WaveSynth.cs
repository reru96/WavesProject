using UnityEngine;

// Generatore audio sincronizzato con WaveTimelineData
[RequireComponent(typeof(AudioSource))]
public class WaveSynth : MonoBehaviour
{
    [Header("Riferimenti")]
    public WaveTimelineData data;          // timeline con key di ampiezza/frequenza
    public bool previewMode = true;        // se false: silenzio
    [Range(0f, 1f)] public float masterVolume = 0.3f;

    private double _phase;
    private double _sampleRate;
    private float _time;

    void Awake()
    {
        var src = GetComponent<AudioSource>();
        src.playOnAwake = true;
        src.loop = true;
        src.spatialBlend = 0f;
        src.volume = 1f;
        src.minDistance = 0.1f;
        src.maxDistance = 1f;

        _sampleRate = AudioSettings.outputSampleRate;
    }

    private void OnAudioFilterRead(float[] dataBuffer, int channels)
    {
        if (data == null || !previewMode)
        {
            for (int i = 0; i < dataBuffer.Length; i++) dataBuffer[i] = 0f;
            return;
        }

        for (int i = 0; i < dataBuffer.Length; i += channels)
        {
            // tempo relativo nella timeline
            _time += 1f / (float)_sampleRate;
            if (_time > data.totalLength)
                _time -= data.totalLength;

            // valori interpolati
            float amp = data.EvaluateAmplitude(_time);         // 0..1
            float freq01 = data.EvaluateFrequency(_time);      // 0..1 → mappa Hz
            float hz = Mathf.Lerp(110f, 880f, freq01);

            double increment = 2.0 * Mathf.PI * hz / _sampleRate;
            float sample = Mathf.Sin((float)_phase) * amp * masterVolume;

            for (int c = 0; c < channels; c++)
                dataBuffer[i + c] = sample;

            _phase += increment;
            if (_phase > Mathf.PI * 2.0)
                _phase -= Mathf.PI * 2.0;
        }
    }

    public void SetPreview(bool on)
    {
        previewMode = on;
        _phase = 0f;
        _time = 0f;
    }
}
