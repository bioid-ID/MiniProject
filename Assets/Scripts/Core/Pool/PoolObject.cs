using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public bool IsSpawned { get; private set; }

    public virtual void OnSpawn()
    {
        IsSpawned = true;
    }

    public virtual void OnDespawn()
    {
        IsSpawned = false;
    }
}