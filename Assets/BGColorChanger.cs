using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System;

public class BGColorChanger : MonoBehaviour
{
    private Image _image;

    [Header("Color Settings")]
    [SerializeField] private float _colorChangeDuration = 0.5f;

    private Color _currentColor = Color.white;
    private PlayerWaveController _waveController;
    private Coroutine _colorTransitionCoroutine;

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_image != null)
        {
            _image.color = _currentColor;
        }
    }

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerReady += GetPlayer;
        }
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerReady -= GetPlayer;
        }

        if (_waveController != null)
        {
            _waveController.OnColorChanged -= OnPlayerColorChanged;
        }

        if (_colorTransitionCoroutine != null)
        {
            StopCoroutine(_colorTransitionCoroutine);
        }
    }

    public void GetPlayer()
    {
        var player = RespawnManager.Instance.Player;

        if (player == null) return;

        _waveController = player.GetComponent<PlayerWaveController>();

        if (_waveController != null)
        {
            _waveController.OnColorChanged += OnPlayerColorChanged;
            OnPlayerColorChanged(_waveController.CurrentColorType);
        }
        else
        {
            _waveController = FindAnyObjectByType<PlayerWaveController>();
            OnPlayerColorChanged(_waveController.CurrentColorType);
        }
    }

    private void OnPlayerColorChanged(ColorType newType)
    {
        Color targetColor = GetTargetColor(newType);

        if (_colorTransitionCoroutine != null)
        {
            StopCoroutine(_colorTransitionCoroutine);
        }

        _colorTransitionCoroutine = StartCoroutine(ColorTransitionRoutine(targetColor));
    }

    private IEnumerator ColorTransitionRoutine(Color targetColor)
    {
        if (_image == null)
        {
            _colorTransitionCoroutine = null;
            yield break;
        }

        Color startingColor = _currentColor;
        float elapsedTime = 0f;

        while (elapsedTime < _colorChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _colorChangeDuration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            Color newColor = Color.Lerp(startingColor, targetColor, easedT);

            _image.color = newColor;

            _currentColor = newColor;

            yield return null;
        }

        _image.color = targetColor;
        _currentColor = targetColor;

        _colorTransitionCoroutine = null;
    }

    private Color GetTargetColor(ColorType currentType)
    {
        Color target = Color.black;

        switch (currentType)
        {
            case ColorType.White:
                target = Color.white;
                break;
            case ColorType.Red:
                target = Color.red;
                break;
            case ColorType.Orange:
                target = new Color(1f, 0.65f, 0f);
                break;
            case ColorType.Yellow:
                target = Color.yellow;
                break;
            case ColorType.Green:
                target = Color.green;
                break;
            case ColorType.Cyan:
                target = Color.cyan;
                break;
            case ColorType.Blue:
                target = Color.blue;
                break;
            case ColorType.Purple:
                target = new Color(0.5f, 0f, 0.5f);
                break;
        }
        return target;
    }
}
