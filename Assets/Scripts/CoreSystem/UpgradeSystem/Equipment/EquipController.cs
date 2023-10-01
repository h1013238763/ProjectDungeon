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
    /// <summary>
    /// create a new target equipment with random tag
    /// </summary>
    /// <param name="id">target equipment base</param>
    /// <returns>target equipment</returns>
    public Equip GenerateEquip(string id, int tier)
    {
        // TODO: Equipment tag randomize generate
        Equip equip = new Equip(id);
        return equip;
    }
}

public enum EquipTag
{
    
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
