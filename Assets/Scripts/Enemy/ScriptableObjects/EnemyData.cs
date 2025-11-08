using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public int damage = 1;
    public float moveSpeed = 2.0f;
    public ColorData enemyColor;

}

public enum ColorData
{
    Red,
    Blue,
    White,
    ColorChanger
}
