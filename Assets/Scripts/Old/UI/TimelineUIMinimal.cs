using UnityEngine;
using UnityEngine.UI;

public class TimelineUIMinimal : MonoBehaviour
{
    [Header("Refs")]
    public WaveTimelineData data;
    public WaveSynth synth;
    public WaveRendererFromTimeline rendererFromTimeline;
    public BallOnWaveFromTimeline ball;

    [Header("UI")]
    public Slider timeSlider;      // 0..totalLength (dove metto il key)
    public Slider ampSlider;       // 0..1 valore per il key
    public Slider freqSlider;      // 0..1 valore per il key
    public Button addKeyButton;
    public Button clearButton;
    public Button previewButton;
    public Button playButton;

    void Start()
    {
        // Inizializzo dati se vuoti
        if (data.keys.Count < 2) data.ResetToDefault();

        // Wiring pulsanti
        addKeyButton.onClick.AddListener(OnAddKey);
        clearButton.onClick.AddListener(OnClear);
        previewButton.onClick.AddListener(OnPreview);
        playButton.onClick.AddListener(OnPlay);

        // Stato iniziale = PREVIEW (si sente il suono, non si vede l'onda)
        synth.data = data;
        synth.SetPreview(true);

        rendererFromTimeline.data = data;
        rendererFromTimeline.Hide();

        ball.data = data;
        ball.ResetBall();

        // Slider range coerenti
        if (timeSlider != null) { timeSlider.minValue = 0f; timeSlider.maxValue = data.totalLength; }
        if (ampSlider != null) { ampSlider.minValue = 0f; ampSlider.maxValue = 1f; }
        if (freqSlider != null) { freqSlider.minValue = 0f; freqSlider.maxValue = 1f; }
    }

    void OnAddKey()
    {
        float t = timeSlider ? timeSlider.value : 0f;
        float amp = ampSlider ? ampSlider.value : 0.5f;
        float fr = freqSlider ? freqSlider.value : 0.5f;

        data.keys.Add(new TimelineKey { time = Mathf.Clamp(t, 0f, data.totalLength), amplitude = amp, frequency = fr });
        data.SortKeys();
        // Durante preview il synth già userà i nuovi key. Non devo fare altro.
    }

    void OnClear()
    {
        data.ResetToDefault();
    }

    void OnPreview()
    {
        synth.SetPreview(true);
        rendererFromTimeline.Hide();
        ball.ResetBall();
    }

    void OnPlay()
    {
        // Silenzio preview (se vuoi puoi lasciarlo, a me piace "solo visivo" in Play)
        synth.SetPreview(false);

        // Disegno l'onda nello spazio e faccio partire la palla
        rendererFromTimeline.DrawFromTimeline();
        ball.Play();
    }
}
