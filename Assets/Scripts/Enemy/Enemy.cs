using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;
    public float returnToPoolOffset = 5f;
    public string fadeSound;
    protected SpriteRenderer _sprite;
    protected Transform player;
    protected Rigidbody2D _rb;
    protected Vector2 moveDirection; 

    protected virtual void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        player = RespawnManager.Instance.Player.transform;
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;

        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    public virtual void Initialize(Sprite colorToSet)
    {
        if (_sprite == null)
            _sprite = GetComponent<SpriteRenderer>();

        _sprite.sprite = colorToSet;
    }

    public virtual void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        transform.right = dir;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var playerLife = other.GetComponent<LifeController>();

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

    public bool ColorsSimilar(Color a, Color b, float tolerance = 0.3f)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.b, a.b)) < tolerance;
    }
}
