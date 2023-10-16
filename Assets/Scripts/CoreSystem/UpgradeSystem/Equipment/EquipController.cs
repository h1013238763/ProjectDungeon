using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control all equipment tags and behavior like equip, unequip, generate
/// Author: Xiaoyue Zhang
/// Last Change: 9/30
/// </summary>
public class EquipController : BaseController<EquipController>
{
    private Dictionary<EquipType, List<EquipTag>> dict_tag = new Dictionary<EquipType, List<EquipTag>>();

    public EquipController()
    {
        foreach(EquipTagList list in Resources.LoadAll<EquipTagList>("Objects/Tags"))
            dict_tag.Add(list.list_type, list.tags);
    }

    /// <summary>
    /// create a new target equipment with random tag
    /// </summary>
    /// <param name="id">target equipment base</param>
    /// <returns>target equipment</returns>
    public Equip GenerateEquip(string id, int level, int tier)
    {
        Equip equip = new Equip(id, level, tier);
        for(int i = 0; i < equip.equip_tag.Length; i ++)
        {
            equip.equip_tag[i] = GetRandomEquipTag(equip.equip_type);
        }
        return equip;
    }

    public EquipTag GetRandomEquipTag( EquipType equip)
    {
        int tag = Random.Range(0, dict_tag[equip].Count-1);
        return dict_tag[equip][tag];
    }

    public string TagString(EquipTag tag)
    {
        switch(tag)
        {
            case EquipTag.Sharp:
                return "Attack + 20%";
            case EquipTag.Solid:
                return "Defense + 20%";
            case EquipTag.Robust:
                return "Health + 20%";
            default:
                return "";
        }
    }
}

public enum EquipTag
{
    Sharp,      // attack + 20%
    Solid,      // defense + 20%
    Robust     // health + 20%
}

public enum EquipType
{
    Weapon,
    Ring,
    Helmet,
    Breastplate,
    LegArmor,
    FootArmor
}
