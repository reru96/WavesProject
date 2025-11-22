using UnityEngine;

[CreateAssetMenu(fileName =("Object"), menuName =("SO / Objects"))]
public class CreatureSO : ScriptableObject
{
    public int maxHp;
    public DeathAction deathType;
    public GameObject prefab;
    public string hitSound;
    public string deathSound;
    public ColorType colorID;
    //public Color SpriteColor;
    public Sprite EnemySprite;
}

public enum DeathAction { None, Disable, Destroy, Respawn, SceneReload, Die }