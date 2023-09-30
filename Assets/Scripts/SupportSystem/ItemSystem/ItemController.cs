using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The system that controls the item and inventory behavior
/// </summary>
public class ItemController : BaseController<ItemController>
{
    public int money;   // the money player own

    

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

public enum PotionEffect
{
    Heal
}