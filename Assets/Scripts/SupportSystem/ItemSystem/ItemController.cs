using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The system that controls the item and inventory behavior
/// control all item base object dictionary
/// Author: Xiaoyue Zhang
/// Last Change: 9/30
/// </summary>
public class ItemController : BaseController<ItemController>
{
    public int money;   // the money player own
    public List<Equip> invent_equip = new List<Equip>();    // the inventory for equipments
    private XmlDictionary<Item, int> invent_item = new XmlDictionary<Item, int>();   // the inventory for items
    private XmlDictionary<Potion, int> invent_potion = new XmlDictionary<Potion, int>(); // the inventory for potions

    private Dictionary<string, Equip> dict_equip = new Dictionary<string, Equip>(); // equip dictionary for searching
    private Dictionary<string, Item> dict_item = new Dictionary<string, Item>();    // item dictionary for searching
    private Dictionary<string, Potion> dict_potion = new Dictionary<string, Potion>();  // potion dictionary for searching

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
    /// <param name="equip">the index of equipment to remove</param>
    public void RemoveEquip(int index)
    {
        // loop through list, if find target equipment, remove it
        if( invent_equip.Count > index)
            invent_equip[index] = null;
    }

    /// <summary>
    /// Get the information about target equipment
    /// </summary>
    /// <param name="id">id of equipment</param>
    /// <returns>target equipment</returns>
    public Equip InfoEquip(string id)
    {
        if(dict_equip.ContainsKey(id))
            return dict_equip[id];
        return null;
    }

    /// <summary>
    /// Add items into player inventory
    /// </summary>
    /// <param name="id">id of the item</param>
    /// <param name="num">num to add</param>
    public void GetItem(string id, int num)
    {
        // item exist check
        if( !dict_item.ContainsKey(id) )
            return;
        // add item into inventory
        if(invent_item.ContainsKey(dict_item[id]))
            invent_item[dict_item[id]] += num;
        else
            invent_item.Add(dict_item[id], num);
    }

    /// <summary>
    /// Remove items from player inventory
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num"> num to remove</param>
    public void RemoveItem(string id, int num)
    {
        // item exist check
        if( !dict_item.ContainsKey(id) )
            return;

        if(invent_item.ContainsKey(dict_item[id]))
        {
            invent_item[dict_item[id]] -= num;
            if(invent_item[dict_item[id]] <= 0)
                invent_item.Remove(dict_item[id]);
        }     
    }

    /// <summary>
    /// Get the information about target item
    /// </summary>
    /// <param name="id">id of item</param>
    /// <returns>target item</returns>
    public Item InfoItem(string id)
    {
        if(dict_item.ContainsKey(id))
            return dict_item[id];
        return null;
    }

    /// <summary>
    /// Add potions into player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to add</param>
    public void GetPotion(string id, int num)
    {
        // potion exist check
        if( !dict_potion.ContainsKey(id) )
            return;

        if(invent_potion.ContainsKey(dict_potion[id]))
            invent_potion[dict_potion[id]] += num;
        else
            invent_potion.Add(dict_potion[id], num);
    }

    /// <summary>
    /// Remove potions from player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to remove</param>
    public void RemovePotion(string id, int num)
    {
        // potion exist check
        if( !dict_potion.ContainsKey(id) )
            return;

        if(invent_potion.ContainsKey(dict_potion[id]))
        {
            invent_potion[dict_potion[id]] -= num;
            if(invent_potion[dict_potion[id]] <= 0)
                invent_potion.Remove(dict_potion[id]);
        } 
    }

    /// <summary>
    /// Get the information about target potion
    /// </summary>
    /// <param name="id">id of potion</param>
    /// <returns>target potion</returns>
    public Potion InfoPotion(string id)
    {
        if(dict_potion.ContainsKey(id))
            return dict_potion[id];
        return null;
    }
}