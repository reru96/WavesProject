using UnityEngine;

public class EnemyShooter : Enemy
{
    public BulletSO bullet;
    public float shootInterval = 1.5f;
    private float timer;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            timer = 0;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject b = ObjectPooler.Instance.Spawn(bullet, transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * 5f;
    }
}
