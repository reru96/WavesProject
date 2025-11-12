using System.Collections;
using UnityEngine;

public class SimoNotesSpawner : MonoBehaviour
{
    [SerializeField] private Transform Lane01;
    [SerializeField] private Transform Lane02;
    [SerializeField] private Transform Lane03;

    [SerializeField] private GameObject notePrefab;

    [SerializeField] private float spawnInterval = 1.5f;

    private Transform[] lanes;

    private void Awake()
    {
        lanes = new Transform[] { Lane01, Lane02, Lane03 };
    }

    private void Start()
    {
        StartCoroutine(SpawnNotesRoutine(spawnInterval));
    }
    public Transform GetRandomLane()
    {
        int randomIndex = Random.Range(0, lanes.Length);
        return lanes[randomIndex];
    }

    public void SpawnNote()
    {
        Transform selectedLane = GetRandomLane();
        GameObject note = Instantiate(notePrefab, selectedLane.position, Quaternion.identity);
        note.GetComponent<NoteLogic>().LaneIndex = System.Array.IndexOf(lanes, selectedLane);
    }

    IEnumerator SpawnNotesRoutine(float interval)
    {
        while (true)
        {
            SpawnNote();
            yield return new WaitForSeconds(interval);
        }
    }


}
