using System.Security.Cryptography;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] private PathManager pathManager;
    [SerializeField] private SpriteFollower playerSprite;

    private float followSpeed;
    [SerializeField] private float horizontalOffset = 2f;

    private bool _isFollowing = false;

    private Vector3 _targetPosition;

    private Transform _lastCommittedPoint;

    private void Start()
    {
        player = RespawnManager.Instance.GetPlayer().GetComponentInChildren<SpriteFollower>().transform;

        if (pathManager == null)
        {
            Debug.Log("CameraFollow: Finding PathManager in scene.");
            pathManager = FindFirstObjectByType<PathManager>();
            if (pathManager == null) Debug.LogError("CameraFollow: PathManager not found in scene.");
        }


        pathManager.OnSegmentCommitted += SetTarget;
        playerSprite = RespawnManager.Instance.GetPlayer().GetComponentInChildren<SpriteFollower>();

        if (player == null)
        {
            Debug.LogError("CameraFollow: Player transform is null.");
        }

        //transform.position = new Vector3(player.position.x - horizontalOffset, player.position.y, transform.position.z);
    }

    private void SetTarget(Vector3 newPos)
    {
        Debug.Log("Set target called in CameraFollow.");
        _targetPosition = new Vector3(player.position.x - horizontalOffset, newPos.y, transform.position.z);
        _isFollowing = true;
    }

    private void Update()
    {
        if (player == null)
        {
            player = RespawnManager.Instance.GetPlayer().GetComponentInChildren<SpriteFollower>().transform;
        }

        followSpeed = playerSprite.Speed;

        Debug.Log($"{player.gameObject.name}");
    }

    private void LateUpdate()
    {
        if (_targetPosition != transform.position && _isFollowing)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * followSpeed);
        }
    }
}
