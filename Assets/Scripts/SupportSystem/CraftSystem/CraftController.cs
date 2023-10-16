using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A crafting system for all items and 
/// </summary>
public class CraftController : BaseController<CraftController>
{
    public Dictionary<string, Recipe> dict_recipe = new Dictionary<string, Recipe>();
    public int equip_level_cap; // the level cap of equipment for strengthen
    public string equip_strengthen_item = "StrengthenShard";   // the string id of item for equip strengthen
    public string equip_enchant_item = "EnchantmentShard";   // the string id of item for equip enchantment
    public int strengthen_item_cost = 1;
    public int enchant_item_cost = 1;

    public CraftController()
    {
        foreach(Recipe list in Resources.LoadAll<Recipe>("Objects/Recipe"))
            dict_recipe.Add(list.recipe_id, list);
    }

    /// <summary>
    /// Check how many times this recipe can be made
    /// </summary>
    /// <param name="id">the id of recipe</param>
    /// <returns>maximum time could product</returns>
    public int RecipeProductTimeCheck(string id)
    {
        if(!dict_recipe.ContainsKey(id))
            return 0;

        int maximum = -1;
        int product_time = 0;
        Item consume_item;

        for( int i = 0; i < dict_recipe[id].recipe_consume.Length; i ++)
        {
            consume_item = ItemController.Controller().InventItemInfo(dict_recipe[id].recipe_consume[i]);
            if(consume_item != null)
                product_time = consume_item.item_num / dict_recipe[id].recipe_consume_num[i];
            else
                product_time = 0;
            if(product_time < maximum || maximum == -1)
                maximum = product_time;
        }
        
        return maximum;
    }

    /// <summary>
    /// craft potions
    /// </summary>
    /// <param name="id">the id of recipe</param>
    /// <param name="num">the time to craft</param>
    public void CraftPotion( string id, int num)
    {
        if(!dict_recipe.ContainsKey(id))
            return;
        
        // get potion
        ItemController.Controller().GetPotion(dict_recipe[id].recipe_result, dict_recipe[id].recipe_result_num * num);

        // remove consume item
        for(int i = 0; i < dict_recipe[id].recipe_consume.Length; i ++)
            ItemController.Controller().RemoveItem(dict_recipe[id].recipe_consume[i], dict_recipe[id].recipe_consume_num[i] * num);
    }

    public int LevelCostCheck()
    {
        Item item = ItemController.Controller().InventItemInfo(equip_strengthen_item);
        if( item == null)
            return 0;
        return ItemController.Controller().InventItemInfo(equip_strengthen_item).item_num / strengthen_item_cost;
    }

    public int EnchantmentCostCheck()
    {
        Item item = ItemController.Controller().InventItemInfo(equip_enchant_item);
        if( item == null)
            return 0;
        return ItemController.Controller().InventItemInfo(equip_enchant_item).item_num / enchant_item_cost;
    }

    /// <summary>
    /// Use equip strengthen item to level up equipment
    /// </summary>
    /// <param name="equip"></param>
    public void StrengthenEquip( Equip equip, int level)
    {
        Item item = ItemController.Controller().InventItemInfo(equip_strengthen_item);
        
        ItemController.Controller().RemoveItem(equip_strengthen_item, strengthen_item_cost * level);
        equip.equip_level += level;
    }

    /// <summary>
    /// Reset part of enchantment
    /// </summary>
    /// <param name="equip"> target equipment to reset </param>
    /// <param name="num"> index of slots to reset </param>
    public void ResetEnchantment( Equip equip, List<int> index )
    {
        for(int i = 0; i < index.Count; i ++)
        {
            equip.equip_tag[index[i]] = EquipController.Controller().GetRandomEquipTag(equip.equip_type);
        }
        ItemController.Controller().RemoveItem(equip_enchant_item, enchant_item_cost * index.Count);
    }
}