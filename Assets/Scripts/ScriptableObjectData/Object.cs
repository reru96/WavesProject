using UnityEngine;

[CreateAssetMenu(fileName =("Object"), menuName =("SO / Objects"))]
public class Object : ScriptableObject
{
    public int maxHp;
    public int currentHp;
    public DeathAction deathType;
    public GameObject prefab;
    public string hitSound;
    public string deathSound;
}

public enum DeathAction { None, Disable, Destroy, Respawn, SceneReload, Die }