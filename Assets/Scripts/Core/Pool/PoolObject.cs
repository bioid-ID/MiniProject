using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public virtual void OnSpawn()
    {
    }

    public virtual void OnDespawn()
    {
    }
}