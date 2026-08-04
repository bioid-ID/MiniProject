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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RegisterPool<T>(ObjectPool<T> pool)
        where T : PoolObject
    {
        pools[typeof(T)] = pool;
    }

    public void RegisterPool<T>(T prefab, int initialSize, Transform parent = null)
        where T : PoolObject
    {
        if (prefab == null)
        {
            Debug.LogError($"PoolManager: {typeof(T).Name} prefab is null.");
            return;
        }

        if (parent == null)
            parent = transform;

        ObjectPool<T> pool = new ObjectPool<T>(prefab, initialSize, parent);
        pool.Initialize();
        RegisterPool(pool);
    }

    public T Get<T>()
        where T : PoolObject
    {
        if (!pools.TryGetValue(typeof(T), out IPool pool))
        {
            Debug.LogError($"PoolManager: {typeof(T).Name} pool is not registered.");
            return null;
        }

        return ((ObjectPool<T>)pool).Get();
    }

    public bool IsRegistered<T>() where T : PoolObject
    {
        return pools.ContainsKey(typeof(T));
    }

    public void Return<T>(T obj)
        where T : PoolObject
    {
        if (obj == null)
            return;

        if (!pools.TryGetValue(typeof(T), out IPool pool))
        {
            Debug.LogError($"PoolManager: {typeof(T).Name} pool is not registered.");
            return;
        }

        ((ObjectPool<T>)pool).Return(obj);
    }
}