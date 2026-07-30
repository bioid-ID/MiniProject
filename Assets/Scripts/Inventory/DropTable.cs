using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop/DropTable")]
public class DropTable : ScriptableObject
{
    public List<DropEntry> drops = new();
}