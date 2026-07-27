using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IPool where T : PoolObject
{
    private readonly Queue<T> pool = new();

    private readonly T prefab;
    private readonly int initialSize;
    private readonly Transform parent;

    public ObjectPool(
        T prefab,
        int initialSize,
        Transform parent)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.parent = parent;
    }

    public void Initialize()
    {
        for (int i = 0; i < initialSize; i++)
        {
            Create();
        }
    }

    private T Create()
    {
        T obj = Object.Instantiate(prefab, parent);

        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);

        return obj;
    }

    public T Get()
    {
        if (pool.Count == 0)
            Create();

        T obj = pool.Dequeue();

        obj.gameObject.SetActive(true);

        obj.OnSpawn();

        return obj;
    }

    public void Return(T obj)
    {
        obj.OnDespawn();

        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);
    }
}