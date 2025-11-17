using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletSO bullet;

    private Vector2 direction;
    private float timer;

    private void OnEnable()
    {
        timer = 0f;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.position += (Vector3)direction * bullet.speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= bullet.lifeTime)
        {
            ObjectPooler.Instance.ReturnToPool(gameObject); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var life = other.GetComponent<LifeController>();
            if (life != null)
                life.TakeDamage(bullet.damage);

            ObjectPooler.Instance.ReturnToPool(gameObject); 
        }
    }
}
