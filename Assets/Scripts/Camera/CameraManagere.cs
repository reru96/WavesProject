using Unity.Cinemachine;
using UnityEngine;

public class CameraManagere : MonoBehaviour
{
    [SerializeField] private Transform _cameraPivot; 
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float _smoothSpeed = 1f;
    public CinemachineCamera cineMachine;

    private void Start()
    {
        GameObject orientation = GameObject.FindGameObjectWithTag("Orientation");
        cineMachine.Follow = orientation.transform;
    }

    //void LateUpdate()
    //{ 
        
    //    //if (_cameraPivot == null) return;
       
    //    //Vector3 targetPos = _cameraPivot.position + _offset;
    //    //transform.position = Vector3.Lerp(transform.position, targetPos, _smoothSpeed * Time.deltaTime);
    //}
}
