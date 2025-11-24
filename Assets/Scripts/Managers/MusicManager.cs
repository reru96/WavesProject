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

    [Header("Pitch / Speed Control")]
    [Tooltip("Pitch quando la lunghezza d'onda è massima (onda larga = lento)")]
    public float minPitch = 0.5f;
    [Tooltip("Pitch quando la lunghezza d'onda è minima (onda stretta = veloce)")]
    public float maxPitch = 1.5f;
    [Tooltip("Lunghezza d'onda minima per il mapping (dovrebbe corrispondere a PlayerControl)")]
    public float minWaveLength = 1f;
    [Tooltip("Lunghezza d'onda massima per il mapping (dovrebbe corrispondere a PlayerControl)")]
    public float maxWaveLength = 10f;

    [Header("Volume Control")]
    [Range(0f, 1f)] public float masterVolume = 1f;

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
            // Intensity (Amplitude)
            float currentIntensity = Mathf.Clamp(Mathf.Abs(playerWave.amplitude), 0f, 5f);
            musicInstance.setParameterByName(parameterName, currentIntensity);

            // Pitch (Wavelength)
            // Invertito: Wavelength Bassa (A) -> Pitch Alto (Veloce)
            // Wavelength Alta (D) -> Pitch Basso (Lento)
            float t = Mathf.InverseLerp(minWaveLength, maxWaveLength, playerWave.waveLength);
            float targetPitch = Mathf.Lerp(maxPitch, minPitch, t); // Lerp invertito
            
            musicInstance.setPitch(targetPitch);

            // Volume
            musicInstance.setVolume(masterVolume);
        }
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}