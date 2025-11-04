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

    [Header("Line Preview")]
    public int previewPoints = 100;       // più punti = curva più liscia
    public float previewDistance = 10f;   // distanza totale visibile (metà avanti, metà dietro)
    public float lineWidth = 0.4f;

    [Header("Runtime Smoothing")]
    public float wavelengthSlewPerSecond = 3f;

    private SpriteRenderer _sprite;
    private LineRenderer _line;
    private Vector3 _startPos;
    private float _time;
    private float _effectiveWaveLength;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line = GetComponent<LineRenderer>();

        // Setup LineRenderer
        _line.positionCount = previewPoints;
        _line.alignment = LineAlignment.TransformZ;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.widthMultiplier = lineWidth;

        // Pattern tratteggiato
        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.SetPixels(new Color[] { Color.white, Color.clear });
        dashTex.Apply();
        _line.material.mainTexture = dashTex;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
        _line.textureMode = LineTextureMode.Tile;

        _startPos = transform.position;
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

        float phase = (2f * Mathf.PI) * (_time / _effectiveWaveLength);
        float x = _startPos.y + _time;
        float y = Mathf.Sin(phase) * amplitude;

        transform.position = new Vector3(x, y, 0f);

        UpdateTrajectory();
        UpdateColors();
    }

    void UpdateTrajectory()
    {
        int mid = previewPoints / 2; // punto centrale (player)
        float halfDist = previewDistance * 0.5f;

        for (int i = 0; i < previewPoints; i++)
        {
            // Offset da -halfDist (dietro) a +halfDist (davanti)
            float offset = Mathf.Lerp(-halfDist, halfDist, i / (float)(previewPoints - 1));

            float t = _time + offset;
            float phase = (2f * Mathf.PI) * (t / _effectiveWaveLength);
            float y = Mathf.Sin(phase) * amplitude;
            float x = _startPos.y + t;

            _line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void UpdateColors()
    {
        float colorFactor = Mathf.InverseLerp(-5f, 5f, amplitude);
        Color c = colorByWave.Evaluate(colorFactor);
        _sprite.color = c;
        _line.startColor = c;
        _line.endColor = c;
    }

    public void SetAmplitude(float value) => amplitude = Mathf.Clamp(value, -5f, 5f);
}
