using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Buff/Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("Info")]
    public string buffName;

    public Sprite icon;

    public BuffType buffType;

    [Header("Duration")]
    public bool isInfinite;

    public float duration = 5f;

    [Header("Stack")]
    public bool canStack;

    public int maxStack = 1;

    [Header("Modifiers")]
    public List<StatModifierData> modifiers = new();
}