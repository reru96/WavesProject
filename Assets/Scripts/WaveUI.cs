using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    public PlayerWaveController player;
    public Slider ampSlider;
    public Slider freqSlider;

    void Start()
    {
        ampSlider.onValueChanged.AddListener(player.SetAmplitude);
        //freqSlider.onValueChanged.AddListener(player.SetFrequency);
    }
}
