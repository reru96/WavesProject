using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]
public class PlayerWaveController : MonoBehaviour
{

    [Header("Wave Parameters")]
    [Range(-5f, 5f)] public float amplitude = 0f;
    [Range(1f, 10f)] public float waveLength = 5f;
    public float speed = 5f; // velocità di propagazione della testa

    [Header("Colors")]
    public Gradient colorByWave;

    [Header("Line Preview")]
    public int previewPoints = 100;
    public float previewDistance = 10f;
    public float lineWidthBase = 0.4f;

    [Header("Runtime Smoothing")]
    public float wavelengthSlewPerSecond = 3f;

    [Header("Inertia Visual Feedback")]
    public float lineWidthExtraAtMaxInertia = 0.2f;
    public float lineMinAlphaAtMaxInertia = 0.6f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Tail Follow")]
    public float tailFollowSpeed = 5f;

    [Header("Amplitude Control")]
    public float amplitudeStep = 0.2f;
    public float amplitudeLerpSpeed = 5f;
    public float minAmplitude = 0f;
    public float maxAmplitude = 5f;

    private SpriteRenderer _sprite;
    private LineRenderer _line;
    private Vector3 _fixedPlayerPos; // posizione della coda (player)
    private Vector3 _waveHeadPos;    // posizione della testa
    private Vector3 _waveDir = Vector3.right;
    private Vector3 _waveUp = Vector3.up;
    private float _time;
    private float _effectiveWaveLength;
    private float _targetAmplitude;
    private float _inertiaBlend;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line = GetComponent<LineRenderer>();

        _fixedPlayerPos = transform.position;
        _waveHeadPos = transform.position;

        _line.positionCount = previewPoints;
        _line.alignment = LineAlignment.TransformZ;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.widthMultiplier = lineWidthBase;

        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.SetPixels(new Color[] { Color.white, Color.clear });
        dashTex.Apply();
        _line.material.mainTexture = dashTex;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
        _line.textureMode = LineTextureMode.Tile;

        _effectiveWaveLength = waveLength;
        _targetAmplitude = amplitude;
    }

    void Update()
    {
        UpdateRotation();
        UpdateAmplitude();
        MoveHead();
        MoveTail();
        UpdateTrajectory();
        UpdateColors();
        ApplyInertiaVisuals();
    }

    void UpdateRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 direction = (worldMousePos - _waveHeadPos).normalized;
        _waveDir = Vector3.Lerp(_waveDir, direction, rotationSpeed * Time.deltaTime);
        _waveUp = new Vector3(-_waveDir.y, _waveDir.x, 0f); // perpendicolare alla direzione
    }

    void UpdateAmplitude()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
            _targetAmplitude = Mathf.Clamp(_targetAmplitude + scroll * amplitudeStep, minAmplitude, maxAmplitude);

        amplitude = Mathf.Lerp(amplitude, _targetAmplitude, amplitudeLerpSpeed * Time.deltaTime);
    }

    void MoveHead()
    {
        _waveHeadPos += _waveDir * speed * Time.deltaTime;
        _effectiveWaveLength = Mathf.MoveTowards(_effectiveWaveLength, waveLength, wavelengthSlewPerSecond * Time.deltaTime);
        _time += Time.deltaTime * speed;
    }

    void MoveTail()
    {
        // la coda segue la testa
        _fixedPlayerPos = Vector3.Lerp(_fixedPlayerPos, _waveHeadPos, tailFollowSpeed * Time.deltaTime);

        // oscillazione verticale per la coda
        float phase = (2f * Mathf.PI) * (_time / _effectiveWaveLength);
        float y = Mathf.Sin(phase) * amplitude;
        transform.position = _fixedPlayerPos + _waveUp * y;
    }

    void UpdateTrajectory()
    {
        for (int i = 0; i < previewPoints; i++)
        {
            float tNorm = i / (float)(previewPoints - 1);
            float offset = Mathf.Lerp(previewDistance, 0, tNorm);

            Vector3 point = _fixedPlayerPos
                            + _waveDir * offset
                            + _waveUp * Mathf.Sin((_time + offset) / _effectiveWaveLength * 2f * Mathf.PI) * amplitude;

            _line.SetPosition(i, point);
        }
    }

    void UpdateColors()
    {
        float colorFactor = Mathf.InverseLerp(minAmplitude, maxAmplitude, amplitude);
        Color c = colorByWave.Evaluate(colorFactor);
        c.a = 1f;

        if (_sprite != null) _sprite.color = c;
        _line.startColor = c;
        _line.endColor = c;
    }

    public void ApplyInertiaFeedback(float ampFactor, float waveFactor)
    {
        _inertiaBlend = Mathf.Clamp01((ampFactor + waveFactor) * 0.5f);
    }

    void ApplyInertiaVisuals()
    {
        float width = lineWidthBase + lineWidthExtraAtMaxInertia * _inertiaBlend;
        _line.widthMultiplier = width;

        float alpha = Mathf.Lerp(1f, lineMinAlphaAtMaxInertia, _inertiaBlend);

        Color sc = _line.startColor; sc.a = alpha;
        Color ec = _line.endColor; ec.a = alpha;
        _line.startColor = sc;
        _line.endColor = ec;
    }

    public void SetAmplitude(float value) => amplitude = Mathf.Clamp(value, -5f, 5f);
}
