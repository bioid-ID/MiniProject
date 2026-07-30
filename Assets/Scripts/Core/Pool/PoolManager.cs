using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<System.Type, IPool> pools = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPool<T>(ObjectPool<T> pool)
        where T : PoolObject
    {
        pools[typeof(T)] = pool;
    }

    public T Get<T>()
        where T : PoolObject
    {
        return ((ObjectPool<T>)pools[typeof(T)]).Get();
    }

    public void Return<T>(T obj)
        where T : PoolObject
    {
        ((ObjectPool<T>)pools[typeof(T)]).Return(obj);
    }
}