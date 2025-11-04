using UnityEngine;

[RequireComponent(typeof(PlayerWaveController))]
public class PlayerControl : MonoBehaviour
{
    private PlayerWaveController _wave;

    [Header("Controls")]
    public float amplitudeStep = 0.5f;       // Intensità della variazione per click
    public float wavelengthRatePerSecond = 1.5f; // Velocità di variazione continua della wave

    [Header("Limits")]
    public float minAmplitude = -5f;
    public float maxAmplitude = 5f;
    public float minWavelength = 1f;
    public float maxWavelength = 10f;

    [Header("Smoothness")]
    public float amplitudeResponse = 10f;    // Quanto velocemente l’ampiezza si adatta

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
        HandleAmplitude();
        HandleWavelength();
    }

    void HandleAmplitude()
    {
        if (Input.GetKey(KeyCode.W))
            _targetAmplitude = Mathf.Clamp(_targetAmplitude - amplitudeStep, minAmplitude, maxAmplitude);

        if (Input.GetKey(KeyCode.S))
            _targetAmplitude = Mathf.Clamp(_targetAmplitude + amplitudeStep, minAmplitude, maxAmplitude);

        // Ampiezza: reattiva e diretta
        _wave.amplitude = Mathf.Lerp(_wave.amplitude, _targetAmplitude, amplitudeResponse * Time.deltaTime);
    }

    void HandleWavelength()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.A)) dir += 1f;
        if (Input.GetKey(KeyCode.D)) dir -= 1f;

        if (Mathf.Abs(dir) > 0f)
        {
            _targetWavelength = Mathf.Clamp(
                _targetWavelength + dir * wavelengthRatePerSecond * Time.deltaTime,
                minWavelength, maxWavelength
            );
        }

        // Aggiorna il target del controller (verrà smussato internamente)
        _wave.waveLength = _targetWavelength;
    }
}
