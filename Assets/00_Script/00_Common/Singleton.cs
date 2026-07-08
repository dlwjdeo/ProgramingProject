using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    public bool IsDontDestroyOnLoad;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (transform.parent != null)
                transform.SetParent(null); // Detach from parent

            if (IsDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
