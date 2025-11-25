using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite[] _playerColors;
    private PlayerWaveController _waveController;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _waveController = GetComponent<PlayerWaveController>();
    }

    private void OnEnable()
    {
        _waveController.OnColorChanged += UpdateSpriteColor;
    }

    private void OnDisable()
    {
        _waveController.OnColorChanged -= UpdateSpriteColor;
    }

    private void UpdateSpriteColor(ColorType colorType)
    {
        int index = (int)colorType;

        if (index < 0 || index >= _playerColors.Length)
        {
            Debug.LogWarning($"PlayerSpriteController: nessun sprite per ColorType {colorType}");
            return;
        }

        _renderer.sprite = _playerColors[index];
    }
}
