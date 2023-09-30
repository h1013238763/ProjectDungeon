using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define all equipments attributes
/// Author: Xiaoyue Zhang
/// Last Change: 9/29
/// </summary>
[CreateAssetMenu(fileName = "New Equipment", menuName = "ProjectDungeon/Equipment", order = 1)]
public class Equip : Item
{
    public int equip_attack;        // attack of equipment
    public int equip_defense;       // defense of equipment
    public int equip_health;        // health of equipment
    public int equip_level;         // level of equipment
    public EquipTag[] equip_tag;    // tag of equipment
    public EquipType equip_type;    // type of equipment 

    public Equip(string id)
    {

    }
}
