using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PathManager pathManager;
    [SerializeField] private SpriteFollower playerSprite;

    private float followSpeed;
    [SerializeField] private float horizontalOffset = 2f;

    private bool _isFollowing = false;

    private Vector3 _targetPosition;

    private Transform _lastCommittedPoint;

    private void Start()
    {
        if (pathManager != null)
        {
            pathManager.OnSegmentCommitted += SetTarget;
        }

        //transform.position = new Vector3(player.position.x - horizontalOffset, player.position.y, transform.position.z);
    }

    private void SetTarget(Vector3 newPos)
    {
        _targetPosition = new Vector3(player.position.x - horizontalOffset, newPos.y, transform.position.z);
        _isFollowing = true;

    }

    private void Update()
    {
        followSpeed = playerSprite.Speed;
    }

    private void LateUpdate()
    {
        if (_targetPosition != transform.position && _isFollowing)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * followSpeed);
        }
    }
}
