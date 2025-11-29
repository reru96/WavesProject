using UnityEngine;
using FMODUnity; 
using FMOD.Studio;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Configurazione FMOD")]
    public EventReference musicEvent; 
    public string parameterName = "Wave_Intensity"; 

    [Header("Riferimenti Gioco")]
    public PlayerWaveController playerWave;

    private EventInstance musicInstance;
    [Range(0f,1f)] public float volume = 0.5f;

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
        SetVolume();
    }

    public void SetVolume()
    {
        if (playerWave != null)
        {

            float currentIntensity = Mathf.Clamp(Mathf.Abs(playerWave.amplitude), 0f, 5f);
            musicInstance.setParameterByName(parameterName, currentIntensity);
            musicInstance.setVolume(volume);
        }
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}