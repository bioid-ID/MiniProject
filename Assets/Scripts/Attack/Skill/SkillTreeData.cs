using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Skill Tree")]
public class SkillTreeData : ScriptableObject
{
    public List<SkillNode> nodes = new();
}