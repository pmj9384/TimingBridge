using UnityEngine;

// 어떤 클래스든 상속만 받으면 싱글톤으로 만들어주는 베이스 클래스
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            if (_isQuitting) return null;

            if (_instance == null)
            {
                // 1. 씬에 이미 있는지 확인
                _instance = FindFirstObjectByType<T>();

                // 2. 없다면 새로 생성
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 씬에 이미 인스턴스가 있다면 새로 깨어난 놈을 파괴 (중복 방지)
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
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
        _isQuitting = true;
    }
}