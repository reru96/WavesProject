using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{

    public float speed = 2f;
    private Transform player;

    void Start()
    {
        player = RespawnManager.Instance.GetPlayer()?.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (transform.position.x < player.position.x - 10f)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            var life = other.GetComponent<LifeController>();
            if (life != null)
                life.TakeDamage(1);

            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }
}

