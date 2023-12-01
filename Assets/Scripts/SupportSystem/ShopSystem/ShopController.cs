using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control all shop behaviors including buy, sell, generate items
/// </summary>
public class ShopController : BaseController<ShopController>
{
    public ShopData data;
    public PlayerData player;
    
    public List<Equip> shop_equip = new List<Equip>();     // equipment shop
    public List<Potion> shop_potion = new List<Potion>();    // potion shop
    public List<Item> shop_item = new List<Item>();      // item shop

    public List<Equip> sell_equip = new List<Equip>();         // sold items from player
    public List<Potion> sell_potion = new List<Potion>();
    public List<Item> sell_item = new List<Item>();

    public InventPanel invent;
    public ShopPanel shop;

    public float SELL_COE = 0.75f;

    public void RefreshShop()
    {
        // generate new shop list
        shop_equip.Clear();
        shop_potion.Clear();
        shop_item.Clear();
        // equip shop
        for(int i = 0; i < data.equip_slot_cap; i ++)
        {
            // set a random unlock equip into this slot
            int index = UnityEngine.Random.Range(0, data.unlock_equip.Count-1);
            Equip equip = ItemController.Controller().RandomEquip(data.unlock_equip[index], player.player_level, data.equip_tier_cap);
            shop_equip.Add(equip);
        }
        // potion shop
        for(int i = 0; i < data.potion_slot_cap; i ++)
        {
            // set a random unlock potion into this slot
            int index = UnityEngine.Random.Range(0, data.unlock_potion.Count-1);
            shop_potion.Add(new Potion(data.unlock_potion[index], data.potion_num_cap));
        }
        // item shop
        for(int i = 0; i < data.item_slot_cap; i ++)
        {
            // set a random unlock item into this slot
            int index = UnityEngine.Random.Range(0, data.unlock_item.Count-1);
            shop_item.Add( new Item(data.unlock_item[index], data.item_num_cap));
        }

        // clear sold items
        sell_equip.Clear();
        sell_potion.Clear();
        sell_item.Clear();
    }

    public bool EquipShop(int index, string act)
    {
        if(act == "Buy")
        {
            if(index < 0 || index >= shop_equip.Count)
                return false;
            // don't have enough money
            if(player.player_money < shop_equip[index].GetPrice())
                return false;

            AudioController.Controller().StartSound("CoinDrop");
            // invent full
            if(ItemController.Controller().GetEquip(shop_equip[index]))
            {
                player.player_money -= shop_equip[index].GetPrice();
                shop_equip.RemoveAt(index);

                shop.ResetBuyGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Rebuy")
        {
            if(index < 0 || index >= sell_equip.Count)
                return false;

            if(player.player_money < (int)(SELL_COE*sell_equip[index].GetPrice()))
                return false;

            AudioController.Controller().StartSound("CoinDrop");
            // invent full
            if(ItemController.Controller().GetEquip(sell_equip[index]))
            {
                player.player_money -= (int)(SELL_COE*sell_equip[index].GetPrice());

                sell_equip.RemoveAt(index);

                shop.ResetSellGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Sell")
        {
            Equip equip = ItemController.Controller().InventEquipInfo(index);

            if(equip == null)
                return false;

            sell_equip.Add(equip);
            if(sell_equip.Count > 18)
                sell_equip.RemoveAt(0);
            ItemController.Controller().RemoveEquip(index);

            player.player_money += (int)(SELL_COE*equip.GetPrice());

            shop.ResetSellGrid();
            invent.ResetInventPanel();
            invent.ResetMoney();

            return true;
        }
        else
            return false;
    }

    public bool PotionShop(int index, string act)
    { 
        if(act == "Buy")
        {   
            if(index < 0 || index >= shop_potion.Count)
                return false;
            // don't have enough money
            if(player.player_money < shop_potion[index].GetPrice())
                return false;

            AudioController.Controller().StartSound("CoinDrop");
            // invent full
            if(ItemController.Controller().GetPotion(shop_potion[index].item_id, 1))
            {
                player.player_money -= shop_potion[index].GetPrice();
                shop_potion[index].item_num --;
                if(shop_potion[index].item_num <= 0)
                {
                    shop_potion.RemoveAt(index);
                }

                shop.ResetBuyGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Rebuy")
        {
            if(index < 0 || index >= sell_potion.Count)
                return false;
            if(player.player_money < (int)(SELL_COE*sell_potion[index].GetPrice()))
                return false;
            
            AudioController.Controller().StartSound("CoinDrop");
            // invent full
            if(ItemController.Controller().GetPotion(sell_potion[index].item_id, 1))
            {
                player.player_money -= (int)(SELL_COE*sell_potion[index].GetPrice());
                sell_potion[index].item_num --;
                if(sell_potion[index].item_num <= 0)
                {
                    sell_potion.RemoveAt(index);
                }

                shop.ResetSellGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Sell")
        {
            Potion potion = invent.PanelItemInfo(index) as Potion;

            if(potion == null)
                return false;

            for(int i = 0; i < 18; i ++)
            {
                if( i < sell_potion.Count)
                {
                    if(sell_potion[i].item_id == potion.item_id)
                    {
                        sell_potion[i].item_num ++;
                        break;
                    }
                }
                else{
                    sell_potion.Add(new Potion(potion.item_id, 1));
                    break;
                }
            }

            if(sell_potion.Count > 18)
                sell_potion.RemoveAt(0);
            ItemController.Controller().RemovePotion(potion.item_id, 1);

            player.player_money += (int)(SELL_COE*potion.GetPrice());

            shop.ResetSellGrid();
            invent.ResetInventPanel();
            invent.ResetMoney();

            return true;
        }
        else
            return false;
        
    }

    public bool ItemShop(int index, string act)
    {
        if(act == "Buy")
        {
            if(index < 0 || index >= shop_item.Count)
                return false;
            // don't have enough money
            if(player.player_money < shop_item[index].GetPrice())
                return false;

            AudioController.Controller().StartSound("CoinDrop");
            // invent full
            if(ItemController.Controller().GetItem(shop_item[index].item_id, 1))
            {
                player.player_money -= shop_item[index].GetPrice();
                shop_item[index].item_num --;
                if(shop_item[index].item_num <= 0)
                {
                    shop_item.RemoveAt(index);
                }

                shop.ResetBuyGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Rebuy")
        {
            if(index < 0 || index >= sell_item.Count)
                return false;
            if(player.player_money < (int)(SELL_COE*sell_item[index].GetPrice()))
                return false;

            AudioController.Controller().StartSound("CoinDrop");
                // invent full
            if(ItemController.Controller().GetItem(sell_item[index].item_id, 1))
            {
                player.player_money -= (int)(SELL_COE*sell_item[index].GetPrice());

                sell_item[index].item_num --;
                if(sell_item[index].item_num <= 0)
                {
                    sell_item.RemoveAt(index);
                }

                shop.ResetSellGrid();
                invent.ResetInventPanel();
                invent.ResetMoney();

                return true;
            }
            return false;
        }
        else if( act == "Sell")
        {
            Item item = invent.PanelItemInfo(index);

            if(item == null)
                return false;

            for(int i = 0; i < 18; i ++)
            {
                if( i < sell_item.Count)
                {
                    if(sell_item[i].item_id == item.item_id)
                    {
                        sell_item[i].item_num ++;
                        break;
                    }
                }
                else{
                    sell_item.Add(new Item(item.item_id, 1));
                    break;
                }
            }

            if(sell_equip.Count > 18)
                sell_equip.RemoveAt(0);
            ItemController.Controller().RemoveItem(item.item_id, 1);
            player.player_money += (int)(SELL_COE*item.GetPrice());

            shop.ResetSellGrid();
            invent.ResetInventPanel();
            invent.ResetMoney();

            return true;
        }
        else      
            return false;
    }

    // add item into unlock list once player get new item
    public void AddUnlockItem<T>(T item) where T : ItemBase
    {
        if(typeof(T).Name == typeof(EquipBase).Name )
        {
            if(!data.unlock_equip.Contains(item.item_id))
                data.unlock_equip.Add(item.item_id);
            return;
        }
        else if(typeof(T).Name == typeof(PotionBase).Name )
        {
            if(!data.unlock_potion.Contains(item.item_id))
                data.unlock_potion.Add(item.item_id);
            return;
        }
        else if(typeof(T).Name == typeof(ItemBase).Name )
        {
            if(!data.unlock_item.Contains(item.item_id))
                data.unlock_item.Add(item.item_id);
            return; 
        }
    }

    public void InitialData()
    {
        
    }
    public void SaveData()
    {
        XmlController.Controller().SaveData(data, "ShopData");
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(ShopData), "ShopData") as ShopData;
        if(data == null)
            data = new ShopData();
        player = PlayerController.Controller().data;
    }
    public void NewData()
    {
        data = new ShopData();
        player = PlayerController.Controller().data;
    }

}

public class ShopData
{
    // equip related shop caps
    public List<string> unlock_equip;
    public int equip_tier_cap;
    public int equip_slot_cap;

    // potion related shop caps
    public List<string> unlock_potion;
    public int potion_slot_cap;
    public int potion_num_cap;

    // item related shop caps
    public List<string> unlock_item;
    public int item_slot_cap;
    public int item_num_cap;

    public ShopData()
    {
        unlock_equip = new List<string>();
        equip_tier_cap = 0;
        equip_slot_cap = 3;

        unlock_potion = new List<string>();
        potion_slot_cap = 3;
        potion_num_cap = 5;

        unlock_item = new List<string>();
        item_slot_cap = 3;
        item_num_cap = 5;
    }
}