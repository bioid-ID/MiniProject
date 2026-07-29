using UnityEngine;

[System.Serializable]
public class SkillNode
{
    public SkillData skill;

    public int currentLevel;

    public int maxLevel = 10;

    public int unlockCost = 1;

    public bool unlocked;

    public SkillNode[] nextNodes;
}