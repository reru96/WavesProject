using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LineRenderer committedLine;
    [SerializeField] private LineRenderer previewLine;
    [SerializeField] private SpriteFollower playerSprite;

    [Header("Path Parameters")]
    [SerializeField] private float timerInterval = 5f;

    [SerializeField] private float defaultAmplitude = 2f;
    [SerializeField] private float defaultWavelength = 5f;
    [SerializeField] private int pointsPerSegment = 100;

    [Header("Length")]
    [SerializeField] private float segmentLength = 10f;
    [Range(1, 25)][SerializeField] private float minSegmentLength = 5f;
    [Range(10, 100)][SerializeField] private float maxSegmentLength = 50f;

    [Header("Amplitude")]
    [Range(-10, 10)][SerializeField] public float minAmplitude = 0.5f;
    [Range(-50, 50)][SerializeField] public float maxAmplitude = 5f;

    [Header("Wavelength")]
    [Range(-10, 10)][SerializeField] public float minWavelength = 2f;
    [Range(-50, 50)][SerializeField] public float maxWavelength = 10f;

    [HideInInspector] public float CurrentAmplitude;
    [HideInInspector] public float CurrentWavelength;

    public List<Vector3> CommittedPoints { get; private set; } = new List<Vector3>();
    public List<Vector3> PreviewPoints { get; private set; } = new List<Vector3>();

    public float SegmentLength => segmentLength;
    public float SetSegmentLength(float value) => segmentLength = Mathf.Clamp(value, minSegmentLength, maxSegmentLength);

    public Action<Vector3> OnSegmentCommitted;

    void Start()
    {
        CurrentAmplitude = defaultAmplitude;
        CurrentWavelength = defaultWavelength;

        // Genera segmento iniziale committed
        CommittedPoints = GenerateSinPoints(Vector3.zero, segmentLength, defaultAmplitude, defaultWavelength);
        UpdateCommittedLine();

        // Genera preview iniziale
        UpdatePreview();

        //StartCoroutine(TimerCoroutine());
    }

    void Update()
    {
        // Aggiorna il preview in tempo reale quando i parametri cambiano
        UpdatePreview();
        AppendPreviewToCommitted();

        if (CurrentAmplitude == 0f || CurrentWavelength == 0f)
        {
            if (CurrentAmplitude  == 0f) CurrentAmplitude = minAmplitude;
            if (CurrentWavelength == 0f) CurrentWavelength = minWavelength;
        }
    }

    IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timerInterval);
            AppendPreviewToCommitted();
            if (inputManager.isDragging)
                yield return new WaitUntil(() => !inputManager.isDragging);
        }
    }

    void AppendPreviewToCommitted()
    {
        if (playerSprite.IsMoving) return;

        CommittedPoints.AddRange(PreviewPoints);
        UpdateCommittedLine();

        // Genera nuovo preview partendo dalla fine
        UpdatePreview();
    }

    void UpdatePreview()
    {
        Vector3 start = CommittedPoints.Count > 0 ? CommittedPoints[CommittedPoints.Count - 1] : Vector3.zero;
        PreviewPoints = GenerateSinPoints(start, segmentLength, CurrentAmplitude, CurrentWavelength);

        previewLine.positionCount = PreviewPoints.Count;
        previewLine.SetPositions(PreviewPoints.ToArray());
    }

    void UpdateCommittedLine()
    {
        Debug.Log($"Updating committed line with {CommittedPoints.Count} points.");
        committedLine.positionCount = CommittedPoints.Count;
        committedLine.SetPositions(CommittedPoints.ToArray());
        OnSegmentCommitted?.Invoke(CommittedPoints[CommittedPoints.Count - 1]);
    }

    List<Vector3> GenerateSinPoints(Vector3 start, float length, float amp, float wave)
    {
        List<Vector3> points = new List<Vector3>();
        float dx = length / (pointsPerSegment - 1);
        for (int i = 0; i < pointsPerSegment; i++)
        {
            float x = start.x + i * dx;
            float y = start.y + amp * Mathf.Sin(2 * Mathf.PI * (i * dx) / wave);
            points.Add(new Vector3(x, y, 0));
        }
        return points;
    }
}
