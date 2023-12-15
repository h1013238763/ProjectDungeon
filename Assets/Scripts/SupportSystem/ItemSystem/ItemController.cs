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
    // Invent attributes
    public int equip_max = 150;     // the maximum capacity of equip inventory

    public InventData data;

    // items dictionary
    private Dictionary<string, EquipBase> dict_equip = new  Dictionary<string, EquipBase>();
    private Dictionary<string, PotionBase> dict_potion = new Dictionary<string, PotionBase>();
    private Dictionary<string, ItemBase> dict_item = new  Dictionary<string, ItemBase>();
    public Dictionary<string, PotionRecipe> dict_recipe = new  Dictionary<string, PotionRecipe>();
    private Dictionary<string, Sprite> image_item = new Dictionary<string, Sprite>();

    // Equip Tier Rate
    public int[,] equip_tier_rate;

    // Equip Craft attributes
    public int equip_level_cap; // the level cap of equipment for strengthen
    public string equip_strengthen_item = "StrengthenShard";   // the string id of item for equip strengthen
    public string equip_enchant_item = "EnchantmentShard";   // the string id of item for equip enchantment

    public void SaveData()
    {
        XmlController.Controller().SaveData(data, "InventData");
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(InventData), "InventData") as InventData;
        if(data == null)
            NewData();
    }
    public void NewData()
    {
        data = new InventData();

        // freshman gift
        PlayerBuild build = PlayerController.Controller().data.player_build[PlayerController.Controller().data.player_build_index];
        
        GetItem("EnchantmentShard", 10);
        GetItem("StrengthenShard", 5);
        // freshman equip
        Equip equip = new Equip("RustySword", 1, 1);
        GetEquip(equip);
        build.equips[0] = equip;
        // freshman potion 
        GetPotion("HealingPotion", 10);
        build.potions.Add("HealingPotion");
    }

    public void InitialData()
    {
        equip_tier_rate = new int[5,5]{ 
                            {100, 0, 0, 0, 0},
                            {60, 100, 0, 0, 0},
                            {20, 70, 100, 0, 0},
                            {10, 50, 80, 100, 0},
                            {0, 25, 60, 85, 100}};

        EquipBase[] equips = Resources.LoadAll<EquipBase>("Object/Equip/");
        if(equips != null)
        {
            foreach(EquipBase equip in equips)
            {
                if(!dict_equip.ContainsKey(equip.item_id))
                    dict_equip.Add(equip.item_id, equip);
            }
        }
        PotionBase[] potions = Resources.LoadAll<PotionBase>("Object/Potion/");
        if(potions != null)
        {
            foreach(PotionBase potion in potions)
            {
                dict_potion.Add(potion.item_id, potion);
            }
        }
        ItemBase[] items = Resources.LoadAll<ItemBase>("Object/Item/");
        if(items != null)
        {
            foreach(ItemBase item in items)
            {
                dict_item.Add(item.item_id, item);
            }
        }

        // Recipe
        PotionRecipe[] recipes = Resources.LoadAll<PotionRecipe>("Object/Recipe/");
        if(recipes != null)
        {
            foreach(PotionRecipe recipe in recipes)
            {
                dict_recipe.Add(recipe.recipe_id, recipe);
            }    
        }

        Sprite[] images = Resources.LoadAll<Sprite>("Image/Item/");
        if(images != null)
        {
            foreach(Sprite image in images)
            {
                image_item.Add(image.name, image);
            }
        }
    }

    /// Inventory Control part
    
    // Get
    public bool GetEquip( Equip equip)
    {
        if(data.invent_equip.Count >= 180)
            return false;
        InsertIntoInvent<Equip>( data.invent_equip, equip );
            return true;
    }
    /// <summary>
    /// Add potion into player inventory
    /// </summary>
    /// <param name="id">id of the potion</param>
    /// <param name="num">num to add</param>
    public bool GetPotion( string id, int num)
    {
        foreach( Potion potion in data.invent_potion)
        {
            if(potion.item_id == id)
            {
                potion.item_num += num;
                return true;
            }
        }
        InsertIntoInvent<Potion>( data.invent_potion, new Potion( id, num ));
        return true;
    }
    /// <summary>
    /// Add items into player inventory
    /// </summary>
    /// <param name="id">id of the item</param>
    /// <param name="num">num to add</param>
    public bool GetItem( string id, int num)
    {
        foreach( Item item in data.invent_item)
        {
            if(item.item_id == id)
            {
                item.item_num += num;
                return true;
            }
        }
        InsertIntoInvent<Item>( data.invent_item, new Item( id, num ));
        return true;
    }
    /// <summary>
    /// Insert item into inventory
    /// </summary>
    /// <param name="invent"> target invent</param>
    /// <param name="item"> item to add</param>
    /// <typeparam name="T"> type of item </typeparam>
    private void InsertIntoInvent<T>(List<T> invent, T item) where T : Item
    {
        if(typeof(Equip).Name == typeof(T).Name)
            ShopController.Controller().AddUnlockItem<EquipBase>(DictEquipInfo(item.item_id));
        else if(typeof(Potion).Name == typeof(T).Name)
            ShopController.Controller().AddUnlockItem<PotionBase>(DictPotionInfo(item.item_id));
        else if(typeof(Item).Name == typeof(T).Name)
            ShopController.Controller().AddUnlockItem<ItemBase>(DictItemInfo(item.item_id));

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
        if( data.invent_equip.Count > index && index >= 0)
            data.invent_equip.RemoveAt(index);
    }
    /// <summary>
    /// Remove potions from player inventory
    /// </summary>
    /// <param name="id">id of target potion</param>
    /// <param name="num">num to remove</param>
    public void RemovePotion(string id, int num)
    {
        Potion potion = null;
        
        foreach( Potion i in data.invent_potion)
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
            data.invent_potion.Remove(potion); 
    }
    /// <summary>
    /// Remove items from player inventory
    /// </summary>
    /// <param name="id">id of target item</param>
    /// <param name="num"> num to remove</param>
    public void RemoveItem(string id, int num)
    {
        Item item = null;
        foreach( Item i in data.invent_item)
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
            data.invent_item.Remove(item);
    }

    // Item Info
    /// <summary>
    /// Get the information about target equipment
    /// </summary>
    /// <param name="id">id of equipment</param>
    /// <returns>target equipment</returns>
    public Equip InventEquipInfo( int index )
    {
        if(index < data.invent_equip.Count)
            return data.invent_equip[index];
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
        foreach( Potion potion in data.invent_potion)
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
        foreach( Item item in data.invent_item)
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
        if(!dict_equip.ContainsKey(id))
            return null;
        return dict_equip[id];
    }
    public PotionBase DictPotionInfo(string id)
    {
        if(!dict_potion.ContainsKey(id))
            return null;
        return dict_potion[id];
    }
    public ItemBase DictItemInfo(string id)
    {
        if(!dict_item.ContainsKey(id))
            return null;
        return dict_item[id];
    }    
    public Sprite GetImage(string id)
    {
        if(!image_item.ContainsKey(id))
            return null;
        return image_item[id];
    }
    

    // check the item type (equip, potion, item)
    public string CheckItemType(string id)
    {
        if(DictEquipInfo(id) != null)
            return "Equip";
        else if(DictPotionInfo(id) != null)
            return "Potion";
        else if(DictItemInfo(id) != null)
            return "Item";
        else
            return "";
    }

    /// Inventory Panel Support Functions

    /// <summary>
    /// Return target invent page to the inventory panel
    /// </summary>
    /// <param name="invent"> GUI Invent panel list</param>
    /// <param name="invent_name"> target invent to display</param>
    /// <param name="page"> target page to display</param>
    /// <param name="token"> token for searching if need </param>
    /// <returns> total pages of this invent</returns>
    public int SetDisplayInvent(List<Item> display, string invent_name, int page)
    {
        // target invent page to display
        display.Clear();
        // 

        switch(invent_name)
        {
            case "Equip":
                return ISetDisplayInvent<Equip>(display, data.invent_equip, page); 
            case "Potion":
                return ISetDisplayInvent<Potion>(display, data.invent_potion, page); 
            case "Item":
                return ISetDisplayInvent<Item>(display, data.invent_item, page); 
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




    /// <summary>
    /// create a new target equipment with random tag
    /// </summary>
    /// <param name="id">target equipment base</param>
    /// <returns>target equipment</returns>
    public Equip RandomEquip(string id, int level_cap, int tier_cap)
    {
        // random tier
        int tier = 0;
        int tier_random = UnityEngine.Random.Range(0, 100);
        for(int i = 0; i < 5; i ++)
        {
            if(equip_tier_rate[tier_cap,i] < tier_random)
            {
                tier = i+1;
                break;
            }      
        }
        int level = level_cap - UnityEngine.Random.Range(0, 3);
        if(level <= 0)
            level = 1;

        Equip equip = new Equip(id, level, tier);

        for(int i = 0; i < equip.enchant_limit; i ++)
        {
            equip.equip_enchants.Add(EnchantController.Controller().GetRandomEnchant(equip.equip_type));
        }
        return equip;
    }

    /// Equip Crafting
    // return true if theres enough item for level strengthen
    public bool LevelCostCheck(Equip equip)
    {
        int cost = GetStrengthCost(equip);
        if(InventItemInfo(equip_strengthen_item) == null)
            return false;
        if(equip.equip_level >= PlayerController.Controller().data.player_level)
            return false;
        
        return InventItemInfo(equip_strengthen_item).item_num >= cost;
    }    
    // return true if theres enough item for enchant strengthen
    public bool EnchantCostCheck(Equip equip, int enchant_num)
    {
        int cost = GetEnchantCost(equip, enchant_num);
        return ( InventItemInfo(equip_enchant_item) == null ) ? false : InventItemInfo(equip_enchant_item).item_num >= cost;
    }

    // Get Craft Cost
    public int GetStrengthCost(Equip equip)
    {
        //  equip level / 5
        return Mathf.CeilToInt(equip.equip_level / 5f);
    }
    public int GetEnchantCost(Equip equip, int enchant_num)
    {
        //  enchant num * tier / 2
        return Mathf.CeilToInt(equip.item_tier / 2f * enchant_num);
    }

    // equip crafting
    public void StrengthenEquip(Equip equip)
    {
        Item item = ItemController.Controller().InventItemInfo(equip_strengthen_item);
        
        ItemController.Controller().RemoveItem(equip_strengthen_item, GetStrengthCost(equip));
        equip.equip_level += 1;
    }
    public void ResetEnchantment( Equip equip, List<int> index )
    {
        for(int i = 0; i < index.Count; i ++)
        {
            equip.equip_enchants[index[i]] = EnchantController.Controller().GetRandomEnchant(equip.equip_type);
        }
        RemoveItem(equip_enchant_item, GetEnchantCost(equip, index.Count));
    }

    
    /// Potion Behavior Control part
    public void UsePotion( string id )
    {
        // player audio
        AudioController.Controller().StartSound("UsePotion");

        PotionBase potion = ItemController.Controller().DictPotionInfo(id);
        BattleUnit player_unit = BattleController.Controller().player_unit;

        switch(potion.potion_effect)
        {
            case PotionEffect.Heal:
                int heal_value =  (int)(0.01f * BattleController.Controller().player_unit.health_max * potion.potion_value);
                player_unit.OnHeal(heal_value);
                break;
            case PotionEffect.Cure:
                player_unit.ClearEffect("debuff");
                break;
            case PotionEffect.Attack:
                BuffController.Controller().AddBuff(BuffAttribute.Attack, (int)(player_unit.GetAttack()*0.5f), 1, player_unit);
                break;
            case PotionEffect.Defense:
                BuffController.Controller().AddBuff(BuffAttribute.Defense, (int)(player_unit.GetDefense()*0.5f), 1, player_unit);
                break;
            default:
                break;
        }        
    }

    /// potion crafting
    // get info of target recipe
    public PotionRecipe RecipeInfo(string id)
    {
        if(!dict_recipe.ContainsKey(id))
            return null;
        return dict_recipe[id];
    }

    // Check how many times this recipe can be made
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

    // craft potions
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
}

public class InventData
    {
        public List<Equip> invent_equip;    // the inventory for equipments
        public List<Potion> invent_potion; // the inventory for potions
        public List<Item> invent_item;   // the inventory for items

        public InventData()
        {
            invent_equip = new List<Equip>();
            invent_potion = new List<Potion>();
            invent_item = new List<Item>();
        }
    }