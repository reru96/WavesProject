using UnityEngine;
using DG.Tweening;

public class MainMenuWave : MonoBehaviour
{
    [SerializeField] private LineRenderer _bgWaveRenderer;
    [SerializeField] private int _point = 100;

    [SerializeField] private float _amplitude = 1f;
    [SerializeField] private float _amplitudeOffset = 0.5f;
    [SerializeField] private float _maxAmplitude = 5f;
    [SerializeField] private float _frequency = 1f;

    private float _leftX;
    private float _rightX;

    private float _currentAmplitude;   // per DOTween
    private float _resetAmplitude;

    public float CurrentAmplitude => _currentAmplitude;

    private void Awake()
    {
        _bgWaveRenderer = GetComponent<LineRenderer>();
        _currentAmplitude = _amplitude;
        _resetAmplitude = _amplitude;

        Camera cam = Camera.main;

        _leftX = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
        _rightX = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 10f)).x;
    }

    private void Update()
    {
        DrawWave();
    }

    private void DrawWave()
    {
        _bgWaveRenderer.positionCount = _point;

        for (int i = 0; i < _point; i++)
        {
            float t = (float)i / (_point - 1);

            // posizione X che copre tutto lo schermo
            float x = Mathf.Lerp(_leftX, _rightX, t);

            // sinusoide
            float y = _currentAmplitude * Mathf.Sin(
                (t * Mathf.PI * 2f * _frequency) + Time.time
            );

            _bgWaveRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }


    public void ImproveAmplitude()
    {
        float target = Mathf.Min(_currentAmplitude + _amplitudeOffset, _maxAmplitude);

        DOTween.To(
            () => _currentAmplitude,
            v => _currentAmplitude = v,
            target,
            0.5f
        ).SetEase(Ease.OutQuad);
    }

    public void RestoreAmplitude()
    {
        float target = _resetAmplitude;

        DOTween.To(
            () => _currentAmplitude,
            v => _currentAmplitude = v,
            target,
            0.5f
        ).SetEase(Ease.OutQuad);
    }
}

