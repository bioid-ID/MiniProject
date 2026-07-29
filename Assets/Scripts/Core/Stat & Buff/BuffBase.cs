using UnityEngine;

public class BuffBase
{
    public BuffData Data { get; }

    public int Stack { get; private set; }

    public float RemainingTime { get; private set; }

    private readonly PlayerStat player;

    public BuffBase(BuffData data)
    {
        Data = data;

        RemainingTime = data.duration;

        Stack = 1;

        player = PlayerStat.Instance;
    }

    public void Apply()
    {
        foreach (StatModifierData modifier in Data.modifiers)
        {
            player.AddModifier(
                new StatModifier(
                    modifier.stat,
                    modifier.modifierType,
                    modifier.value,
                    this));
        }
    }

    public void Remove()
    {
        player.RemoveModifier(this);
    }

    public bool Tick(float deltaTime)
    {
        if (Data.isInfinite)
            return false;

        RemainingTime -= deltaTime;

        return RemainingTime <= 0f;
    }

    public void Refresh()
    {
        RemainingTime = Data.duration;
    }

    public void AddStack()
    {
        if (!Data.canStack)
            return;

        Stack = Mathf.Min(
            Stack + 1,
            Data.maxStack);
    }
}