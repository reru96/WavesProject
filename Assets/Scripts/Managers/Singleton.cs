using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;
    private static bool _isApplicationQuitting = false;
    protected virtual bool ShouldBeDestroyOnLoad() => false;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (_instance != null && !_isApplicationQuitting) 
                {
                    GameObject gameObject = new GameObject(typeof(T).ToString());
                    _instance = gameObject.AddComponent<T>();
                }
             
            } 
            return _instance;
        }   
       
    }
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (!ShouldBeDestroyOnLoad())
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != null)
        {
            Destroy(gameObject);
        }
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
        _isApplicationQuitting = true;
    }

}
