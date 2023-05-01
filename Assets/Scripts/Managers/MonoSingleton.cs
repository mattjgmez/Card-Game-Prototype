using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null) Debug.LogError($"{typeof(T).Name} is not initialized.");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            Init();
        }
        else if (_instance != this)
        {
            // Destroy the new instance if it's not the same as the existing one
            Destroy(gameObject);
        }
    }

    protected virtual void Init() { }
}
