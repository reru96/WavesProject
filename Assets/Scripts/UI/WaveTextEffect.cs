using UnityEngine;
using TMPro; // Ricorda di includere la libreria di TextMeshPro
using System.Collections; // Non essenziale qui, ma buona pratica per le Coroutine se le usassi

// Assicuriamoci che l'oggetto abbia sempre un TextMeshProUGUI
[RequireComponent(typeof(TextMeshProUGUI))]
public class WaveTextEffect : MonoBehaviour
{
    [Header("Parametri Onda Unitaria")]
    [Tooltip("L'altezza massima dell'onda (spostamento massimo in Y).")]
    public float waveAmplitude = 15f;

    [Tooltip("La frequenza dell'onda. Valori più bassi creano onde più lunghe e fluide.")]
    public float waveFrequency = 0.7f; // Il segreto per l'effetto unitario

    [Tooltip("La velocità con cui l'onda si muove lungo il testo.")]
    public float waveSpeed = 3f;

    private TMP_Text _textComponent;
    private Vector3[] _baseVertices; // Array per le posizioni iniziali dei vertici

    // --- Inizializzazione e Gestione Aggiornamento ---

    void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        // Forza la prima generazione del mesh
        _textComponent.ForceMeshUpdate();

        // All'inizio, il testo è appena stato generato, quindi i vertici sono nella posizione di base.
        CacheBaseVertices();
    }

    // Ci agganciamo all'evento di TextMeshPro per sapere quando il testo viene rigenerato
    void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    // Questo viene chiamato quando il testo o le sue impostazioni cambiano
    private void OnTextChanged(Object obj)
    {
        // Se l'oggetto che ha subito modifiche è il nostro TextMeshPro,
        // dobbiamo salvare le nuove posizioni di base.
        if (obj == _textComponent)
        {
            CacheBaseVertices();
        }
    }

    // Salva una copia delle posizioni iniziali dei vertici
    private void CacheBaseVertices()
    {
        if (_textComponent.textInfo.characterCount == 0) return;

        // Assumiamo che stiamo usando il primo sottomesh (indice 0) per i vertici del testo.
        int totalVertexCount = _textComponent.textInfo.meshInfo[0].vertices.Length;

        // Riallociamo e copiamo l'array solo se le dimensioni non corrispondono
        if (_baseVertices == null || _baseVertices.Length != totalVertexCount)
        {
            _baseVertices = new Vector3[totalVertexCount];
        }

        // Copia i vertici "puliti"
        System.Array.Copy(_textComponent.textInfo.meshInfo[0].vertices, _baseVertices, totalVertexCount);
    }

    // --- Logica di Animazione Onda ---

    void LateUpdate()
    {
        if (_textComponent.textInfo.characterCount == 0 || _baseVertices == null) return;

        // Richiedi un aggiornamento della geometria per accedere ai dati attuali.
        // Questo carica i dati correnti (che noi aggiorneremo) nell'oggetto textInfo.
        _textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = _textComponent.textInfo;

        // Otteniamo l'array di vertici sul quale lavoreremo (array di destinazione)
        Vector3[] destinationVertices = textInfo.meshInfo[0].vertices;

        // Iteriamo su tutti i caratteri visibili
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;

            // 1. Posizione X: Usiamo l'origine (centro orizzontale) del carattere in spazio locale.
            // Questa è la chiave per definire la posizione del carattere lungo la curva dell'onda.
            float charXPosition = charInfo.origin;

            // 2. Calcolo della Fase: combina tempo (per il movimento scorrevole) e posizione X (per la forma).
            // L'uso di `charXPosition * waveFrequency` assicura che ci sia una singola, continua curva.
            float wavePhase = (Time.time * waveSpeed) + (charXPosition * waveFrequency);

            // 3. Spostamento Y: La curva sinusoidale
            float waveOffset = Mathf.Sin(wavePhase) * waveAmplitude;

            Vector3 offset = new Vector3(0, waveOffset, 0);

            // --- Applicazione: Partiamo dalla Base ---

            for (int j = 0; j < 4; j++)
            {
                // Applichiamo l'offset Y alla posizione ORIGINALE del vertice, 
                // non a quella del frame precedente. Ciao ciao, cumulazione!
                destinationVertices[vertexIndex + j] = _baseVertices[vertexIndex + j] + offset;
            }
        }

        // Carichiamo i vertici modificati sulla GPU. Questo è l'ultimo passo.
        _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}