using System;
using UnityEngine;

using static ParryController;



[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]

public class PlayerWaveController : MonoBehaviour

{

    [Header("Wave Parameters")]

    [Range(-50f, 50f)] public float amplitude = 0f;

    [Range(1f, 10f)] public float waveLength = 5f;

    public float speed = 5f;



    [Header("Colors")]

    public WaveStateMapping[] stateMappings;

    public Gradient colorByWave;
    public const float MAX_AMPLITUDE_FOR_COLOR = 5f; 



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

    private float _baseY;

    private float _time;

    private float _effectiveWaveLength;

    private float _inertiaBlend;



    private float _parryTimer = 0f;

    private ColorOverride _currentColorOverride = ColorOverride.None;

    public ColorOverride CurrentColorOverride => _currentColorOverride;



    public void SetColorOverride(ColorOverride newColor) => _currentColorOverride = newColor;

    public ColorType CurrentWaveColor { get; private set; }

    public Action<ColorType> OnColorChanged;





    void Start()

    {

        _sprite = GetComponent<SpriteRenderer>();

        _line = GetComponent<LineRenderer>();



        _baseY = transform.position.y;



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



        Vector3 pos = transform.position;

        pos.y = _baseY + y;



        transform.position = pos;


        HandleParryState();
        UpdateTrajectory();
        UpdateColors();
        ApplyInertiaVisuals();

    }



    void HandleParryState()

    {

        if (Input.GetKeyDown(parryKey) && _currentColorOverride == ColorOverride.None)

        {

            _parryTimer = parryDuration;

            _currentColorOverride = ColorOverride.Parry;

            Debug.Log("Parry Attivato!");

        }



        if (_parryTimer > 0f)

        {

            _parryTimer -= Time.deltaTime;

            if (_parryTimer <= 0f)

            {

                _currentColorOverride = ColorOverride.None;


            }

        }

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





    void UpdateColors()
    {
        Color c;

        // 1. Gestione Override (es. Parry)
        if (_currentColorOverride == ColorOverride.Parry)
        {
            c = parryColor;
            // Se hai uno sprite specifico per il Parry, dovrai aggiungere 'Parry' al tuo ColorType 
            // o gestirlo separatamente nello SpriteController.
            // Per ora, assumiamo che Parry non cambi lo sprite del colore base.
        }
        else
        {
            float currentAmplitude = Mathf.Clamp(Mathf.Abs(amplitude), 0f, MAX_AMPLITUDE_FOR_COLOR);
            float activeThreshold = 0f;

            // 2. Trova il Mappaggio Attivo
            foreach (var mapping in stateMappings)
            {
                if (currentAmplitude >= mapping.threshold)
                {
                    // ✨ Ecco la magia: salviamo il tipo di colore associato alla soglia
                    CurrentWaveColor = mapping.state;
                    activeThreshold = mapping.threshold;
                }
            }

            // 3. Calcola il Colore (Per il LineRenderer/SpriteRenderer)
            // Questa logica rimane per colorare la wave e il corpo del player con la Gradient.
            float colorFactor = Mathf.InverseLerp(0f, MAX_AMPLITUDE_FOR_COLOR, activeThreshold);
            c = colorByWave.Evaluate(colorFactor);
        }

        // 4. Applica il Colore
        _sprite.color = c;
        Color sc = c; sc.a = _line.startColor.a;
        Color ec = c; ec.a = _line.endColor.a;
        _line.startColor = sc;
        _line.endColor = ec;

        // 📢 5. Notifica lo Sprite Controller
        // Inviamo il TIPO di colore esatto che è stato attivato dalla soglia
        if (_currentColorOverride != ColorOverride.Parry) 
            OnColorChanged?.Invoke(CurrentWaveColor);

        else OnColorChanged?.Invoke(ColorType.white);
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

    public void SetAmplitude(float value) => amplitude = Mathf.Clamp(value, -50f, 50f);

}



[System.Serializable]

public struct WaveStateMapping

{

    public ColorType state;

    public float threshold;

}