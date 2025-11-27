using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;
    public float returnToPoolOffset = 5f;
    public string deathSound = "EnemyDeathSound";
    public string hitSound = "EnemyHit";
    public ColorType currentColorType;
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

    public virtual void Initialize(Sprite colorToSet, ColorType colorType)
    {
        if (_sprite == null)
            _sprite = GetComponent<SpriteRenderer>();

        _sprite.sprite = colorToSet;
        currentColorType = colorType;
    }

    public virtual void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        transform.right = dir;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerWaveController = other.GetComponentInParent<PlayerWaveController>();
            var life = other.GetComponentInParent<LifeController>();

            if (playerWaveController == null || life == null)
            {
                Debug.LogWarning("Hai colpito un oggetto taggato 'Player', ma mancano gli script necessari (PlayerWaveController o LifeController)!", other.gameObject);
                return;
            }

            ColorType enemy = currentColorType;
            ColorType player = playerWaveController.CurrentColorType; 

            if (enemy == player)
            {
                AudioManager.Instance.PlaySfx(hitSound);
                Debug.Log("non colpito");
            }
            else
            {
                Debug.Log("colpito");
                AudioManager.Instance.PlaySfx(deathSound);
                TimeSetter.Instance.SlowMotionForImpact(0.1f);
                life.TakeDamage(damage);
            }

            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }

    public bool ColorsSimilar(Color a, Color b, float tolerance = 0.3f)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.b, a.b)) < tolerance;
    }
}
