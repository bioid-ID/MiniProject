using UnityEngine;

public static class DefaultSkillFactory
{
    private static SkillData fireballSkill;

    public static SkillData GetFireballSkill()
    {
        if (fireballSkill != null)
            return fireballSkill;

        fireballSkill = ScriptableObject.CreateInstance<SkillData>();
        fireballSkill.skillName = "Fireball";
        fireballSkill.skillType = SkillType.Active;
        fireballSkill.trigger = SkillTriggerType.Auto;
        fireballSkill.target = SkillTargetType.Enemy;
        fireballSkill.cooldown = 2f;
        fireballSkill.damage = 18f;
        fireballSkill.range = 8f;
        fireballSkill.autoCast = true;

        return fireballSkill;
    }
}
