using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipEnchant", menuName = "ProjectDungeon/EquipEnchant", order = 0)]
public class EquipEnchant : ScriptableObject {
    
    public string enchant_id;       // id of enchant
    public string enchant_name;     // name of enchant

    public string enchant_describe; // describe of enchant

    public List<EquipType> avail_type;
}