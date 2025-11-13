using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]
public class PlayerWaveController : MonoBehaviour
{
    [Header("Wave Parameters")]
    [Range(-5f, 5f)] public float amplitude = 0f;
    [Range(1f, 10f)] public float waveLength = 5f;
    public float speed = 5f;

    [Header("Colors")]
    public Gradient colorByWave;

    [Header("Snake Wave Settings")]
    [SerializeField] private float trailMaxLength = 10f;
    [SerializeField] private float minDistanceBetweenPoints = 0.1f;
    [SerializeField] private AnimationCurve alphaOverLength;

    [Header("Inertia Visual Feedback")]
    public float lineWidthBase = 0.4f;
    public float lineWidthExtraAtMaxInertia = 0.2f;
    public float lineMinAlphaAtMaxInertia = 0.6f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minMouseDistance = 0.4f;

    // Runtime
    private SpriteRenderer _sprite;
    private LineRenderer _line;
    private List<Vector3> _trail = new List<Vector3>();
    private float _inertiaBlend;
    private float _lastValidAngle;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line = GetComponent<LineRenderer>();

        _line.positionCount = 0;
        _line.alignment = LineAlignment.TransformZ;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.widthMultiplier = lineWidthBase;

        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.SetPixels(new Color[] { Color.white, Color.clear });
        dashTex.Apply();
        _line.material.mainTexture = dashTex;
        _line.textureMode = LineTextureMode.Tile;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
    }

    void Update()
    {
        UpdateRotation();

        // Movimento del player
        transform.position += transform.right * speed * Time.deltaTime;

        // Aggiornamento scia
        RecordTrailPoint();
        TrimTrailLength();
        UpdateSnakeWave();

        UpdateColors();
        ApplyInertiaVisuals();
    }

    // Rotazione verso il mouse con limite di distanza
    void UpdateRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 dir = worldMousePos - transform.position;
        float dist = dir.magnitude;

        // Se il mouse e troppo vicino evitiamo rotazioni instabili
        if (dist < minMouseDistance)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, _lastValidAngle);
            return;
        }

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);

        _lastValidAngle = angle;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Registra la posizione del player per creare la scia
    private void RecordTrailPoint()
    {
        Vector3 pos = transform.position;

        if (_trail.Count == 0 || Vector3.Distance(_trail[_trail.Count - 1], pos) > minDistanceBetweenPoints)
        {
            _trail.Add(pos);
        }
    }

    // Mantiene la scia entro la lunghezza stabilita
    private void TrimTrailLength()
    {
        if (_trail.Count < 2) return;

        float total = 0f;

        for (int i = _trail.Count - 1; i > 0; i--)
        {
            total += Vector3.Distance(_trail[i], _trail[i - 1]);

            if (total > trailMaxLength)
            {
                _trail.RemoveRange(0, i - 1);
                return;
            }
        }
    }

    // Disegna la scia come onda animata
    private void UpdateSnakeWave()
    {
        int count = _trail.Count;
        if (count < 2) return;

        _line.positionCount = count;

        float totalLength = 0f;

        for (int i = 0; i < count; i++)
        {
            Vector3 forward;
            if (i == count - 1)
                forward = (_trail[i] - _trail[i - 1]).normalized;
            else
                forward = (_trail[i + 1] - _trail[i]).normalized;

            Vector3 normal = new Vector3(-forward.y, forward.x, 0f);

            if (i > 0)
                totalLength += Vector3.Distance(_trail[i], _trail[i - 1]);

            // Onda animata
            float phase = ((totalLength / waveLength) + (Time.time * 0.5f)) * Mathf.PI * 2f;
            float offset = Mathf.Sin(phase) * amplitude;

            Vector3 finalPos = _trail[i] + normal * offset;
            _line.SetPosition(i, finalPos);

            float t = totalLength / trailMaxLength;
            float alpha = alphaOverLength.Evaluate(t);

            Color c = colorByWave.Evaluate(Mathf.InverseLerp(-5f, 5f, amplitude));
            c.a = alpha;

            _line.startColor = c;
            _line.endColor = c;
        }
    }

    // Aggiorna colore del player
    private void UpdateColors()
    {
        float f = Mathf.InverseLerp(-5f, 5f, amplitude);
        Color c = colorByWave.Evaluate(f);
        c.a = 1f;

        _sprite.color = c;
    }

    // Effetti visivi basati sull inerzia
    public void ApplyInertiaFeedback(float ampFactor, float waveFactor)
    {
        _inertiaBlend = Mathf.Clamp01((ampFactor + waveFactor) * 0.5f);
    }

    private void ApplyInertiaVisuals()
    {
        float width = lineWidthBase + lineWidthExtraAtMaxInertia * _inertiaBlend;
        _line.widthMultiplier = width;

        float alpha = Mathf.Lerp(1f, lineMinAlphaAtMaxInertia, _inertiaBlend);

        Color sc = _line.startColor; sc.a = alpha;
        Color ec = _line.endColor; ec.a = alpha;

        _line.startColor = sc;
        _line.endColor = ec;
    }

    public void SetAmplitude(float value)
    {
        amplitude = Mathf.Clamp(value, -5f, 5f);
    }
}
