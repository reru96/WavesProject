using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject WavePrefab;
    [SerializeField] private float waveLife = 3f;
    [SerializeField] private float waveSpeed = 100f;

    public float speed = 2f;
    private Transform player;
    public float returnToPoolOffset = 10f;
    public string obstacleSound;

    void Start()
    {
        player = RespawnManager.Instance.Player.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Muoviti costantemente verso sinistra
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Se l'ostacolo è troppo indietro rispetto al player, torna alla pool
        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<ParryController>().IsParryActive())
            {
                TransfromInWaves();
            }
            else
            {
                var life = other.GetComponent<LifeController>();
                if (life != null)
                    life.TakeDamage(1);
            }
            AudioManager.Instance.PlaySfx(obstacleSound);
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }

    private void TransfromInWaves()
    {
        GameObject wave = Instantiate(WavePrefab, transform.position, Quaternion.identity);
        StartCoroutine(MoveWave(waveLife, wave));
    }
    private IEnumerator MoveWave(float wavelife, GameObject waveobj)
    {
        float timer = 0f;
        float direction = Mathf.Sign(player.localScale.x);
        Vector3 moveDirection = Vector3.right * direction;

        while (timer < wavelife)
        {
            // Muovi l’onda usando la direzione costante calcolata e la velocità definita
            waveobj.transform.Translate(moveDirection * waveSpeed * Time.deltaTime, Space.World);

            timer += Time.deltaTime;
            yield return null;
        }

        // Dopo waveLife secondi distruggi l’onda
        Destroy(waveobj);
    }

}




