using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimoPlayerMovement : MonoBehaviour
{
    [Header("Curva")]
    [SerializeField] private int curveResolution = 30;
    [SerializeField] private float curveHeight = 2f;
    [SerializeField] private float scrollSensitivity = 0.5f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 5f;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isDrawing = false;
    private Vector3 endPoint;
    private List<Vector3> bezierPoints = new();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Scroll modifica ampiezza
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            curveHeight += scroll * scrollSensitivity;
            curveHeight = Mathf.Clamp(curveHeight, minHeight, maxHeight);
        }

        // Inizio disegno
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
        }

        // Fine disegno e inizio movimento
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            StartCoroutine(FollowCurve());
        }

        // Disegna dinamicamente
        if (isDrawing)
        {
            Vector3 startPoint = transform.position;
            endPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            endPoint.z = 0f;

            Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up * curveHeight;

            bezierPoints = GenerateQuadraticBezierPoints(startPoint, controlPoint, endPoint);
            DrawCurve(bezierPoints);
        }
    }

    List<Vector3> GenerateQuadraticBezierPoints(Vector3 start, Vector3 control, Vector3 end)
    {
        List<Vector3> points = new();
        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 point = Mathf.Pow(1 - t, 2) * start +
                            2 * (1 - t) * t * control +
                            Mathf.Pow(t, 2) * end;
            points.Add(point);
        }
        return points;
    }

    void DrawCurve(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    IEnumerator FollowCurve()
    {
        foreach (Vector3 point in bezierPoints)
        {
            while (Vector3.Distance(transform.position, point) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

}
