using UnityEngine;

public class EnemySpecial : Enemy
{
    protected override void FixedUpdate()
    {
        if (player == null) return;

        // Muoviti costantemente verso sinistra
        transform.position += (Vector3.left * speed * Time.deltaTime) + (Vector3.down * speed * Time.deltaTime);

        // Se l'ostacolo è troppo indietro rispetto al player, torna alla pool
        if (transform.position.x < player.position.x - returnToPoolOffset)
            ObjectPooler.Instance.ReturnToPool(gameObject);
    }

}
