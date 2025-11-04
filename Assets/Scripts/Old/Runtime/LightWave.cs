using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class LightWave : MonoBehaviour
{
    [Header("References")]
    public Light light3D;
    public Light2D light2D;
    public LineRenderer lineRenderer;

    [Header("Parameters")]
    [Range(0f, 5f)] public float intensity = 1f;
    [Range(0f, 5f)] public float frequency = 1f;
    public Color color = Color.cyan;
    public float width = 10f;
    public int points = 256;

    [Header("Runtime")]
    public bool isPlaying = false;
    public static System.Action<LightWave> OnLightSelected;

    private float _time;
    private bool _isSelected = false;
    private Color _baseColor;

    void Start()
    {
        if (lineRenderer)
        {
            lineRenderer.positionCount = points;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = color;
        }

        _baseColor = color;
    }

    void Update()
    {
        if (isPlaying)
        {
            _time += Time.deltaTime * frequency * 2f;
            DrawWave();
            AnimateLight();
        }
        else
            DrawFlat();
    }

    void DrawWave()
    {
        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);
            float x = (t - 0.5f) * width;
            float y = Mathf.Sin((t * Mathf.PI * 2f * frequency) + _time) * intensity;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void DrawFlat()
    {
        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);
            float x = (t - 0.5f) * width;
            lineRenderer.SetPosition(i, new Vector3(x, 0f, 0f));
        }
    }

    void AnimateLight()
    {
        float pulse = Mathf.Sin(Time.time * frequency * 2f) * 0.5f + 0.5f;
        float currentIntensity = intensity * (0.5f + pulse);

        if (light3D)
        {
            light3D.color = _isSelected ? Color.white : color;
            light3D.intensity = currentIntensity * (_isSelected ? 5f : 3f);
        }

        if (light2D)
        {
            light2D.color = _isSelected ? Color.white : color;
            light2D.intensity = currentIntensity * (_isSelected ? 3f : 2f);
        }

        if (lineRenderer)
            lineRenderer.material.color = _isSelected ? Color.Lerp(color, Color.white, 0.5f) : color;
    }

    public void SetIntensity(float val) => intensity = val;
    public void SetFrequency(float val) => frequency = val;
    public void SetPlay(bool play) { isPlaying = play; _time = 0f; }

    public void SetSelected(bool sel)
    {
        _isSelected = sel;
    }

    private void OnMouseDown()
    {
        OnLightSelected?.Invoke(this);
    }
}
