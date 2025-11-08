using UnityEngine;

public class SimoCameraControl : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField][Range(0,10)] private float xDifferencePos = -5;
    [SerializeField] private float speed = 2f;
    private float xPos;

    private void Update()
    {
        //xPos = player.transform.position.x + xDifferencePos;
        
    }
    private void LateUpdate()
    {
        //transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        transform.position += Vector3.right * speed * Time.deltaTime;

    }
}
