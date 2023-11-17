using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : PanelBase
{
    public List<Item> buy_list;
    public List<Item> sell_list;

    public string type;

    public override void ShowSelf()
    {
        // set shop items
        ResetBuyGrid();
        ResetSellGrid();
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1, (p) =>
        {
            p.panel = type + "ShopPanel";
        });
        ShopController.Controller().shop = this;
    }

    protected override void OnButtonClick(string button_name)
    {
        int index;

        if(button_name == "CloseBtn")
        {
            GUIController.Controller().RemovePanel("InventPanel");
            GUIController.Controller().RemovePanel(type+"ShopPanel");
        }
        else if(button_name.Contains("BuySlot"))
        {
            index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));

            if(type == "Equip")
                ShopController.Controller().EquipShop(index, "Buy");
            else if(type == "Potion")
                ShopController.Controller().PotionShop(index, "Buy");
            else
                ShopController.Controller().ItemShop(index, "Buy");
        }
        else if(button_name.Contains("SellSlot"))
        {
            index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));

            if(type == "Equip")
                ShopController.Controller().EquipShop(index, "Rebuy");
            else if(type == "Potion") 
                ShopController.Controller().PotionShop(index, "Rebuy");
            else
                ShopController.Controller().ItemShop(index, "Rebuy");
        }
    }

    public void ResetBuyGrid()
    {
        GetShopList();

        for(int i = 0; i < 18; i ++)
        {
            Transform btn = FindComponent<Button>("BuySlot ("+i+")").transform;
            if(i < buy_list.Count)
            {
                btn.GetChild(0).gameObject.SetActive(true);
                btn.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+buy_list[i].item_id);
                if(type == "Equip")
                {
                    btn.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    btn.GetChild(1).gameObject.SetActive(true);
                    btn.GetChild(1).GetComponent<Text>().text = buy_list[i].item_num.ToString();
                }
            }
            else
            {
                btn.GetChild(0).gameObject.SetActive(false);
                btn.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void ResetSellGrid()
    {
        GetShopList();

        for(int i = 0; i < 18; i ++)
        {
            Transform btn = FindComponent<Button>("SellSlot ("+i+")").transform;
            if(i < sell_list.Count)
            {
                btn.GetChild(0).gameObject.SetActive(true);
                btn.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+sell_list[i].item_id);
                if(type != "Equip")
                {
                    btn.GetChild(1).gameObject.SetActive(true);
                    btn.GetChild(1).GetComponent<Text>().text = sell_list[i].item_num.ToString();
                }
            }
            else
            {
                btn.GetChild(0).gameObject.SetActive(false);
                btn.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void GetShopList()
    {
        if(type == "Equip")
        {
            buy_list = ShopController.Controller().shop_equip.OfType<Item>().ToList();
            sell_list = ShopController.Controller().sell_equip.OfType<Item>().ToList();
        }
        else if( type == "Potion")
        {
            buy_list = ShopController.Controller().shop_potion.OfType<Item>().ToList();
            sell_list = ShopController.Controller().sell_potion.OfType<Item>().ToList();
        }
        else
        {
            buy_list = ShopController.Controller().shop_item;
            sell_list = ShopController.Controller().sell_item;
        }
    }
}
