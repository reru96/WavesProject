using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Scene Library", menuName = ("SO / Audio"))]
public class SceneLibrary :ScriptableObject
{
    [System.Serializable]
    public class SceneMusicEntry
    {
        public string sceneName; 
        public string musicKey;
    }
    public List<SceneMusicEntry> sceneMusics= new List<SceneMusicEntry>();
}
