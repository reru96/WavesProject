using UnityEngine;

public class SimoCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        NoteLogic note = collision.GetComponent<NoteLogic>();
        if (note != null)
        {
            switch (note.LaneIndex)
            {
                case 0:
                    Debug.Log("Collided with note in lane 0");
                    break;
                case 1:
                    Debug.Log("Collided with note in lane 1");
                    break;
                case 2:
                    Debug.Log("Collided with note in lane 2");
                    break;
                case 3:
                    Debug.Log("Collided with note in lane 3");
                    break;
                default:
                    Debug.Log("Collided with note in unknown lane");
                    break;
            }
        }
    }
}
