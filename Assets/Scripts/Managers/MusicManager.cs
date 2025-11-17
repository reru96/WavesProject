using UnityEngine;
using FMODUnity; 
using FMOD.Studio; 

public class MusicManager : MonoBehaviour
{
    [Header("Configurazione FMOD")]
    public EventReference musicEvent; 
    public string parameterName = "Wave_Intensity"; 

    [Header("Riferimenti Gioco")]
    public PlayerWaveController playerWave; 

    private EventInstance musicInstance;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
        if (playerWave == null)
        {
            playerWave = FindFirstObjectByType<PlayerWaveController>();
        }
    }

    void Update()
    {
        if (playerWave != null)
        {

            float currentIntensity = Mathf.Clamp(Mathf.Abs(playerWave.amplitude), 0f, 5f);
            musicInstance.setParameterByName(parameterName, currentIntensity);
        }
    }


    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}