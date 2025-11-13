using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;
    public float returnToPoolOffset = 5f;
    protected SpriteRenderer _sprite;
    protected Transform player;
    public string fadeSound;
<<<<<<< Updated upstream
=======
    protected Rigidbody2D _rb;
    protected Vector2 moveDirection;
>>>>>>> Stashed changes

    protected virtual void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

        player = RespawnManager.Instance.GetPlayer()?.transform;
    }

<<<<<<< Updated upstream
    void Update()
=======
    protected virtual void FixedUpdate()
>>>>>>> Stashed changes
    {
        if (player == null) return;

        // Muoviti costantemente verso sinistra
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Se l'ostacolo è troppo indietro rispetto al player, torna alla pool
        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

<<<<<<< Updated upstream
    private void OnTriggerEnter2D(Collider2D other)
=======
    public virtual void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        transform.right = dir; 
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
>>>>>>> Stashed changes
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

    public bool ColorsSimilar(Color a, Color b, float tolerance = 0.3f)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < tolerance;
    }
}
