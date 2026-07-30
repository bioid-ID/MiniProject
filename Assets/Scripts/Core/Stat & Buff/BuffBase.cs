using UnityEngine;

public abstract class BuffBase : ScriptableObject
{
    [SerializeField]
    private float duration = 5f;

    protected PlayerStat player;

    private float timer;

    public bool Finished => timer <= 0;

    public void Initialize(PlayerStat player)
    {
        this.player = player;
        timer = duration;
    }

    public void UpdateBuff(float dt)
    {
        timer -= dt;
    }

    public abstract void Apply();

    public abstract void Remove();
}