using System.Collections;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManagere : MonoBehaviour
{
    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [SerializeField, Range(0f, 1f)] private float smoothTime = 0.1f;

    public CinemachineCamera cineMachine;
    private GameObject target;
    private Vector3 currentVelocity;

    private void OnEnable()
    {
        cineMachine = GetComponent<CinemachineCamera>();
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += () => OnPlayerSpawned(target);
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady -= () => OnPlayerSpawned(target);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        cineMachine.Follow = target.transform;
    }

    private void OnPlayerSpawned(GameObject player)
    {
        if (player != null)
            target = player;
    }
    public void SetTarget(Transform newTarget)
    {
       newTarget  = target.transform;
    }
}
