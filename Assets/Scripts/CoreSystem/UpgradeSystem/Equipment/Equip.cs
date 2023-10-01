using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define all equipments attributes
/// Author: Xiaoyue Zhang
/// Last Change: 9/30
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
        Equip temp = ItemController.Controller().InfoEquip(id);
        item_id = temp.item_id;
        item_name = temp.item_name;
        item_describe = temp.item_describe;
        item_price = temp.item_price;
        item_tier = temp.item_tier;
        equip_attack = temp.equip_attack;
        equip_defense = temp.equip_defense;
        equip_health = temp.equip_health;
        equip_level = temp.equip_level;
        equip_tag = new EquipTag[item_tier];
        equip_type = temp.equip_type;
    }

    
}
