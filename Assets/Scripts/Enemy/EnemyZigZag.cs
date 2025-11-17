using UnityEngine;

public class EnemyZigZag : Enemy
{

    public float amplitude = 1f;
    public float frequency = 2f;

    private float yOffset;

    protected override void Start()
    {
        base.Start();
        yOffset = transform.position.y;
    }

    protected override void FixedUpdate()
    {
        if (player == null) return;

        float y = yOffset + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position += Vector3.left * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
