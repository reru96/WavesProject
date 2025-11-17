using UnityEngine;

[RequireComponent(typeof(PlayerWaveController))]
public class PlayerControl : MonoBehaviour
{
    private PlayerWaveController _wave;

    [Header("Movement")]

    [Tooltip("Velocità orizzontale del giocatore")]
    public float speed = 5f;

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
        _targetAmplitude = _wave.amplitude;
        _targetWavelength = _wave.waveLength;
    }

    void Update()
    {

        transform.position += Vector3.right * speed * Time.deltaTime;
        HandleAmplitude();
        HandleWavelength();
        ApplyInertiaFeedback();
    }

    private void HandleAmplitude()
    {
        float sensitivity = baseAmplitudeStep / (1f + Mathf.Abs(_wave.amplitude) * amplitudeInertia);

        if (Input.GetKey(KeyCode.W))
            _targetAmplitude = Mathf.Clamp(_targetAmplitude - sensitivity, minAmplitude, maxAmplitude);

        if (Input.GetKey(KeyCode.S))
            _targetAmplitude = Mathf.Clamp(_targetAmplitude + sensitivity, minAmplitude, maxAmplitude);

        _wave.amplitude = Mathf.Lerp(_wave.amplitude, _targetAmplitude, amplitudeResponse * Time.deltaTime);
    }

    private void HandleWavelength()
    {
        float sensitivity = baseWavelengthRate / (1f + (_wave.waveLength - 1f) * wavelengthInertia);

        float dir = 0f;
        if (Input.GetKey(KeyCode.A)) dir += 1f;
        if (Input.GetKey(KeyCode.D)) dir -= 1f;

        if (Mathf.Abs(dir) > 0f)
        {
            _targetWavelength = Mathf.Clamp(
                _targetWavelength + dir * sensitivity * Time.deltaTime,
                minWavelength, maxWavelength
            );
        }

        _wave.waveLength = _targetWavelength;
    }

    private void ApplyInertiaFeedback()
    {
        float maxAmpAbs = Mathf.Max(Mathf.Abs(minAmplitude), Mathf.Abs(maxAmplitude));
        float ampFactor = Mathf.InverseLerp(0f, maxAmpAbs, Mathf.Abs(_wave.amplitude));
        float waveFactor = Mathf.InverseLerp(minWavelength, maxWavelength, _wave.waveLength);

        _wave.ApplyInertiaFeedback(ampFactor, waveFactor);
    }
}
