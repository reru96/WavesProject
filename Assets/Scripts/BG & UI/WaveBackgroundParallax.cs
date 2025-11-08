using UnityEngine;

[ExecuteAlways]
public class WaveBackgroundField : MonoBehaviour
{
    [Header("References")]
    public PlayerWaveController player;
    public GameObject squarePrefab;

    [Header("Field Settings")]
    public int squareCount = 100;
    public Vector2 fieldSize = new Vector2(30f, 15f);
    public Vector2 scaleRange = new Vector2(0.5f, 6f);
    public Vector2 zRange = new Vector2(-10f, 10f);

    [Header("Animation")]
    [Tooltip("Percentuale di variazione rispetto alla scala base (0.5 = ±50%)")]
    [Range(0f, 1f)] public float scaleVariation = 0.5f;
    public float lerpSpeed = 3f;
    public float rotationSpeed = 5f;

    private Transform[] _squares;
    private SpriteRenderer[] _renderers;
    private bool[] _groupA;
    private float[] _baseScales;
    private UnityEngine.Color[] _baseColors;

    void Start()
    {
        GenerateField();
    }

    void GenerateField()
    {
        // Cancella i figli se rigenerato in editor
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        _squares = new Transform[squareCount];
        _renderers = new SpriteRenderer[squareCount];
        _groupA = new bool[squareCount];
        _baseScales = new float[squareCount];
        _baseColors = new UnityEngine.Color[squareCount];

        for (int i = 0; i < squareCount; i++)
        {
            GameObject sq = Instantiate(squarePrefab, transform);

            // Posizione casuale nel campo (XY + Z)
            float x = Random.Range(-fieldSize.x * 0.5f, fieldSize.x * 0.5f);
            float y = Random.Range(-fieldSize.y * 0.5f, fieldSize.y * 0.5f);
            float z = Random.Range(zRange.x, zRange.y);
            sq.transform.localPosition = new Vector3(x, y, z);

            // Scala base casuale
            float baseScale = Random.Range(scaleRange.x, scaleRange.y);
            sq.transform.localScale = Vector3.one * baseScale;

            // SpriteRenderer setup
            SpriteRenderer sr = sq.GetComponent<SpriteRenderer>();
            if (sr == null) sr = sq.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -100; // sempre in background
            sr.color = new UnityEngine.Color(1f, 1f, 1f, Random.Range(0.2f, 0.7f));

            // Salva dati
            _squares[i] = sq.transform;
            _renderers[i] = sr;
            _groupA[i] = (i % 2 == 0); // alterna battere/levare
            _baseScales[i] = baseScale;
            _baseColors[i] = sr.color;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Otteniamo la "posizione verticale" del player come fase
        float normalizedY = Mathf.InverseLerp(-Mathf.Abs(player.amplitude), Mathf.Abs(player.amplitude), player.transform.localPosition.y);
        float phase = Mathf.Clamp01(normalizedY);

        for (int i = 0; i < _squares.Length; i++)
        {
            float baseScale = _baseScales[i];
            UnityEngine.Color baseColor = _baseColors[i];
            Transform t = _squares[i];
            SpriteRenderer sr = _renderers[i];

            // Battere / levare invertiti
            float curveValue = _groupA[i] ? phase : (1f - phase);

            // Calcola offset relativo alla scala base
            float offset = (curveValue - 0.5f) * 2f * scaleVariation;
            float targetScale = baseScale * (1f + offset);

            // Interpolazione morbida
            float newScale = Mathf.Lerp(t.localScale.x, targetScale, lerpSpeed * Time.deltaTime);
            t.localScale = Vector3.one * newScale;

            // Opacità proporzionale (più ampiezza, più trasparente in un gruppo)
            float targetAlpha = Mathf.Clamp01(baseColor.a * (1f + offset * 0.5f));
            UnityEngine.Color c = sr.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, lerpSpeed * Time.deltaTime);
            sr.color = c;

            // Rotazione lenta (varia in base alla profondità)
            float rotSpeed = rotationSpeed * Mathf.Sign(zRange.y - t.localPosition.z);
            t.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
        }
    }
}
