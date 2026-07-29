using UnityEngine;

public class PercentStatBuff : BuffBase
{
    [SerializeField]
    private StatType stat;

    [SerializeField]
    private float value;

    private StatModifier modifier;

    public override void Apply()
    {
        modifier = new StatModifier(
            stat,
            ModifierType.Percent,
            value,
            this);

        player.AddModifier(modifier);
    }

    public override void Remove()
    {
        player.RemoveModifier(this);
    }
}