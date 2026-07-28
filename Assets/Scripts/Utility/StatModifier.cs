using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public StatType statType;

    public ModifierType modifierType;

    public float value;

    public object source;

    public StatModifier(
        StatType statType,
        ModifierType modifierType,
        float value,
        object source = null)
    {
        this.statType = statType;
        this.modifierType = modifierType;
        this.value = value;
        this.source = source;
    }
}