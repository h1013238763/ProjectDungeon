using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A crafting system for all items and 
/// </summary>
public class CraftController : BaseController<CraftController>
{
    public Dictionary<string, Recipe> dict_recipe = new Dictionary<string, Recipe>();
    public List<string> unlock_potion = new List<string>();

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

        for( int i = 0; i < dict_recipe[id].recipe_consume.Length; i ++)
        {
            product_time = ItemController.Controller().InfoItem(dict_recipe[id].recipe_consume[i]).item_num / dict_recipe[id].recipe_consume_num[i];
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
        ItemController.Controller().GetPotion(dict_recipe[id].recipe_result, dict_recipe[id].recipe_result_num);

        // remove consume item
        for(int i = 0; i < dict_recipe[id].recipe_consume.Length; i ++)
            ItemController.Controller().RemoveItem(dict_recipe[id].recipe_consume[i], dict_recipe[id].recipe_consume_num[i]);
    }
}
