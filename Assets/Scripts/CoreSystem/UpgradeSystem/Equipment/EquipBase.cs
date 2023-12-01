using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define all equipments attributes
/// Author: Xiaoyue Zhang
/// Last Change: 9/30
/// </summary>
[CreateAssetMenu(fileName = "New Equipment", menuName = "ProjectDungeon/Equipment", order = 1)]
public class EquipBase : ItemBase
{
    public int equip_attack;        // attack of equipment
    public int equip_attack_grow;   // attack to add per level
    public int equip_defense;       // defense of equipment
    public int equip_defense_grow;  // defense to add per level
    public int equip_health;        // health of equipmen
    public int equip_health_grow;   // health to add per level
    public EquipType equip_type;    // type of equipment 
}

public enum EquipType
{
    Weapon = 0,
    Ring = 1,
    Helmet = 2,
    Breastplate = 3,
    LegArmor = 4,
    FootArmor = 5
}