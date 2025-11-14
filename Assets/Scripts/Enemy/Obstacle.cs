using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{

    public float speed = 2f;
    private Transform player;
    public float returnToPoolOffset = 10f;
    public string obstacleSound;

    void Start()
    {
        player = RespawnManager.Instance.Player.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Muoviti costantemente verso sinistra
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Se l'ostacolo è troppo indietro rispetto al player, torna alla pool
        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            var life = other.GetComponent<LifeController>();
            if (life != null)
                life.TakeDamage(1);
            AudioManager.Instance.PlaySfx(obstacleSound);
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }
}

    


