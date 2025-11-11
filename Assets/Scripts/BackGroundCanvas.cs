using System.Timers;
using UnityEngine;

public class BackGroundCanvas : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += UpdateRotation;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady -= UpdateRotation;
    }

    void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.Rotate(0f, -(rotationSpeed * Time.deltaTime), 0f);
    }
}
