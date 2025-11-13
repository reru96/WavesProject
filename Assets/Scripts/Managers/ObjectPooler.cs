using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [SerializeField] private List<PoolEntry> poolEntries = new List<PoolEntry>();

    private Dictionary<CreatureSO, Queue<GameObject>> poolDictionary =
        new Dictionary<CreatureSO, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var entry in poolEntries)
        {
            if (entry.objectSo == null || entry.objectSo.Count == 0)
                continue;

            foreach (var obj in entry.objectSo)
            {
                if (obj is CreatureSO data && data.prefab != null)
                {
                    AddToPool(data, entry.poolsize);
                }
                else
                {
                    Debug.LogWarning($"[ObjectPooler] '{entry.name}' contiene elementi non validi: devono essere ScriptableObject derivati da BasePooledData con prefab assegnato.");
                }
            }
        }
    }

    public void AddToPool(CreatureSO data, int size)
    {
        if (data == null || data.prefab == null) return;

        if (!poolDictionary.ContainsKey(data))
            poolDictionary[data] = new Queue<GameObject>();

        var queue = poolDictionary[data];

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(data.prefab);
            obj.SetActive(false);

            var marker = obj.GetComponent<PooledMarker>();
            if (marker == null) marker = obj.AddComponent<PooledMarker>();
            marker.data = data;

            queue.Enqueue(obj);
        }
    }

    public GameObject Spawn(CreatureSO data, Vector3 position, Quaternion rotation)
    {
        if (data == null)
        {
            Debug.LogWarning("[ObjectPooler] Spawn: data null");
            return null;
        }

        if (!poolDictionary.ContainsKey(data) || poolDictionary[data].Count == 0)
        {
            Debug.LogWarning($"[ObjectPooler] Nessun pool disponibile per {data.name}");
            return null;
        }

        var obj = poolDictionary[data].Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        poolDictionary[data].Enqueue(obj);

        return obj;
    }

    public T Spawn<T>(CreatureSO data, Vector3 position, Quaternion rotation) where T : Component
    {
        var go = Spawn(data, position, rotation);
        return go != null ? go.GetComponent<T>() : null;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
    }

    public void ClearAllPools()
    {
        foreach (var kv in poolDictionary)
        {
            foreach (var go in kv.Value)
                if (go != null) Destroy(go);
        }
        poolDictionary.Clear();
    }
}

[System.Serializable] public class PoolEntry
{
    public string name;
    public int poolsize = 10;
    public List<CreatureSO> objectSo;
}
