using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2f;
    private SpriteRenderer _sprite;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
    }

    void Update()
    {
        // Muoviti verso sinistra (indietro rispetto al player)
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // Distruggi se troppo indietro rispetto alla camera
        if (transform.position.x < Camera.main.transform.position.x - 10f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //var player = other.GetComponent<PlayerWaveController>(); al momento non serve, poi vediamo se riutilizzarla
        var player = other.GetComponent<LifeController>();
        var life = this.GetComponent<LifeController>();
        if (player != null)
        {
            Color playerColor = player.GetComponent<SpriteRenderer>().color;
            if (ColorsSimilar(_sprite.color, playerColor))
            {
                life.SetHp(0);
                // Qui potresti aggiungere: GameManager.Instance.AddScore(10);
            }
            else
            {
                player.TakeDamage(1);
                // Game Over: puoi chiamare un evento qui
            }
        }
    }

    bool ColorsSimilar(Color a, Color b)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < 0.3f;
    }
}
