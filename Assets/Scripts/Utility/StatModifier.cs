using System;

[Serializable]
public class StatModifier
{
    public StatType Stat;

    public ModifierType Type;

    public float Value;

    public object Source;

    public StatModifier(
        StatType stat,
        ModifierType type,
        float value,
        object source)
    {
        Stat = stat;
        Type = type;
        Value = value;
        Source = source;
    }
}