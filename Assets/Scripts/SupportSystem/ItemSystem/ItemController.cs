using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The system that controls the item and inventory behavior
/// Author: Xiaoyue Zhang
/// Last Change: 9/29
/// </summary>
public class ItemController : BaseController<ItemController>
{
    public int money;   // the money player own
    public List<Equip> invent_equip = new List<Equip>();    // the inventory for equipments
    public XmlDictionary<string, int> invent_item = new XmlDictionary<string, int>();   // the inventory for items
    public XmlDictionary<string, int> invent_potion = new XmlDictionary<string, int>(); // the inventory for potions

    /// <summary>
    /// Add a equipment into player inventory
    /// </summary>
    /// <param name="equip">target equipment to add</param>
    public void GetEquip( Equip equip )
    {
        // Prioritize placing equipment in the previously empty slot
        for(int i = 0; i < invent_equip.Count; i ++)
        {
            if(invent_equip[i] == null)
            {
                invent_equip[i] = equip;
                return;
            }
        }
        // if no empty grid, place into new slot
        invent_equip.Add(equip);
    }

    /// <summary>
    /// remove a equipment from player inventory
    /// </summary>
    /// <param name="equip">target equipment to remove</param>
    public void RemoveEquip(Equip equip)
    {
        // loop through list, if find target equipment, remove it
        for(int i = 0; i < invent_equip.Count; i ++)
        {
            if(invent_equip[i] == equip)
            {
                invent_equip[i] = null;
                return;
            }
        }
    }

    /// <summary>
    /// return the information about the Equipment stored in target slot
    /// </summary>
    /// <param name="index">the index of slot in inventory</param>
    /// <returns>the information about the Equipment</returns>
    public Equip GetEquipInfo(int index)
    {
        return invent_equip[index];
    }

    /// <summary>
    /// create a new target equipment with random tag
    /// </summary>
    /// <param name="id">target equipment base</param>
    /// <returns>target equipment</returns>
    public Equip GenerateEquip(string id)
    {
        // TODO: Equipment tag randomize generate
        Equip equip = new Equip(id);
        return equip;
    }

    /// <summary>
    /// Add items into player inventory
    /// </summary>
    /// <param name="id">id of the item</param>
    /// <param name="num">num to add</param>
    public void GetItem(string id, int num)
    {
        if(invent_item.ContainsKey(id))
            invent_item[id] += num;
        else
            invent_item.Add(id, num);
    }

    /// <summary>
    /// Remove items from player inventory
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num"> num to remove</param>
    public void RemoveItem(string id, int num)
    {
        if(invent_item.ContainsKey(id))
        {
            invent_item[id] -= num;
            if(invent_item[id] <= 0)
                invent_item.Remove(id);
        }     
    }

    /// <summary>
    /// check if there are enough target items
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num">goal number</param>
    /// <returns>true if player have enough target item</returns>
    public bool CheckItemNum(string id, int num)
    {
        if(invent_item.ContainsKey(id))
            if(invent_item[id] >= num)
                return true;
        return false;
    }

    /// <summary>
    /// Add potions into player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to add</param>
    public void GetPotion(string id, int num)
    {
        if(invent_potion.ContainsKey(id))
            invent_potion[id] += num;
        else
            invent_potion.Add(id, num);
    }

    /// <summary>
    /// Remove potions from player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to remove</param>
    public void RemovePotion(string id, int num)
    {
        if(invent_potion.ContainsKey(id))
        {
            invent_potion[id] -= num;
            if(invent_potion[id] <= 0)
                invent_potion.Remove(id);
        } 
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

public enum PotionEffect
{
    Heal
}