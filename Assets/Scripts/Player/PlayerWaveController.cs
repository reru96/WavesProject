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
    public int   previewPoints   = 100;    // più punti = più liscia
    public float previewDistance = 10f;    // metà dietro + metà davanti
    public float lineWidthBase   = 0.4f;   // spessore base

    [Header("Runtime Smoothing")]
    public float wavelengthSlewPerSecond = 3f; // velocità con cui l'effettivo segue il target

    // Feedback visivo dell'inerzia
    [Header("Inertia Visual Feedback")]
    [Tooltip("Quanto la linea può diventare più spessa a massima inerzia")]
    public float lineWidthExtraAtMaxInertia = 0.2f;
    [Tooltip("Alpha minimo della linea a massima inerzia (0..1)")]
    public float lineMinAlphaAtMaxInertia   = 0.6f;

<<<<<<< Updated upstream
    private SpriteRenderer _sprite;
    private LineRenderer   _line;
    private Vector3 _startPos;
    private float   _time;
    private float   _effectiveWaveLength; // valore usato davvero per la sinusoide
    private float   _inertiaBlend;        // 0..1 media tra ampiezza/lunghezza (per feedback)

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _line   = GetComponent<LineRenderer>();
=======


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
>>>>>>> Stashed changes

        // Setup LineRenderer (pattern tratteggiato)
        _line.positionCount = previewPoints;
        _line.alignment     = LineAlignment.TransformZ;
        _line.material      = new Material(Shader.Find("Sprites/Default"));
        _line.widthMultiplier = lineWidthBase;

        Texture2D dashTex = new Texture2D(2, 1);
        dashTex.SetPixels(new UnityEngine.Color[] { UnityEngine.Color.white, UnityEngine.Color.clear });
        dashTex.Apply();
        _line.material.mainTexture      = dashTex;
        _line.material.mainTextureScale = new Vector2(10f, 1f);
        _line.textureMode               = LineTextureMode.Tile;

        _startPos = transform.position;
        _effectiveWaveLength = waveLength;
    }

    void Update()
    {
        _time += Time.deltaTime * speed;

<<<<<<< Updated upstream
        // Slew lineare del valore usato (niente accelerazioni)
        _effectiveWaveLength = Mathf.MoveTowards(
            _effectiveWaveLength, 
            waveLength, 
            wavelengthSlewPerSecond * Time.deltaTime
        );
=======
        void Update()
        {
        

            _time += Time.deltaTime * speed;
>>>>>>> Stashed changes

        // Movimento: avanza lungo X (verticale visivo), oscilla su Y
        float phase = (2f * Mathf.PI) * (_time / _effectiveWaveLength);
        float x = _startPos.y + _time;   
        float y = Mathf.Sin(phase) * amplitude;
        transform.position = new Vector3(x, y, 0f);

        UpdateTrajectory();
        UpdateColors();
        ApplyInertiaVisuals(); // applica lo stato visivo dell’inerzia (spessore/alpha)
    }

<<<<<<< Updated upstream
    void UpdateTrajectory()
    {
=======
            transform.position = new Vector3(_fixedPlayerPos.x, _fixedPlayerPos.y + y, _fixedPlayerPos.z);

            UpdateTrajectory();
            UpdateColors();
            ApplyInertiaVisuals();
        }



    void UpdateTrajectory()
        {
>>>>>>> Stashed changes
        float halfDist = previewDistance * 0.5f;

        for (int i = 0; i < previewPoints; i++)
        {
            // da -halfDist (dietro) a +halfDist (davanti), centrato sul player
            float tNorm  = i / (float)(previewPoints - 1);
            float offset = Mathf.Lerp(-halfDist, +halfDist, tNorm);

            float t     = _time + offset;
            float phase = (2f * Mathf.PI) * (t / _effectiveWaveLength);
            float x     = _startPos.y + t;
            float y     = Mathf.Sin(phase) * amplitude;

            _line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void UpdateColors()
    {
        float colorFactor = Mathf.InverseLerp(-5f, 5f, amplitude);
        UnityEngine.Color c = colorByWave.Evaluate(colorFactor);

        // Alpha regolato da ApplyInertiaVisuals (qui lo lasciamo pieno, lo modificheremo dopo)
        c.a = 1f;

        _sprite.color   = c;
        _line.startColor = c;
        _line.endColor   = c;
    }

    // Chiamata dal PlayerControl ogni frame:
    // ampFactor  = 0..1 (0 = ampiezza piccola; 1 = ampiezza grande)
    // waveFactor = 0..1 (0 = lunghezza piccola; 1 = lunghezza grande)
    public void ApplyInertiaFeedback(float ampFactor, float waveFactor)
    {
        _inertiaBlend = Mathf.Clamp01((ampFactor + waveFactor) * 0.5f);
        // Non applichiamo subito qui i cambi grafici per evitare doppio set del colore:
        // mettiamo i dati e poi li usiamo in ApplyInertiaVisuals() a fine Update().
    }

    void ApplyInertiaVisuals()
    {
        // Spessore: più inerzia ⇒ più spesso (come “tensione” dell’onda)
        float width = lineWidthBase + lineWidthExtraAtMaxInertia * _inertiaBlend;
        _line.widthMultiplier = width;

        // Alpha: più inerzia ⇒ meno brillante (un filo “pesante”)
        float alpha = Mathf.Lerp(1f, lineMinAlphaAtMaxInertia, _inertiaBlend);

        // Applica alpha al colore attuale della linea (manteniamo tinta da gradient)
        UnityEngine.Color sc = _line.startColor; sc.a = alpha;
        UnityEngine.Color ec = _line.endColor;   ec.a = alpha;
        _line.startColor = sc;
        _line.endColor   = ec;

        // (facoltativo) anche lo sprite potrebbe calare alpha con inerzia:
        // ColorData pc = _sprite.color; pc.a = alpha; _sprite.color = pc;
    }

    public void SetAmplitude(float value) => amplitude = Mathf.Clamp(value, -5f, 5f);
}
