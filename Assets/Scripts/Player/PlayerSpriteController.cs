using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [Tooltip("Watch out, they need to be in order! (Red, Orange, Yellow, Green, Cyan, Blue, Purple, white")]
    [SerializeField] private Sprite[] _playerColors;

    private PlayerWaveController _playerWaveController;
    private SpriteRenderer _playerSprite;

    public Sprite currentColor { get; private set; }

    private void Awake()
    {
        _playerSprite = GetComponent<SpriteRenderer>();
        _playerWaveController = GetComponent<PlayerWaveController>();
    }

    private void Start()
    {
        _playerWaveController.OnColorChanged += EvaluatePlayerColor;
    }
    public void UpdatePlayerColor(Sprite newColor)
    {
        if (_playerSprite.sprite != newColor)
        { _playerSprite.sprite = newColor; }
    }

    private void EvaluatePlayerColor(ColorType color)
    {
        switch (color) 
        {
            case ColorType.Red:    UpdatePlayerColor(_playerColors[0]);
                break;

            case ColorType.Orange: UpdatePlayerColor(_playerColors[1]);
                break;

            case ColorType.Yellow: UpdatePlayerColor(_playerColors[2]);
                break;

            case ColorType.Green:  UpdatePlayerColor(_playerColors[3]);
                break;

            case ColorType.Cyan:   UpdatePlayerColor(_playerColors[4]);
                break;

            case ColorType.Blue:   UpdatePlayerColor(_playerColors[5]);
                break;

            case ColorType.Purple: UpdatePlayerColor(_playerColors[6]);
                break;

            case ColorType.white:  UpdatePlayerColor(_playerColors[7]);
                    break;
        }
    }

}
