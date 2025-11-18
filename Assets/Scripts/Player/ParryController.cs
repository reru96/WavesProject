using UnityEngine;

public class ParryController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private LineRenderer _line;
    public enum ColorOverride
    {
        None,
        Parry
    }

    [Header("Parry Settings")]
    public KeyCode parryKey = KeyCode.Space;
    public float parryDuration = 0.25f;
    public Color parryColor = Color.white;

    public Gradient colorByWave;
    public WaveStateMapping[] stateMappings;

    private float _parryTimer = 0f;
    private bool _isParryActive = false;
    private float _currentAmplitude = 0f; 

    public void SetCurrentAmplitude(float amplitude)
    {
        _currentAmplitude = amplitude;
    }

    public bool IsParryActive()
    {
        return _isParryActive;
    }

    void Update()
    {
        HandleParryInput();
        HandleParryStateTimer();
        UpdateColors(); 
    }

    void HandleParryInput()
    {
        if (Input.GetKeyDown(parryKey) && !_isParryActive)
        {
            _parryTimer = parryDuration;
            _isParryActive = true;
            Debug.Log("Parry Attivato!");
        }
    }

    void HandleParryStateTimer()
    {
        if (_parryTimer > 0f)
        {
            _parryTimer -= Time.deltaTime;

            if (_parryTimer <= 0f)
            {
                _isParryActive = false;
                Debug.Log("Parry Scaduto. Colore dinamico ripristinato.");
            }
        }
    }

    void UpdateColors()
    {
        Color c;

        if (_isParryActive)
        {
            c = parryColor;
        }
        else
        {
            float activeThreshold = 0f;
            float amplitude = Mathf.Clamp(Mathf.Abs(_currentAmplitude), 0f, 5f);
            foreach (var mapping in stateMappings)
            {
                if (amplitude >= mapping.threshold)
                {
                    activeThreshold = mapping.threshold;
                }
            }

            float colorFactor = Mathf.InverseLerp(0f, 5f, activeThreshold);
            c = colorByWave.Evaluate(colorFactor);
        }

        _sprite.color = c;

        Color sc = c; sc.a = _line.startColor.a;
        Color ec = c; ec.a = _line.endColor.a;
        _line.startColor = sc;
        _line.endColor = ec;
    }


    public void Initialize(SpriteRenderer sprite, LineRenderer line)
    {
        _sprite = sprite;
        _line = line;
    }
}
