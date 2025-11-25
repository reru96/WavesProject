using System;
using UnityEngine;

public enum ColorOverride
{
    None,
    Parry
}

[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]
public class PlayerWaveController : MonoBehaviour
{
    [Header("Wave Parameters")]
    [Range(-50f, 50f)] public float amplitude = 0f;
    [Range(1f, 10f)] public float waveLength = 5f;
    public float speed = 5f;

    [Header("Parry System")]
    public KeyCode parryKey = KeyCode.Space;
    public float parryDuration = 0.25f;
    public Color parryColor = Color.white;

    [Header("Line Preview")]
    public int previewPoints = 100;
    public float previewDistance = 10f;
    public float lineWidthBase = 0.4f;

    [Header("Runtime Smoothing")]
    public float wavelengthSlewPerSecond = 3f;

    [Header("Inertia Visual Feedback")]
    public float lineWidthExtraAtMaxInertia = 0.2f;
    public float lineMinAlphaAtMaxInertia = 0.6f;

    private SpriteRenderer _sprite;
    private LineRenderer _line;
    private WaveColorResolver _colorResolver;

    private float _baseY;
    private float _time;
    private float _effectiveWaveLength;
    private float _inertiaBlend;

    private float _parryTimer = 0f;
    private ColorOverride _currentColorOverride = ColorOverride.None;

    public ColorOverride CurrentColorOverride => _currentColorOverride;

    public ColorType CurrentWaveColor { get; private set; }

    public Color CurrentWaveUnityColor;

    public Action<ColorType> OnColorChanged;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line = GetComponent<LineRenderer>();
        _colorResolver = GetComponent<WaveColorResolver>();

        _colorResolver.OnResolvedColor += ReceiveResolvedColor;

        _baseY = transform.position.y;

        _line.positionCount = previewPoints;
        _line.alignment = LineAlignment.TransformZ;

        if (_line.material == null)
            _line.material = new Material(Shader.Find("Sprites/Default"));

        _line.widthMultiplier = lineWidthBase;

        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.SetPixels(new Color[] { Color.white, Color.clear });
        dashTex.Apply();

        _line.material.mainTexture = dashTex;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
        _line.textureMode = LineTextureMode.Tile;

        _effectiveWaveLength = waveLength;
    }

    void Update()
    {
        _time += Time.deltaTime * speed;

        _effectiveWaveLength = Mathf.MoveTowards(
            _effectiveWaveLength,
            waveLength,
            wavelengthSlewPerSecond * Time.deltaTime
        );

        UpdateVerticalMovement();
        HandleParryState();
        UpdateTrajectory();

        _colorResolver.ResolveColor(amplitude);

        UpdateVisuals();
    }

    void UpdateVerticalMovement()
    {
        float phase = (2f * Mathf.PI) * (_time / _effectiveWaveLength);
        float y = Mathf.Sin(phase) * amplitude;

        Vector3 pos = transform.position;
        pos.y = _baseY + y;
        transform.position = pos;
    }

    private void ReceiveResolvedColor(ColorType type, Color resolvedColor)
    {
        CurrentWaveColor = type;
        CurrentWaveUnityColor = resolvedColor;

        if (_currentColorOverride == ColorOverride.None)
        {
            OnColorChanged?.Invoke(type);
        }
    }

    void HandleParryState()
    {
        if (Input.GetKeyDown(parryKey) && _currentColorOverride == ColorOverride.None)
        {
            _parryTimer = parryDuration;
            SetColorOverride(ColorOverride.Parry);
            Debug.Log("Parry Attivato!");
        }

        if (_parryTimer > 0f)
        {
            _parryTimer -= Time.deltaTime;

            if (_parryTimer <= 0f)
            {
                SetColorOverride(ColorOverride.None);
                Debug.Log("Parry Scaduto.");
            }
        }
    }

    public void SetColorOverride(ColorOverride newColor)
    {
        _currentColorOverride = newColor;
    }

    public bool IsParryActive()
    {
        return _currentColorOverride == ColorOverride.Parry;
    }

    void UpdateTrajectory()
    {
        float halfDist = previewDistance * 0.5f;
        float baseX = transform.position.x;

        for (int i = 0; i < previewPoints; i++)
        {
            float tNorm = i / (float)(previewPoints - 1);
            float offset = Mathf.Lerp(-halfDist, +halfDist, tNorm);

            float xPos = baseX + offset;
            float t = _time + offset;

            float phase = (2f * Mathf.PI) * (t / _effectiveWaveLength);
            float yPos = _baseY + Mathf.Sin(phase) * amplitude;

            _line.SetPosition(i, new Vector3(xPos, yPos, 0f));
        }
    }

    void UpdateVisuals()
    {
        Color finalColor;

        if (_currentColorOverride == ColorOverride.Parry)
            finalColor = parryColor;
        else
            finalColor = CurrentWaveUnityColor;

        Color spriteColor = finalColor;
        spriteColor.a = 1f;
        _sprite.color = spriteColor;

        float targetWidth = lineWidthBase + lineWidthExtraAtMaxInertia * _inertiaBlend;
        float targetAlpha = Mathf.Lerp(1f, lineMinAlphaAtMaxInertia, _inertiaBlend);

        _line.widthMultiplier = targetWidth;

        Color lineColor = finalColor;
        lineColor.a = targetAlpha;

        _line.startColor = lineColor;
        _line.endColor = lineColor;
        _line.material.color = lineColor;
    }
    public void ApplyInertiaFeedback(float ampFactor, float waveFactor)
    {
        _inertiaBlend = Mathf.Clamp01((ampFactor + waveFactor) * 0.5f);
    }

    public void SetAmplitude(float value)
    {
        amplitude = Mathf.Clamp(value, -50f, 50f);
    }
}

[System.Serializable]
public struct WaveStateMapping
{
    public ColorType state;
    public float threshold;
}