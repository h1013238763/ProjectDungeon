using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Equip : Item
{
    public int equip_level;         // level of equipment
    public List<EquipEnchant> equip_enchants;    // tag of equipment
    public int enchant_limit;

    public EquipType equip_type;    // type of equipment 

    public Equip(string id, int level, int tier) : base(id, 1)
    {
        EquipBase temp = ItemController.Controller().DictEquipInfo(id);
        item_id = temp.item_id;
        equip_level = level;
        item_tier = tier;
        
        enchant_limit = (item_tier == 5) ? 5 : item_tier-1;
        equip_enchants = new List<EquipEnchant>();

        equip_type = temp.equip_type;
    }

    public int GetAttributesIncrease(string value)
    {
        EquipBase base_data = ItemController.Controller().DictEquipInfo(item_id);

        switch(value)
        {
            case "Attack":
                return (int)(base_data.equip_attack_grow * 10 * (1 + item_tier + 0.04));  // attack
            case "Defense":
                return (int)(base_data.equip_defense_grow * 10 * (1 + item_tier + 0.04));  // defense
            case "Health":
                return (int)(base_data.equip_health_grow * 10 * (1 + item_tier + 0.04));  // health
            default:
                return 0;
        }
    }

    public int GetAttributes(string value)
    {
        EquipBase base_data = ItemController.Controller().DictEquipInfo(item_id);
        // equip attribute = ( basic attribute + attribute grow * equip level * 10 ) * (1 + tier * 0.04)
        switch(value)
        {
            case "Attack":
                return (int)((base_data.equip_attack + base_data.equip_attack_grow * 10 * equip_level) * (1 + item_tier + 0.04));  // attack
            case "Defense":
                return (int)((base_data.equip_defense + base_data.equip_defense_grow * 10 * equip_level) * (1 + item_tier + 0.04));  // defense
            case "Health":
                return (int)((base_data.equip_health + base_data.equip_health_grow * 10 * equip_level) * (1 + item_tier + 0.04)); // health
            default:
                return 0;
        }
    }

    // equip price = basic price * equip level * equip tier^2
    public override int GetPrice()
    {
        EquipBase base_data = ItemController.Controller().DictEquipInfo(item_id);
        return base_data.item_price * equip_level * item_tier * item_tier;
    }

    public void UpgradeEquip()
    {
        equip_level ++;
    }

    public override string ToString()
    {
        string re = "Equip ["+item_id+", "+equip_level+", "+enchant_limit+"[";
        for(int i = 0; i < equip_enchants.Count; i ++)
            re += equip_enchants[i].ToString();
        re += "]";
        return re;
    }

    
}
