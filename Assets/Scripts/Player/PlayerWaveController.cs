using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct WaveColorDefinition
{
    public string name;       
    public ColorType type;     
    public float minAmplitude; 
    public Color color;        
}

[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]
public class PlayerWaveController : MonoBehaviour
{
    [Header("Wave Parameters")]
    [Range(-50f, 50f)] public float amplitude = 0f;
    [Range(1f, 10f)] public float waveLength = 5f;
    public float speed = 5f;
    public float wavelengthSlewPerSecond = 3f;

    public string colorPropertyName = "_Color";

    [Header("Parry System")]
    public KeyCode parryKey = KeyCode.Space;
    public float parryDuration = 0.25f;
    public Color parryColor = Color.white;

    [Header("Color Mapping")]
    public List<WaveColorDefinition> colorDefinitions;

    [Header("Line Preview")]
    public int previewPoints = 100;
    public float previewDistance = 10f;
    public float lineWidthBase = 0.4f;
    private float _inertiaBlend;
    private SpriteRenderer _sprite;
    private LineRenderer _line;
    private float _baseY;
    private float _time;
    private float _effectiveWaveLength;
    private float _parryTimer = 0f;
    private bool _isParrying = false;
    private ColorType _currentType;
    public Action<ColorType> OnColorChanged; 

    public float lineWidthExtraAtMaxInertia = 0.2f;
    public float lineMinAlphaAtMaxInertia = 0.6f;
    private MaterialPropertyBlock _mpb;
    public ColorType CurrentColorType => _currentType;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line = GetComponent<LineRenderer>();
        _mpb = new MaterialPropertyBlock();
        _baseY = transform.position.y;
        _line.positionCount = previewPoints;
        _line.useWorldSpace = true;
        _line.widthMultiplier = lineWidthBase;
        SetupDashedLineTexture();
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
        HandleParryInput();
        UpdateColorLogic();
        DrawTrajectory();
    }

    void SetupDashedLineTexture()
    {
        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.filterMode = FilterMode.Point;
        dashTex.SetPixels(new Color[] { Color.white, Color.clear });
        dashTex.Apply();

        _line.material.mainTexture = dashTex;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
        _line.textureMode = LineTextureMode.Tile;
    }

    void UpdateVerticalMovement()
    {
        float phase = (2f * Mathf.PI) * (_time / _effectiveWaveLength);
        float y = Mathf.Sin(phase) * amplitude;

        Vector3 pos = transform.position;
        pos.y = _baseY + y;
        transform.position = pos;
    }

    void HandleParryInput()
    {
        if (Input.GetKeyDown(parryKey) && !_isParrying)
        {
            _parryTimer = parryDuration;
            _isParrying = true;
            Debug.Log("parry attivo");
        }

        if (_isParrying)
        {
            _parryTimer -= Time.deltaTime;
            if (_parryTimer <= 0f)
            {
                _isParrying = false;
            }
        }
    }

    void UpdateColorLogic()
    {
        Color targetColor = Color.white;
        ColorType targetType = ColorType.White;

        if (_isParrying)
        {
            targetColor = parryColor;
            targetType = ColorType.White;
        }
        else
        {
            float currentAbsAmp = Mathf.Abs(amplitude);
            var match = colorDefinitions
                .OrderByDescending(x => x.minAmplitude)
                .FirstOrDefault(x => currentAbsAmp >= x.minAmplitude);

            if (!string.IsNullOrEmpty(match.name))
            {
                targetColor = match.color;
                targetType = match.type;
            }
        }

        float targetWidth = lineWidthBase + (lineWidthExtraAtMaxInertia * _inertiaBlend);
        _line.widthMultiplier = targetWidth;

        float targetAlpha = Mathf.Lerp(1f, lineMinAlphaAtMaxInertia, _inertiaBlend);

        Color finalColor = targetColor;
        finalColor.a = targetAlpha;
        _sprite.GetPropertyBlock(_mpb);
        _mpb.SetColor(colorPropertyName, targetColor);
        _sprite.SetPropertyBlock(_mpb);
        _line.startColor = finalColor;
        _line.endColor = finalColor;

        if (_currentType != targetType)
        {
            _currentType = targetType;
            OnColorChanged?.Invoke(_currentType);
        }
    }

    void DrawTrajectory()
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

    public void ApplyInertiaFeedback(float ampFactor, float waveFactor)
    {
        _inertiaBlend = Mathf.Clamp01((ampFactor + waveFactor) * 0.5f);
    }
}