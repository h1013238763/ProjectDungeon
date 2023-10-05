using System.Collections;
using System.Collections.Generic;
using System;
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
    public int equip_max = 150;     // the maximum capacity of equip inventory
    private List<Equip> invent_equip = new List<Equip>();    // the inventory for equipments
    private List<Item> invent_item = new List<Item>();   // the inventory for items
    private List<Potion> invent_potion = new List<Potion>(); // the inventory for potions
    private List<Item> invent_search = new List<Item>();

    private Dictionary<string, Equip> dict_equip = new Dictionary<string, Equip>(); // equip dictionary for searching
    private Dictionary<string, Item> dict_item = new Dictionary<string, Item>();    // item dictionary for searching
    private Dictionary<string, Potion> dict_potion = new Dictionary<string, Potion>();  // potion dictionary for searching


    public ItemController()
    {
        // dictionary initial
        // equips
        foreach(Equip equip in Resources.LoadAll<Equip>("Objects/Equip"))
            dict_equip.Add(equip.item_id, equip);
        // potions
        foreach(Potion potion in Resources.LoadAll<Potion>("Objects/Potion"))
            dict_potion.Add(potion.item_id, potion);
        // items
        foreach(Item item in Resources.LoadAll<Item>("Objects/Item"))
            dict_item.Add(item.item_id, item);

        // player invent initial
        InitialInventList<Equip>(dict_equip, invent_equip);
        InitialInventList<Potion>(dict_potion, invent_potion);
        InitialInventList<Item>(dict_item, invent_item);
    }

    /// <summary>
    /// Initial all invent list
    /// </summary>
    public void InitialInventList<T>(Dictionary<string, T> dict, List<T> invent) where T : Item
    {
        foreach(var pair in dict)
            if(pair.Value.item_num > 0)
                InsertIntoInvent<T>(invent, pair.Value);
    }

    /// <summary>
    /// Add a equipment into player inventory
    /// </summary>
    /// <param name="equip">target equipment to add</param>
    public void GetEquip( Equip equip )
    {
        if(invent_equip.Count >= equip_max)
            return;
        // loop to find slot for inserting
        InsertIntoInvent<Equip>(invent_equip, equip);
    }

    /// <summary>
    /// remove a equipment from player inventory
    /// </summary>
    /// <param name="equip">the index of equipment to remove</param>
    public void RemoveEquip(int index)
    {
        if( invent_equip.Count > index)
            invent_equip.RemoveAt(index);
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
    /// Add potions into player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to add</param>
    public void GetPotion(string id, int num)
    {
        // invent item exist check
        if(dict_potion[id].item_num == 0 && num != 0)
            InsertIntoInvent<Potion>(invent_potion, dict_potion[id]);
        // add number
        dict_potion[id].item_num += num;
    }

    /// <summary>
    /// Remove potions from player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to remove</param>
    public void RemovePotion(string id, int num)
    {
        dict_potion[id].item_num -= num;

        if(dict_potion[id].item_num <= 0)
        {
            RemoveFromInvent<Potion>(invent_potion, dict_potion[id]);
            dict_potion[id].item_num = 0;
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

    /// <summary>
    /// Add items into player inventory
    /// </summary>
    /// <param name="id">id of the item</param>
    /// <param name="num">num to add</param>
    public void GetItem(string id, int num)
    {
        // invent item exist check
        if(dict_item[id].item_num == 0 && num != 0)
            InsertIntoInvent<Item>(invent_item, dict_item[id]);
        // add number
        dict_item[id].item_num += num;
    }

    /// <summary>
    /// Remove items from player inventory
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num"> num to remove</param>
    public void RemoveItem(string id, int num)
    {
        dict_item[id].item_num -= num;

        if(dict_item[id].item_num <= 0)
        {
            RemoveFromInvent<Item>(invent_item, dict_item[id]);
            dict_item[id].item_num = 0;
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
    /// show player all the item contains the name token
    /// </summary>
    /// <param name="token">name token</param>
    private void SearchItemByName(string token, string type)
    {
        // equip search
        switch(type)
        {
            case "Equip":
                foreach(Equip equip in invent_equip)
                    if(equip.item_name.Contains(token))
                        invent_search.Add(equip);
                break;
            case "Item":
                foreach(Item item in invent_item)
                    if(item.item_name.Contains(token))
                        invent_search.Add(item);
                break;
            case "Potion":
                foreach(Potion potion in invent_potion)
                    if(potion.item_name.Contains(token))
                        invent_search.Add(potion);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Return target invent page to the inventory panel
    /// </summary>
    /// <param name="invent"> GUI Invent panel list</param>
    /// <param name="invent_name"> target invent to display</param>
    /// <param name="page"> target page to display</param>
    /// <param name="token"> token for searching if need </param>
    /// <returns> total pages of this invent</returns>
    public int GetInvent(List<Item> display, string invent_name, int page, string token = "")
    {
        switch(invent_name)
        {
            case "Equip":
                return IGetInvent<Equip>(display, invent_equip, page, token);
            case "Item":
                return IGetInvent<Item>(display, invent_item, page, token);
            case "Potion":
                return IGetInvent<Potion>(display, invent_potion, page, token);
            default:
                return 0;
        }
    }
    // the implement of GetInvent function
    private int IGetInvent<T>( List<Item> display, List<T> invent, int page, string token = "") where T : Item
    {
        display.Clear();
        for(int i = page*30; i < (page+1)*30 && i < invent.Count; i ++)
        { 
            // if player is searching
            if(token != "")
            {
                if(invent[i].item_name.Contains(token))
                    display.Add(invent[i]);
            }
            // else
            else
                display.Add(invent[i]);
        }
        return (int)(invent.Count/30);
    }

    /// <summary>
    /// Insert item into inventory
    /// </summary>
    /// <param name="invent"> target invent</param>
    /// <param name="item"> item to add</param>
    /// <typeparam name="T"> type of item </typeparam>
    private void InsertIntoInvent<T>(List<T> invent, T item) where T : Item
    {
        for(int i = 0; i < invent.Count; i ++)
        {
            // first sort by tier
            if(invent[i].item_tier <= item.item_tier)
                // then sort by id
                if(String.Compare(invent[i].item_id, item.item_id) <= 0 )
                {
                    invent.Insert(i, item);
                    return;
                }
        }
        invent.Add(item);
    }

    /// <summary>
    /// remove item from inventory
    /// </summary>
    /// <param name="invent"> target invent </param>
    /// <param name="item"> item to remove </param>
    /// <typeparam name="T"> type of item</typeparam>
    private void RemoveFromInvent<T>(List<T> invent, T item) where T : Item
    {
        for(int i = 0; i < invent.Count; i ++)
            if(invent[i].item_id == item.item_id)
            {
                invent.RemoveAt(i);
                return;
            }
    }
}