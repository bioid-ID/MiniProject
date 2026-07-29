using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;

    public Sprite icon;

    public SkillType skillType;

    public SkillTriggerType trigger;

    public SkillTargetType target;

    public float cooldown = 1f;

    public float damage = 10f;

    public float range = 5f;

    public int maxLevel = 10;

    public GameObject prefab;

    public float duration;

    public float radius;

    public bool autoCast = true;
}