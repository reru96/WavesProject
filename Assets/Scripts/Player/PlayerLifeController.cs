using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private bool infiniteLives = false;
    [SerializeField] private bool maxLivesOnStart = true;
    private int currentLives;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("PlayerLifeController detected a collision.");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with Enemy in PlayerLifeController.");
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"Enemy damage: {enemy.EnemyData.damage}");
                TakeDamage(enemy.EnemyData.damage);
            }
            else
            {
                Debug.LogWarning("EnemyData component not found on the collided enemy.");
                TakeDamage(1); // Default damage if EnemyData is missing
            }
        }
    }
}
