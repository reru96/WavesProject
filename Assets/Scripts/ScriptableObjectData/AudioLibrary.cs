using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =("Audio Library"), menuName =("SO / Audio / Audio Library"))]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    public List<AudioEntry> clips = new List<AudioEntry>();
}
