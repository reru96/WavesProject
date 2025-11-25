using System;
using UnityEngine;

public class WaveColorResolver : MonoBehaviour
{
    [Header("Mappings")]
    public WaveStateMapping[] mappings;
    public Gradient gradientByWave;
    public const float MAX_AMPLITUDE = 5f;

    public Action<ColorType, Color> OnResolvedColor;

    public void ResolveColor(float amplitude)
    {
        float absAmp = Mathf.Clamp(Mathf.Abs(amplitude), 0f, MAX_AMPLITUDE);

        ColorType selectedType = ColorType.White;
        float selectedThreshold = 0f;

        foreach (var m in mappings)
        {
            if (absAmp >= m.threshold)
            {
                selectedType = m.state;
                selectedThreshold = m.threshold;
            }
        }

        float factor = Mathf.InverseLerp(0f, MAX_AMPLITUDE, selectedThreshold);
        Color finalColor = gradientByWave.Evaluate(factor);
        OnResolvedColor?.Invoke(selectedType, finalColor);
    }
}
