using System.Collections.Generic;
using UnityEngine;

// Dati "grezzi" della timeline. Tengo tutto minimo: una lista di key con tempo, amp, freq.
// NB: freq è normalizzata 0..1, la mapperò a Hz quando genero audio/onda.
[System.Serializable]
public class TimelineKey
{
    public float time;       // sec, 0..totalLength
    [Range(0f, 1f)] public float amplitude = 1f;
    [Range(0f, 1f)] public float frequency = 0.5f;
}

[CreateAssetMenu(fileName = "WaveTimeline", menuName = "EchoPath/WaveTimelineData")]
public class WaveTimelineData : ScriptableObject
{
    public float totalLength = 5f; // durata della sequenza in secondi
    public List<TimelineKey> keys = new List<TimelineKey>();

    // Ritorno amp/freq interpolati linearmente nel tempo (sufficienti per la jam).
    public float EvaluateAmplitude(float t)
    {
        if (keys.Count == 0) return 0f;
        if (t <= keys[0].time) return keys[0].amplitude;
        if (t >= keys[keys.Count - 1].time) return keys[keys.Count - 1].amplitude;

        for (int i = 0; i < keys.Count - 1; i++)
        {
            if (t >= keys[i].time && t <= keys[i + 1].time)
            {
                float f = Mathf.InverseLerp(keys[i].time, keys[i + 1].time, t);
                return Mathf.Lerp(keys[i].amplitude, keys[i + 1].amplitude, f);
            }
        }
        return 0f;
    }

    public float EvaluateFrequency(float t)
    {
        if (keys.Count == 0) return 0.5f;
        if (t <= keys[0].time) return keys[0].frequency;
        if (t >= keys[keys.Count - 1].time) return keys[keys.Count - 1].frequency;

        for (int i = 0; i < keys.Count - 1; i++)
        {
            if (t >= keys[i].time && t <= keys[i + 1].time)
            {
                float f = Mathf.InverseLerp(keys[i].time, keys[i + 1].time, t);
                return Mathf.Lerp(keys[i].frequency, keys[i + 1].frequency, f);
            }
        }
        return 0.5f;
    }

    // Utility: mantieni ordinati i key per tempo (evito casini).
    public void SortKeys()
    {
        keys.Sort((a, b) => a.time.CompareTo(b.time));
    }

    // Resetta con due punti base (inizio/fine): pratico per test immediato.
    public void ResetToDefault()
    {
        keys.Clear();
        keys.Add(new TimelineKey { time = 0f, amplitude = 0.5f, frequency = 0.4f });
        keys.Add(new TimelineKey { time = totalLength, amplitude = 0.5f, frequency = 0.4f });
    }
}
