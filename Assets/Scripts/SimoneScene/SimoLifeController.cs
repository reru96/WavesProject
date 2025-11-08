using Unity.VisualScripting;
using UnityEngine;

public class SimoLifeController : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private bool infiniteLives = false;
    [SerializeField] private bool maxLivesOnStart = true;
    private int currentLives;

    private ColorData playerColor;

    private void Start()
    {
        if (maxLivesOnStart)
        {
            currentLives = maxLives;
        }

    }

    public void TakeDamage(int damage)
    {
        if (infiniteLives) return;
        currentLives -= damage;
        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }

    public void SetPlayerColor(ColorData color)
    {
        if (color == ColorData.ColorChanger)
        {
            Debug.Log("Changing color");
            playerColor = Random.Range(0, 3) switch
            {
                0 => ColorData.Red,
                1 => ColorData.Blue,
                2 => ColorData.White,
                _ => ColorData.White,
            };
            GetComponent<SpriteRenderer>().color = playerColor switch
            {
                ColorData.Red => UnityEngine.Color.red,
                ColorData.Blue => UnityEngine.Color.blue,
                ColorData.White => UnityEngine.Color.white,
                _ => UnityEngine.Color.white,
            };
            return;
        }

        playerColor = color;
        GetComponent<SpriteRenderer>().color = color switch
        {
            ColorData.Red => UnityEngine.Color.red,
            ColorData.Blue => UnityEngine.Color.blue,
            ColorData.White => UnityEngine.Color.white,
            _ => UnityEngine.Color.white,
        };
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("PlayerLifeController detected a collision.");
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with Enemy in PlayerLifeController.");
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                EnemyData enemyData = enemy.EnemyData;
                if (enemyData != null)
                {
                    if (enemyData.enemyColor == ColorData.ColorChanger)
                    {
                        SetPlayerColor(enemyData.enemyColor);
                        return;
                    }
                    if (enemyData.enemyColor != playerColor)
                    {
                        Debug.Log($"Enemy damage: {enemyData.damage}");
                        TakeDamage(enemyData.damage);
                    }
                    else
                    {
                        Destroy(collider.gameObject);
                        Debug.Log("No damage taken due to color match.");
                    }
                }
            }
        }
    }
}
