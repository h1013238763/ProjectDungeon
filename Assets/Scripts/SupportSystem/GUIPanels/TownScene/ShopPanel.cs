using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopPanel : PanelBase
{
    public List<Item> buy_list;
    public List<Item> sell_list;

    public Quest equip_quest;
    public Quest potion_quest;
    public Quest item_quest;

    public string type;

    public override void ShowSelf()
    {
        // set shop items
        ResetPanel();
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1, (p) =>
        {
            p.panel = type + "ShopPanel";
        });
        ShopController.Controller().shop = this;

        Button btn;
        for(int i = 0; i < 18; i ++)
        {
            btn = FindComponent<Button>("BuySlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
            btn = FindComponent<Button>("SellSlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
    }

    protected override void OnButtonClick(string button_name)
    {
        int index;

        if(button_name == "CloseBtn")
        {
            AudioController.Controller().StartSound("ShopRing");

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
        else if(button_name == "QuestTip")
        {
            AudioController.Controller().StartSound("AcceptQuest");

            if(type == "Equip")
            {
                EventController.Controller().EventTrigger<Quest>("AcceptQuest", equip_quest);
                equip_quest = null;
            }
            else if(type == "Potion")
            {
                EventController.Controller().EventTrigger<Quest>("AcceptQuest", potion_quest);
                potion_quest = null;
            }
            else if(type == "Item")
            {
                EventController.Controller().EventTrigger<Quest>("AcceptQuest", item_quest);
                item_quest = null;
            }  
            ResetPanel();
        }
    }

    public void ResetPanel()
    {
        ResetBuyGrid();
        ResetSellGrid();
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
                btn.GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(buy_list[i].item_id);
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
                btn.GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(sell_list[i].item_id);
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

    // mouse hover to check item information
    /// <summary>
    /// show info panel on pointer enter
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerEnter(PointerEventData event_data)
    {
        string name = event_data.pointerEnter.name;
        // get index
        int index = Int32.Parse(name.Substring(name.IndexOf("(")+1, name.IndexOf(")")-name.IndexOf("(")-1));

        if(name.Contains("Buy"))
        {
            if(index < 0 || index >= buy_list.Count)
                return;
            if(buy_list[index] == null)
                return;

            GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, (p) =>
            {
                p.info_type = buy_list[index].GetType().Name;
                p.info_item = buy_list[index];
                p.mouse_pos = event_data.position;
            });
        }
        else if(name.Contains("Sell"))
        {
            if(index < 0 || index >= sell_list.Count)
                return;
            if(sell_list[index] == null)
                return;

            GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, (p) =>
            {
                p.info_type = sell_list[index].GetType().Name;
                p.info_item = sell_list[index];
                p.mouse_pos = event_data.position;
            });
        }

        
    }
    /// <summary>
    /// hide info panel on pointer exit
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerExit(PointerEventData event_data)
    {
        GUIController.Controller().RemovePanel("InfoPanel");
    }
}
