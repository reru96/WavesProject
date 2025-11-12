using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NoteLogic : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float destroyYPosition = -6f;

    [Header("Onda")]
    [SerializeField] private float minWaveAmplitude = 0.5f;
    [SerializeField] private float maxWaveAmplitude = 2.5f;
    [SerializeField] private float minWaveFrequency = 1f;
    [SerializeField] private float maxWaveFrequency = 4f;

    [Header("Traiettoria")]
    [SerializeField] private float targetDistance = 10f;
    [SerializeField] private int trajectoryResolution = 50;

    private LineRenderer lineRenderer;
    private Vector3[] pathPoints;
    private int currentPathIndex = 0;

    private int laneIndex = 0;
    public int LaneIndex
    {
        get => laneIndex;
        set => laneIndex = value;
    }

    private float waveAmplitude;
    private float waveFrequency;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void Start()
    {
        waveAmplitude = Random.Range(minWaveAmplitude, maxWaveAmplitude);
        waveFrequency = Random.Range(minWaveFrequency, maxWaveFrequency);

        GenerateTrajectory();
        transform.position = pathPoints[0]; // Start esattamente dal primo punto
    }

    private void Update()
    {
        if (currentPathIndex >= pathPoints.Length)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 target = pathPoints[currentPathIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
            currentPathIndex++;
    }

    private void GenerateTrajectory()
    {
        pathPoints = new Vector3[trajectoryResolution];
        lineRenderer.positionCount = trajectoryResolution;

        Vector3 start = transform.position;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i / (float)(trajectoryResolution - 1);
            float yOffset = t * targetDistance;
            float y = start.y - yOffset;
            float time = yOffset / speed;

            float x = start.x + Mathf.Sin(time * waveFrequency) * waveAmplitude;

            Vector3 point = new Vector3(x, y, 0);
            pathPoints[i] = point;
            lineRenderer.SetPosition(i, point);
        }
    }

    private void SetupLineRenderer()
    {
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.widthMultiplier = 0.15f;
        lineRenderer.useWorldSpace = true;
    }
}