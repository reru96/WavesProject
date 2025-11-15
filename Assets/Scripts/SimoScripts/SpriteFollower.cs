// SpriteFollower.cs
using UnityEngine;

public class SpriteFollower : MonoBehaviour
{
    [SerializeField] private PathManager pathManager;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSpeed = 15f;
    [Range(1.1f, 10f)][SerializeField] private float speedMultiplier = 1.25f;

    private float _standardSpeed;

    public float Speed => speed;

    private int currentPointIndex = 0;
    private float distanceOnSegment = 0f; // Distanza percorsa sul segmento corrente

    public bool IsMoving => currentPointIndex < pathManager.CommittedPoints.Count - 1;

    public void ResetSpeed() => speed = _standardSpeed;
    public void ResetSpeed(Vector3 value) => ResetSpeed();

    private void Start()
    {
        _standardSpeed = speed;
        pathManager.OnSegmentCommitted += ResetSpeed;
    }

    void Update()
    {
        if (pathManager.CommittedPoints.Count < 2 || !IsMoving)
        {
            return; // Stop se fine del path
        }

        speed += speedMultiplier * Time.deltaTime;
        speed = Mathf.Min(speed, maxSpeed);

        Vector3 start = pathManager.CommittedPoints[currentPointIndex];
        Vector3 end = pathManager.CommittedPoints[currentPointIndex + 1];
        float segmentLength = Vector3.Distance(start, end);

        // Avanza lungo il segmento
        distanceOnSegment += speed * Time.deltaTime;

        // Controlla se abbiamo completato il segmento
        if (distanceOnSegment >= segmentLength)
        {
            // Passa al prossimo segmento
            distanceOnSegment -= segmentLength;
            currentPointIndex++;

            if (currentPointIndex >= pathManager.CommittedPoints.Count - 1)
            {
                // Arrivato alla fine
                transform.position = pathManager.CommittedPoints[pathManager.CommittedPoints.Count - 1];
                return;
            }
        }

        // Calcola la posizione interpolata
        start = pathManager.CommittedPoints[currentPointIndex];
        end = pathManager.CommittedPoints[currentPointIndex + 1];
        segmentLength = Vector3.Distance(start, end);

        float t = segmentLength > 0 ? distanceOnSegment / segmentLength : 0f;
        transform.position = Vector3.Lerp(start, end, t);
    }

    // Metodo opzionale per resettare il follower
    public void ResetToStart()
    {
        currentPointIndex = 0;
        distanceOnSegment = 0f;

        if (pathManager.CommittedPoints.Count > 0)
        {
            transform.position = pathManager.CommittedPoints[0];
        }
    }
}