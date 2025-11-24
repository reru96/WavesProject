using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class GameManager : Singleton<GameManager>
{
    private const string BRIGHTNESS_KEY = "BrightnessValue";
    private const string RESOLUTION_INDEX_KEY = "ResolutionIndex";
    private const string FULLSCREEN_KEY = "FullscreenEnabled";

    private ColorAdjustments colorAdjustments;

    protected override void Awake()
    {
        base.Awake();
        SetupBrightness();
    }

    private void SetupBrightness()
    {
        float saved = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 0f);
        SetBrightness(saved);
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
            colorAdjustments.postExposure.value = value;

        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
    }

    public float GetBrightness() =>
        PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 0f);

    public void SetResolution(int index)
    {
        Resolution[] resolutions = Screen.resolutions;
        if (index < 0 || index >= resolutions.Length) return;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, index);
    }

    public int GetSavedResolutionIndex() =>
        PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, 0);

    public void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, enabled ? 1 : 0);
    }

    public bool GetSavedFullscreen() =>
        PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
}
