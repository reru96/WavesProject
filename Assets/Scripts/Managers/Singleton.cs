using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    protected static T _instance;
    protected static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            if (_isQuitting)
                return null;

            if (_instance == null)
            {
             
                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);

                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}
