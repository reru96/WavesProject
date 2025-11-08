using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;

    private SpriteRenderer _sprite;
    private Transform player;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

        player = RespawnManager.Instance.GetPlayer()?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Muoviti costantemente verso sinistra
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Se l'ostacolo è troppo indietro rispetto al player, torna alla pool
        if (transform.position.x < player.position.x - 10f)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerLife = other.GetComponent<LifeController>();
        var life = this.GetComponent<LifeController>();

        if (playerLife != null)
        {
            Color playerColor = playerLife.GetComponent<SpriteRenderer>().color;

            if (ColorsSimilar(_sprite.color, playerColor))
            {
                life?.SetHp(0);
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
