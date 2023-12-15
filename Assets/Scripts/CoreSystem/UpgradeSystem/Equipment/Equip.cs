using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Equip : Item
{
    public int equip_level;         // level of equipment
    public List<EquipEnchant> equip_enchants;    // tag of equipment
    public int enchant_limit;
    public EquipType equip_type;    // type of equipment 

    public Equip() : base("", 1)
    {
        item_id = "";
        equip_level = 1;
        item_tier = 1;
        enchant_limit = 0;
        equip_enchants = new List<EquipEnchant>();
        equip_type = EquipType.Weapon;
    }

    public Equip(string id, int level, int tier) : base(id, 1)
    {
        EquipBase temp = ItemController.Controller().DictEquipInfo(id);
        item_id = temp.item_id;
        equip_level = level;
        item_tier = tier;
        
        enchant_limit = (item_tier == 5) ? 5 : item_tier-1;
        equip_enchants = new List<EquipEnchant>();

        for(int i = 0; i < enchant_limit; i ++)
        {
            equip_enchants.Add(EnchantController.Controller().GetRandomEnchant(equip_type));
        }
        

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
        float result = 0;
        int enchant_count = 0;
        float increase = 0;
        string id = "";

        if(value == "Attack")
        {
            result = (base_data.equip_attack + base_data.equip_attack_grow * 10f * equip_level) * (1 + item_tier * 0.04f);
            id = "FortifyAttack";
        }
        else if(value == "Defense")
        {
            result = (base_data.equip_defense + base_data.equip_defense_grow * 10f * equip_level) * (1 + item_tier * 0.04f);
            id = "FortifyDefense";
        }
        else if(value == "Health")
        {
            result = (base_data.equip_health + base_data.equip_health_grow * 10f* equip_level) * (1 + item_tier * 0.04f);
            id = "FortifyHealth";
        }

        foreach(EquipEnchant enchant in equip_enchants)
        {
            if(enchant.enchant_id == id)
            {
                enchant_count ++;
                increase = enchant.enchant_value;
            }
        }

        return (int)(result * (1 + increase * enchant_count));
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
        EquipBase base_data = ItemController.Controller().DictEquipInfo(item_id);
        
        string re = "Equip ["+item_id+", lv: "+equip_level+", t: "+item_tier+", p:"+base_data.item_price+", "+enchant_limit+"[";
        for(int i = 0; i < equip_enchants.Count; i ++)
            re += equip_enchants[i].ToString();
        re += "]";
        return re;
    }

    
}
