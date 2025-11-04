using UnityEngine;
using UnityEngine.UI;

public class LightWaveManager : MonoBehaviour
{
    [Header("UI")]
    public Slider intensitySlider;
    public Slider frequencySlider;
    public Button playButton;

    private bool _isPlaying = false;
    private LightWave _selectedLight;
    private LightWave[] _allLights;

    void Start()
    {
        _allLights = FindObjectsByType<LightWave>(FindObjectsSortMode.None);


        // Sottoscrivi all’evento di selezione
        LightWave.OnLightSelected += SelectLight;

        // Slider disattivi finché non selezioni una luce
        intensitySlider.interactable = false;
        frequencySlider.interactable = false;

        intensitySlider.onValueChanged.AddListener(UpdateIntensity);
        frequencySlider.onValueChanged.AddListener(UpdateFrequency);
        playButton.onClick.AddListener(TogglePlay);
    }

    void OnDestroy()
    {
        LightWave.OnLightSelected -= SelectLight;
    }

    void SelectLight(LightWave lw)
    {
        // Deseleziona tutte
        foreach (var l in _allLights)
            l.SetSelected(false);

        // Seleziona la nuova
        _selectedLight = lw;
        _selectedLight.SetSelected(true);

        intensitySlider.value = lw.intensity;
        frequencySlider.value = lw.frequency;

        intensitySlider.interactable = true;
        frequencySlider.interactable = true;
    }


    void UpdateIntensity(float val)
    {
        if (_selectedLight != null)
            _selectedLight.SetIntensity(val);
    }

    void UpdateFrequency(float val)
    {
        if (_selectedLight != null)
            _selectedLight.SetFrequency(val);
    }

    void TogglePlay()
    {
        _isPlaying = !_isPlaying;
        foreach (var lw in _allLights)
            lw.SetPlay(_isPlaying);
    }
}
