using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManagere : MonoBehaviour
{
    [SerializeField] private float cameraOffset = 5f;
    private Transform player;

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned += SetPlayer;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerSpawned -= SetPlayer;
    }

    private void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer.transform;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 camPos = transform.position;
        camPos.x = player.position.x + cameraOffset;
        transform.position = camPos;
    }
}
