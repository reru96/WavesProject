using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private List<PoolEntry> poolEntries = new List<PoolEntry>();

    public static ObjectPooler Instance;
    private Dictionary<CreatureSO, Queue<GameObject>> poolDictionary =
        new Dictionary<CreatureSO, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
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
            GameObject obj = Instantiate(data.prefab, transform);
            obj.SetActive(false);

            var marker = obj.GetComponent<PooledMarker>();
            if (marker == null) marker = obj.AddComponent<PooledMarker>();
            marker.data = data;

            queue.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject(CreatureSO data, Vector3 position, Quaternion rotation)
    {
        GameObject newObj = Instantiate(data.prefab, position, rotation, transform);

        var marker = newObj.GetComponent<PooledMarker>();
        if (marker == null) marker = newObj.AddComponent<PooledMarker>();
        marker.data = data;

        return newObj;
    }

    public GameObject Spawn(CreatureSO data, Vector3 position, Quaternion rotation)
    {
        if (data == null)
        {
            Debug.LogWarning("[ObjectPooler] Spawn: data null");
            return null;
        }

        if (!poolDictionary.ContainsKey(data))
        {
            Debug.LogWarning($"[ObjectPooler] Pool non disponibile per {data.name}. Creazione di un nuovo oggetto al volo.");
            return CreateNewObject(data, position, rotation);
        }

        var pool = poolDictionary[data];

        if (pool.Count > 0)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();

                if (obj != null)
                {
                    obj.transform.SetPositionAndRotation(position, rotation);
                    obj.SetActive(true);

                    pool.Enqueue(obj);

                    return obj;
                }
            }
        }

        Debug.LogWarning($"[ObjectPooler] Pool vuoto o inaffidabile per {data.name}. Creazione di un nuovo oggetto.");
        return CreateNewObject(data, position, rotation);
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
