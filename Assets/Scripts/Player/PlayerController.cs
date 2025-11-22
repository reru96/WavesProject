using UnityEngine;

[RequireComponent(typeof(PlayerWaveController))]
public class PlayerControl : MonoBehaviour
{
    private PlayerWaveController _wave;

    [Header("Movement")]
    [Tooltip("Velocità orizzontale del giocatore")]
    public float speed = 5f;

    [Header("Amplitude Controls (Feel)")]
    [Tooltip("Quanto velocemente sale l'onda quando premi W/S")]
    public float amplitudeSensitivity = 2.0f;
    [Tooltip("Quanto velocemente l'onda torna a 0 se lasci i tasti")]
    public float amplitudeDecay = 1.5f;

    [Header("Wavelength Controls")]
    public float wavelengthSensitivity = 2.0f;
    public float minWavelength = 1f;
    public float maxWavelength = 10f;
    [Range(0.1f, 3f)]public float wavelengthIncrease = 1f;

    [Header("Adaptive Difficulty")]
    [Tooltip("Riduce la sensibilità quando l'ampiezza è alta (più difficile manovrare onde grandi)")]
    public float inertiaFactor = 0.2f;

    [Header("Limits")]
    public float minAmplitude = -5f;
    public float maxAmplitude = 5f;
    [Range(0, 1.1f)]public float standardwave = 0.3f;

    void Start()
    {
        _wave = GetComponent<PlayerWaveController>();
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        HandleAmplitudePhysics();
        HandleWavelength();
        ApplyInertiaFeedback();
    }

    private void HandleAmplitudePhysics()
    {
        bool isPressing = false;

        float currentInertia = 1f + (Mathf.Abs(_wave.amplitude) * inertiaFactor);
        float effectiveSensitivity = amplitudeSensitivity / currentInertia;

        if (Input.GetKey(KeyCode.W))
        {
            _wave.amplitude += effectiveSensitivity * Time.deltaTime;
            isPressing = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _wave.amplitude -= effectiveSensitivity * Time.deltaTime;
            isPressing = true;
        }

        if (!isPressing)
        {
            float targetAmplitude = (_wave.amplitude >= 0) ? Mathf.Abs(standardwave) : -Mathf.Abs(standardwave);
            _wave.amplitude = Mathf.MoveTowards(_wave.amplitude, targetAmplitude, amplitudeDecay * Time.deltaTime);
        }
        _wave.amplitude = Mathf.Clamp(_wave.amplitude, minAmplitude, maxAmplitude);
    }

    private void HandleWavelength()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.D)) dir += wavelengthIncrease; // D allarga l'onda
        if (Input.GetKey(KeyCode.A)) dir -= wavelengthIncrease; // A stringe l'onda

        if (Mathf.Abs(dir) > 0f)
        {
            _wave.waveLength += dir * wavelengthSensitivity * Time.deltaTime;
            _wave.waveLength = Mathf.Clamp(_wave.waveLength, minWavelength, maxWavelength);
        }
    }

    private void ApplyInertiaFeedback()
    {
        float maxAmpAbs = Mathf.Max(Mathf.Abs(minAmplitude), Mathf.Abs(maxAmplitude));
        float ampFactor = Mathf.InverseLerp(0f, maxAmpAbs, Mathf.Abs(_wave.amplitude));
        float waveFactor = Mathf.InverseLerp(minWavelength, maxWavelength, _wave.waveLength);

        _wave.ApplyInertiaFeedback(ampFactor, waveFactor);
    }
}