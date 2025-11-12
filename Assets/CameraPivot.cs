using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    private Vector3 baseLocalPos;

    void Start()
    {
        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = new Vector3(baseLocalPos.x, baseLocalPos.y, baseLocalPos.z);
    }
}
