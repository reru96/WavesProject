using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;
    public float returnToPoolOffset = 5f;
    private SpriteRenderer _sprite;
    private Transform player;
    public string fadeSound;
    private Rigidbody2D _rb;
    private Vector2 moveDirection;

    void Start()
    {
         _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

        player = RespawnManager.Instance.GetPlayer()?.transform;
    }

    void FixedUpdate()
    {
        if (_rb != null)
            _rb.linearVelocity = moveDirection * speed;

        if (player != null && transform.position.x < player.position.x - returnToPoolOffset * 2)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        transform.right = dir; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerLife = other.GetComponent<LifeController>();
        var life = gameObject.GetComponent<LifeController>();

        if (playerLife != null)
        {
            Color playerColor = playerLife.GetComponent<SpriteRenderer>().color;

            if (ColorsSimilar(_sprite.color, playerColor))
            {
                AudioManager.Instance.PlaySfx(fadeSound);
            }
            else
            {
                playerLife.TakeDamage(damage);
            }

            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }

    bool ColorsSimilar(Color a, Color b, float tolerance = 0.3f)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < tolerance;
    }
}
