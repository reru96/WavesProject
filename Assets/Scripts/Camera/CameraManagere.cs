using Unity.VisualScripting;
using UnityEngine;

public class CameraManagere : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _cameraOffset = 5f;

    void LateUpdate()
    {
        if (_player == null)
        {
<<<<<<< Updated upstream
            _player = RespawnManager.Instance.GetPlayer()?.transform;
        }
        
=======
            _player = GameObject.FindAnyObjectByType<RespawnManager>()?.GetPlayer()?.transform;
        }
        ;
>>>>>>> Stashed changes

        Vector3 camPos = transform.position;
        camPos.x = _player.position.x + _cameraOffset;
        transform.position = camPos;
    }
}
