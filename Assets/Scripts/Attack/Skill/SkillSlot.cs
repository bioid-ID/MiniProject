using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public SkillData skillData;
    public int level = 1;

    public SkillBase runtimeSkill;
}