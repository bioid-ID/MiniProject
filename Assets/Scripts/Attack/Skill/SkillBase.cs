using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected SkillData data;

    private float cooldown;

    public SkillData Data => data;

    public bool CanUse =>
        cooldown <= 0f;

    public virtual void Tick(float dt)
    {
        if (cooldown > 0f)
            cooldown -= dt;
    }

    public virtual bool TryUse()
    {
        if (!CanUse)
            return false;

        cooldown = data.cooldown;

        Use();

        return true;
    }

    protected abstract void Use();
}