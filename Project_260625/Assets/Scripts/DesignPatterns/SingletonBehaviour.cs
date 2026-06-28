using System.Dynamic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>, new()
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<T>();

            return _instance;
        }
    }

    //////////////////////////////////////////////////////

    protected virtual void Awake()
    {
        _ = Instance;

        if (IsMyInstance() == false)
            Destroy(gameObject.GetComponent<T>());
    }

    protected virtual void OnDestroy()
    {
        if (IsMyInstance())
            _instance = null;
    }

    public virtual void Init() { }

    //////////////////////////////////////////////////////

    public static bool IsLive() => _instance != null;

    protected bool IsMyInstance() => _instance == this;
}
