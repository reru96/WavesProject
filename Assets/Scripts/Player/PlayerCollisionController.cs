using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with Enemy!");
            // Handle collision with enemy (e.g., reduce health, play sound, etc.)
        }
    }
}
