using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField]
    private List<SkillSlot> equippedSkills = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        foreach (SkillSlot slot in equippedSkills)
        {
            if (slot.runtimeSkill == null)
                continue;

            slot.runtimeSkill.Tick(dt);
        }
    }

    public void AddSkill(SkillBase skill)
    {
        SkillSlot slot = new SkillSlot();

        slot.runtimeSkill = skill;

        equippedSkills.Add(slot);
    }

    public void RemoveSkill(SkillBase skill)
    {
        equippedSkills.RemoveAll(x => x.runtimeSkill == skill);
    }

    public void UseAutoSkills()
    {
        foreach (SkillSlot slot in equippedSkills)
        {
            if (slot.runtimeSkill == null)
                continue;

            if (slot.runtimeSkill.Data.trigger != SkillTriggerType.Auto)
                continue;

            slot.runtimeSkill.TryUse();
        }
    }
}