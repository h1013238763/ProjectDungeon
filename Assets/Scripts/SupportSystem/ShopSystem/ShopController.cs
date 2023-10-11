using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control all shop behaviors including buy, sell, generate items
/// </summary>
public class ShopController : BaseController<ShopController>
{
    private Dictionary<string, int> shop_equip = new Dictionary<string, int>();     // equipment shop
    private Dictionary<string, int> shop_potion = new Dictionary<string, int>();    // potion shop
    private Dictionary<string, int> shop_item = new Dictionary<string, int>();      // item shop

    // equip related shop caps
    private List<string> unlock_equip;
    private int equip_level_cap;
    private int equip_tier_cap;
    private int equip_slot_cap;

    // potion related shop caps
    private List<string> unlock_potion;
    private int potion_slot_cap;
    private int potion_num_cap;

    // item related shop caps
    private List<string> unlock_item;
    private int item_slot_cap;
    private int item_num_cap;

    /// <summary>
    /// get the dictionary represent the shop

    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int > GetShop()
    {
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RefreshShop()
    {

    }

}
