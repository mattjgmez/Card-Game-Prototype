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
        _instance = (T)this;

        Init();
    }

    protected virtual void Init() { }
}
