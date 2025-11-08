using UnityEngine;

[CreateAssetMenu(fileName =("Object"), menuName =("SO / Objects"))]
public class Object : ScriptableObject
{
    public float maxHp;
    public float currentHp;
    public DeathAction deathType;
    public GameObject prefab;
}

public enum DeathAction { None, Disable, Destroy, Respawn, SceneReload, Die }