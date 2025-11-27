using UnityEngine;

public class BGColorChanger : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;
    [SerializeField] private string _colorPropertyName = "_BaseColor";
    private Color currentColor = Color.white;
    private PlayerWaveController _waveController;

    private void OnEnable()
    {
        RespawnManager.Instance.OnPlayerReady += GetPlayer;
    }

    private void OnDisable()
    {
        RespawnManager.Instance.OnPlayerReady -= GetPlayer;
    }

    private void Start()
    {
        
        _mpb = GetComponent<MaterialPropertyBlock>();
    }

    public void GetPlayer()
    {
        var player = RespawnManager.Instance.Player;
        _waveController = player.GetComponent<PlayerWaveController>();
    }

    void Update()
    {
        ChangeColorByType(_waveController.CurrentColorType, currentColor);
        _mpb.SetColor(_colorPropertyName,currentColor);
    }

    void ChangeColorByType(ColorType current, Color bgColor)
    {
        switch (_waveController.CurrentColorType)
        {
            case ColorType.White:
                bgColor = Color.white;
                break;
            case ColorType.Red:
                bgColor = Color.red;
                break;
            case ColorType.Orange:
                bgColor = Color.orange;
                break;
            case ColorType.Yellow:
                bgColor = Color.yellow;
                break;
            case ColorType.Green:
                bgColor = Color.green;
                break;
            case ColorType.Cyan:
                bgColor = Color.cyan;
                break;
            case ColorType.Blue:
                bgColor = Color.blue;
                break;
            case ColorType.Purple:
                bgColor = Color.purple;
                break;

        }
    }
}
