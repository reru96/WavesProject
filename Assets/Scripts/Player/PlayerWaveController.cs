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
        private Vector3 _fixedPlayerPos;
        private float _time;
        private float _effectiveWaveLength;
        private float _inertiaBlend;

        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _line = GetComponent<LineRenderer>();

            _fixedPlayerPos = transform.position;

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
            float y = Mathf.Sin(phase) * amplitude;

            transform.position = new Vector3(_fixedPlayerPos.x, _fixedPlayerPos.y + y, _fixedPlayerPos.z);

            UpdateTrajectory();
            UpdateColors();
            ApplyInertiaVisuals();
        }

        void UpdateTrajectory()
        {
        float halfDist = previewDistance * 0.5f;

        for (int i = 0; i < previewPoints; i++)
        {
            float tNorm = i / (float)(previewPoints - 1);
            float offset = Mathf.Lerp(-halfDist, +halfDist, tNorm);

            float xPos = _fixedPlayerPos.x + offset; 
            float t = _time + offset;
            float phase = (2f * Mathf.PI) * (t / _effectiveWaveLength);
            float yPos = _fixedPlayerPos.y + Mathf.Sin(phase) * amplitude;

            _line.SetPosition(i, new Vector3(xPos, yPos, 0f));
        }
    }

        void UpdateColors()
        {
            float colorFactor = Mathf.InverseLerp(-5f, 5f, amplitude);
            Color c = colorByWave.Evaluate(colorFactor);
            c.a = 1f;

            _sprite.color = c;
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
