using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Equip : Item
{
    public int equip_level;         // level of equipment
    public EquipTag[] equip_tag;    // tag of equipment
    public EquipType equip_type;    // type of equipment 

    public Equip(string id, int level, int tier) : base(id, 1)
    {
        EquipBase temp = ItemController.Controller().DictEquipInfo(id);
        item_id = temp.item_id;
        equip_level = level;
        item_tier = tier;
        equip_tag = (item_tier == 5) ? new EquipTag[item_tier] : new EquipTag[item_tier-1] ;
        equip_type = temp.equip_type;
    }

    public override string ToString()
    {
        string re = "Equip ["+item_id+", "+equip_level+", "+equip_tag.Length+"[";
        for(int i = 0; i < equip_tag.Length; i ++)
            re += equip_tag[i].ToString();
        re += "]";
        return re;
    }
}
