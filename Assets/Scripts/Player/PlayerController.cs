using UnityEngine;

[RequireComponent(typeof(PlayerWaveController))]
public class PlayerControl : MonoBehaviour
{
    private PlayerWaveController _wave;

    public float inputThreshold = 0.1f;

    [Header("Controls (Base Rates)")]
    [Tooltip("Variazione base per l'ampiezza ad ogni 'step' di input")]
    public float baseAmplitudeStep = 0.6f;
    [Tooltip("Velocità base di variazione della lunghezza d'onda (unità al secondo)")]
    public float baseWavelengthRate = 0.6f;

    [Header("Adaptive Sensitivity")]
    [Tooltip("Riduce la sensibilità quando l'ampiezza cresce")]
    public float amplitudeInertia = 0.2f;
    [Tooltip("Riduce la sensibilità quando la lunghezza cresce")]
    public float wavelengthInertia = 0.15f;

    [Header("Limits")]
    public float minAmplitude = -5f;
    public float maxAmplitude = 5f;
    public float minWavelength = 1f;
    public float maxWavelength = 10f;

    [Header("Smoothness")]
    [Tooltip("Reattività con cui l'ampiezza raggiunge il target")]
    public float amplitudeResponse = 10f;

    private float _targetAmplitude;
    private float _targetWavelength;


    void Start()
    {
        _wave = GetComponent<PlayerWaveController>();
        _targetAmplitude  = _wave.amplitude;
        _targetWavelength = _wave.waveLength;
    }

    void Update()
    {
        HandleAmplitude();
        HandleWavelength();

        // ---- FEEDBACK VISIVO DELL’INERZIA ----
        // fattore 0..1 per quanta “pesantezza” abbiamo ora
        float maxAmpAbs = Mathf.Max(Mathf.Abs(minAmplitude), Mathf.Abs(maxAmplitude));
        float ampInertiaFactor  = Mathf.InverseLerp(0f, maxAmpAbs, Mathf.Abs(_wave.amplitude));
        float waveInertiaFactor = Mathf.InverseLerp(minWavelength, maxWavelength, _wave.waveLength);

        _wave.ApplyInertiaFeedback(ampInertiaFactor, waveInertiaFactor);
    }

    void HandleAmplitude()
    {
        // Sensibilità adattiva (più ampiezza ⇒ meno sensibilità)
        float sensitivity = baseAmplitudeStep / (1f + Mathf.Abs(_wave.amplitude) * amplitudeInertia);

        if ((Input.mousePosition.y - transform.position.y) > inputThreshold) 
            _targetAmplitude = Mathf.Clamp(_targetAmplitude - sensitivity, minAmplitude, maxAmplitude);

        if ((Input.mousePosition.y - transform.position.y) < -inputThreshold)
            _targetAmplitude = Mathf.Clamp(_targetAmplitude + sensitivity, minAmplitude, maxAmplitude);

        // Ampiezza reattiva ma morbida
        _wave.amplitude = Mathf.Lerp(_wave.amplitude, _targetAmplitude, amplitudeResponse * Time.deltaTime);
    }

    void HandleWavelength()
    {
        // Sensibilità adattiva (più lunghezza ⇒ meno sensibilità)
        float sensitivity = baseWavelengthRate / (1f + (_wave.waveLength - 1f) * wavelengthInertia);

        float dir = 0f;
        if (Input.mousePosition.x - transform.position.x > inputThreshold) dir += 1f;
        if (Input.mousePosition.x - transform.position.x < -inputThreshold) dir -= 1f;

        if (Mathf.Abs(dir) > 0f)
        {
            _targetWavelength = Mathf.Clamp(
                _targetWavelength + dir * sensitivity * Time.deltaTime,
                minWavelength, maxWavelength
            );
        }

        // Il controller interno farà lo "slew" lineare (vedi PlayerWaveController)
        _wave.waveLength = _targetWavelength;
    }
}
