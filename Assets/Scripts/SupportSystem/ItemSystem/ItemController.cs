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
    private List<Potion> invent_potion = new List<Potion>(); // the inventory for potions
    private List<Item> invent_item = new List<Item>();   // the inventory for items
    private List<Item> invent_search = new List<Item>();   // the inventory for search item

    private Dictionary<string, EquipBase> dict_equip = new Dictionary<string, EquipBase>(); // equip dictionary for searching
    private Dictionary<string, PotionBase> dict_potion = new Dictionary<string, PotionBase>();  // potion dictionary for searching
    private Dictionary<string, ItemBase> dict_item = new Dictionary<string, ItemBase>();    // item dictionary for searching

    // controller initial
    public ItemController()
    {
        // dictionary initial
        // equips
        foreach(EquipBase equip in Resources.LoadAll<EquipBase>("Objects/Equip"))
            dict_equip.Add(equip.item_id, equip);            
        // potions
        foreach(PotionBase potion in Resources.LoadAll<PotionBase>("Objects/Potion"))
            dict_potion.Add(potion.item_id, potion);
        // items
        foreach(ItemBase item in Resources.LoadAll<ItemBase>("Objects/Item"))
            dict_item.Add(item.item_id, item);

        // player invent initial
        
    }

    // Get
    /// <summary>
    /// Add new Equip into inventory
    /// </summary>
    /// <param name="id"> equip base id</param>
    /// <param name="level">equip level</param>
    /// <param name="tier">equip tier</param>
    public void GetEquip( string id, int level, int tier)
    {
        // loop to find slot for inserting
        if(invent_equip.Count >= equip_max)
            return;

        InsertIntoInvent<Equip>( invent_equip, EquipController.Controller().GenerateEquip(id, level, tier) );
    }
    /// <summary>
    /// Add potion into player inventory
    /// </summary>
    /// <param name="id">id of the potion</param>
    /// <param name="num">num to add</param>
    public void GetPotion( string id, int num)
    {
        foreach( Potion potion in invent_potion)
        {
            if(potion.item_id == id)
            {
                potion.item_num += num;
                return;
            }
        }
        InsertIntoInvent<Potion>( invent_potion, new Potion( id, num ));
    }
    /// <summary>
    /// Add items into player inventory
    /// </summary>
    /// <param name="id">id of the item</param>
    /// <param name="num">num to add</param>
    public void GetItem( string id, int num)
    {
        foreach( Item item in invent_item)
        {
            if(item.item_id == id)
            {
                item.item_num += num;
                return;
            }
        }
        InsertIntoInvent<Item>( invent_item, new Item( id, num ));
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

    // Remove
    /// <summary>
    /// Remove equip from target index
    /// </summary>
    /// <param name="index"> target index</param>
    public void RemoveEquip(int index)
    {
        if( invent_equip.Count > index && index >= 0)
            invent_equip.RemoveAt(index);
    }
    /// <summary>
    /// Remove potions from player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to remove</param>
    public void RemovePotion(string id, int num)
    {
        Potion potion = null;
        
        foreach( Potion i in invent_potion)
        {
            if( i.item_id == id )
            {
                potion = i;
                break;
            }
        }
        if(potion.item_num >= num)
            potion.item_num -= num;
        // Debug.Log(potion);
        if(potion.item_num <= 0)
            invent_potion.Remove(potion); 
    }
    /// <summary>
    /// Remove items from player inventory
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num"> num to remove</param>
    public void RemoveItem(string id, int num)
    {
        Item item = null;
        foreach( Item i in invent_item)
        {
            if( i.item_id == id )
            {
                item = i;
                break;
            } 
        }
        if(item.item_num >= num)
            item.item_num -= num;
        // Debug.Log(item);
        if(item.item_num <= 0)
            invent_item.Remove(item);
    }

    // Item Info
    /// <summary>
    /// Get the information about target equipment
    /// </summary>
    /// <param name="id">id of equipment</param>
    /// <returns>target equipment</returns>
    public Equip InventEquipInfo( int index )
    {
        if(index < invent_equip.Count)
            return invent_equip[index];
        else
            return null;
    }
    /// <summary>
    /// Get the information about target potion
    /// </summary>
    /// <param name="id">id of potion</param>
    /// <returns>target potion</returns>
    public Potion InventPotionInfo(string id)
    {
        foreach( Potion potion in invent_potion)
            if(potion.item_id == id)
                return potion;
        return null;
    }
    /// <summary>
    /// Get the information about target item
    /// </summary>
    /// <param name="id">id of item</param>
    /// <returns>target item</returns>
    public Item InventItemInfo(string id)
    {
        foreach( Item item in invent_item)
            if(item.item_id == id)
                return item;
        return null;
    }


    // Item Base Info
    /// <summary>
    /// Get Item base information from dictionary by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EquipBase DictEquipInfo(string id)
    {
        if(dict_equip.ContainsKey(id))
            return dict_equip[id];
        return null;
    }
    public PotionBase DictPotionInfo(string id)
    {
        if(dict_potion.ContainsKey(id))
            return dict_potion[id];
        return null;
    }
    public ItemBase DictItemInfo(string id)
    {
        if(dict_item.ContainsKey(id))
            return dict_item[id];
        return null;
    }    

    /// <summary>
    /// Return target invent page to the inventory panel
    /// </summary>
    /// <param name="invent"> GUI Invent panel list</param>
    /// <param name="invent_name"> target invent to display</param>
    /// <param name="page"> target page to display</param>
    /// <param name="token"> token for searching if need </param>
    /// <returns> total pages of this invent</returns>
    public int SetDisplayInvent(List<Item> display, string invent_name, int page, string token = "")
    {
        // target invent page to display
        display.Clear();
        // 

        switch(invent_name)
        {
            case "Equip":
                if( token == "")
                    return ISetDisplayInvent<Equip>(display, invent_equip, page); 
                else
                {
                    // add item into search list
                    foreach(Equip equip in invent_equip)
                    if( dict_equip[equip.item_id].item_name.Contains(token))
                        invent_search.Add(equip);
                    return ISetDisplayInvent<Item>(display, invent_search, page);
                }
            case "Potion":
                if( token == "")
                    return ISetDisplayInvent<Potion>(display, invent_potion, page); 
                else
                {
                    // add item into search list
                    foreach(Potion potion in invent_potion)
                    if( dict_potion[potion.item_id].item_name.Contains(token))
                        invent_search.Add(potion);
                    return ISetDisplayInvent<Item>(display, invent_search, page);
                }
            case "Item":
                if( token == "")
                    return ISetDisplayInvent<Item>(display, invent_item, page); 
                else
                {
                    // add item into search list
                    foreach(Item item in invent_item)
                    if( dict_item[item.item_id].item_name.Contains(token))
                        invent_search.Add(item);
                    return ISetDisplayInvent<Item>(display, invent_search, page);
                }
            default:
                return 0;
        }
    }
    // the implement of GetInvent function
    private int ISetDisplayInvent<T>( List<Item> display, List<T> invent, int page) where T : Item
    {
        for(int i = page*30; i < (page+1)*30 && i < invent.Count; i ++)
        { 
            display.Add(invent[i]);
        }
        return (int)(invent.Count/30);
    }




    /// Equip Behavior Control part
    public void InitialEnchant()
    {

    }

    /// <summary>
    /// create a new target equipment with random tag
    /// </summary>
    /// <param name="id">target equipment base</param>
    /// <returns>target equipment</returns>
    public Equip GenerateEquip(string id, int level, int tier)
    {
        Equip equip = new Equip(id, level, tier);
        for(int i = 0; i < equip.equip_enchants.Length; i ++)
        {
            equip.equip_enchants[i] = GetRandomEnchant(equip.equip_type).ToString();
        }
        return equip;
    }

    // return a random enchant of given equip type
    public string GetRandomEnchant( EquipType equip_type)
    {
        int enchant_index = UnityEngine.Random.Range(0, dict_enchant.Count-1);
        return dict_enchant[equip_type.ToString()][enchant_index];
    }

    // get enchant info
    public EquipEnchant EnchantInfo(string id)
    {
        return ResourceController.Controller().Load<EquipEnchant>("Objects/Equip/Enchant/"+id);
    }

    // equip price = basic price * equip level * equip tier^2
    public int GetEquipPrice(Equip equip)
    {
        return ItemController.Controller().DictEquipInfo(equip.item_id).item_price * equip.equip_level * equip.item_tier * equip.item_tier;
    }

    // equip attribute = ( basic attack + attack grow * equip level * 10 ) * (1 + t * 0.04)
    public int GetEquipAttribute(Equip equip, string value)
    {
        EquipBase equip_base = ItemController.Controller().DictEquipInfo(equip.item_id);
        switch(value)
        {
            case "attack":
                return (int)((equip_base.equip_attack + equip_base.equip_attack_grow * equip.equip_level * 10) * (1 + equip.item_tier*0.04));
            case "defense":
                return (int)((equip_base.equip_defense + equip_base.equip_defense_grow * equip.equip_level * 10) * (1 + equip.item_tier*0.04));
            case "health":
                return (int)((equip_base.equip_health + equip_base.equip_health_grow * equip.equip_level * 10) * (1 + equip.item_tier*0.04));
            default:
                return 0;
        }
    }





    /// Potion Behavior Control part
    
    public void UsePotion( string id )
    {
        PotionBase potion = ItemController.Controller().DictPotionInfo(id);

        // heal potion
        if(potion.potion_effect == PotionEffect.Heal)
        {
            int health_max = BattleController.Controller().player_unit.health_max;
            int health_curr = BattleController.Controller().player_unit.health_curr;
            BattleController.Controller().player_unit.health_curr = (health_curr + potion.potion_value > health_max) ? health_max : health_curr+potion.potion_value;
        }
    }
    





    /// Shop Control part
    



      
      
    // Craft Control part
    public void InitialPotionRecipe()
    {
        dict_potion_recipe.Add();
    }
    

    /// <summary>
    /// Check how many times this recipe can be made
    /// </summary>
    /// <param name="id">the id of recipe</param>
    /// <returns>maximum time could product</returns>
    public int RecipeProductLimit(string id)
    {
        PotionRecipe recipe = RecipeInfo(id);
        if(recipe == null)
            return 0;

        int maximum = -1;
        int product_time = 0;
        Item consume_item;

        for( int i = 0; i < recipe.recipe_consume.Length; i ++)
        {
            consume_item = InventItemInfo(recipe.recipe_consume[i]);
            if(consume_item != null)
                product_time = consume_item.item_num / recipe.recipe_consume_num[i];
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
        PotionRecipe recipe = RecipeInfo(id);
        if(recipe == null)
            return;
        
        // get potion
        GetPotion(recipe.recipe_result, recipe.recipe_result_num * num);

        // remove consume item
        for(int i = 0; i < recipe.recipe_consume.Length; i ++)
            RemoveItem(recipe.recipe_consume[i], recipe.recipe_consume_num[i] * num);
    }

    // return true if theres enough item for level strengthen
    public bool LevelCostCheck(int cost)
    {
        return ( InventItemInfo(equip_strengthen_item) == null ) ? false : InventItemInfo(equip_strengthen_item).item_num >= cost;
    }

    
    // return true if theres enough item for enchant strengthen
    public bool EnchantCostCheck(int cost)
    {
        return ( InventItemInfo(equip_enchant_item) == null ) ? false : InventItemInfo(equip_enchant_item).item_num >= cost;
    }

    // get info of target recipe
    public PotionRecipe RecipeInfo(string id)
    {
        return ResourceController.Controller().Load<PotionRecipe>("Objects/Potion/Recipe/"+id);
    }

    // return all recipes' name
    public List<string> GetRecipeDict()
    {
        return dict_potion_recipe;
    }

    

    


    // equip crafting

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
            equip.equip_enchants[index[i]] = GetRandomEnchant(equip.equip_type);
        }
        RemoveItem(equip_enchant_item, enchant_item_cost * index.Count);
    }


    // potion crafting
}

