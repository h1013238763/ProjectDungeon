using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to register tag list for equip types
/// </summary>
[CreateAssetMenu(fileName = "EquipTagList", menuName = "ProjectDungeon/EquipTagList", order = 0)]
public class EquipTagList : ScriptableObject {
    public EquipType list_type;         // target type
    public List<EquipTag> tags;         // tags for target type
}
