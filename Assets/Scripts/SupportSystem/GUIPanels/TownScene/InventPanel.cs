using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// control the player inventory panel behavior
/// </summary>
public class InventPanel : PanelBase
{
    public string type;         // current invent item type
    public int page;            // current invent page
    public List<Item> display_invent = new List<Item>();  // current display list
    public string panel;

    public override void ShowSelf()
    {
        // reset type and page
        page = 0;

        // reset money, page buttons and slots
        ResetMoney();

        SetCurrPanel();

        ResetInventPanel();
        // add slots pointer hover events
        

        ShopController.Controller().invent = this;
    }

    /// <summary>
    /// Invent Panel button click events
    /// </summary>
    /// <param name="button_name"></param>
    protected override void OnButtonClick(string button_name)
    {
        // display type buttons
        // click type buttons to switch invent
        if(button_name.Contains("TypeBtn"))
        {
            type = button_name.Substring(0, button_name.IndexOf("TypeBtn"));
            page = 0;
            ResetInventPanel();
        }

        // change invent page:
        else if(button_name.Contains("PageBtn"))
        {
            page = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
        }
        // click slot button
        else if(button_name.Contains("InventSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));

            if(slot < 0 || slot >= display_invent.Count)
                return;

            if(panel == "EquipCraftPanel")
            {
                Equip equip = PanelItemInfo(slot) as Equip;
                if(equip == null)
                    return;
                GUIController.Controller().GetPanel<EquipCraftPanel>("EquipCraftPanel").SetEquip(equip);
            }
            else if(panel == "EquipShopPanel")
            {
                ShopController.Controller().EquipShop(slot, "Sell");
            }
            else if(panel == "PotionShopPanel")
            {
                ShopController.Controller().PotionShop(slot, "Sell");
            }
            else if(panel == "ItemShopPanel")
            {
                ShopController.Controller().ItemShop(slot, "Sell");
            }
            else if(panel == "PlayerPanel")
            {
                PlayerController.Controller().Equip(display_invent[slot], type);
            }
            ResetInventPanel();
        }
    }

    // display player money text
    public void ResetMoney()
    {
        FindComponent<Text>("MoneyText").text = PlayerController.Controller().data.player_money.ToString();
    }

    // display all player items in invent
    public void ResetInventPanel()
    {
        // add items into display list and get total page
        int page_totel = ItemController.Controller().SetDisplayInvent(display_invent, type, page);
        // set pages
        for(int i = 0; i < 5; i ++)
        {
            FindComponent<Button>("PageBtn (" + i + ")").gameObject.SetActive( i <= page_totel );
        }

        // reset item by display list
        Transform slot;
        
        for(int i = 0; i < 30; i ++)
        {
            slot = FindComponent<Button>("InventSlot (" + i + ")").transform;

            if(i < display_invent.Count)
            {   
                // search and set image by item id
                slot.GetChild(0).gameObject.SetActive(true);
                slot.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+display_invent[i].item_id);
                // set number text
                if(type != "Equip")
                {
                    slot.GetChild(1).gameObject.SetActive(true);
                    slot.GetChild(1).GetComponent<Text>().text = display_invent[i].item_num.ToString();
                    if(type == "Potion")
                    {
                        PlayerData player = PlayerController.Controller().data;

                        slot.GetComponent<Button>().interactable = !(panel == "PlayerPanel" && 
                            player.player_build[player.player_build_index].potions.Contains(display_invent[i] as Potion));
                    }
                    else
                    {
                        slot.GetComponent<Button>().interactable = true;
                    }
                }
                else
                {
                    slot.GetChild(1).gameObject.SetActive(false);

                    PlayerData player = PlayerController.Controller().data;

                    slot.GetComponent<Button>().interactable = !(panel == "PlayerPanel" && 
                        display_invent[i] as Equip == player.player_build[player.player_build_index].equips[(int)(display_invent[i] as Equip).equip_type]);
                    
                }
            }
            else
            {
                // inparent item sprite and num text
                slot.GetChild(0).gameObject.SetActive(false);
                slot.GetChild(1).gameObject.SetActive(false);
            }
           
        }
    }

    public void SetCurrPanel()
    {
        switch(panel)
        {
            case "EquipCraftPanel":
                FindComponent<Button>("EquipTypeBtn").interactable = true;
                FindComponent<Button>("PotionTypeBtn").interactable = false;
                FindComponent<Button>("ItemTypeBtn").interactable = false;
                type = "Equip";
                break;
            case "EquipShopPanel":
                FindComponent<Button>("EquipTypeBtn").interactable = true;
                FindComponent<Button>("PotionTypeBtn").interactable = false;
                FindComponent<Button>("ItemTypeBtn").interactable = false;
                type = "Equip";
                break;
            case "PotionShopPanel":
                FindComponent<Button>("EquipTypeBtn").interactable = false;
                FindComponent<Button>("PotionTypeBtn").interactable = true;
                FindComponent<Button>("ItemTypeBtn").interactable = false;
                type = "Potion";
                break;
            case "ItemShopPanel":
                FindComponent<Button>("EquipTypeBtn").interactable = false;
                FindComponent<Button>("PotionTypeBtn").interactable = false;
                FindComponent<Button>("ItemTypeBtn").interactable = true;
                type = "Item";
                break;
            case "PlayerPanel":
                FindComponent<Button>("EquipTypeBtn").interactable = true;
                FindComponent<Button>("PotionTypeBtn").interactable = true;
                FindComponent<Button>("ItemTypeBtn").interactable = true;
                type = "Equip";
                break;
            default:
                break;
        }
    }

    public Item PanelItemInfo(int index)
    {
        if(index < 0 || index >= display_invent.Count)
            return null;
        return display_invent[index];
    }


    // Add custom event listener on each slot, show item information
    public void SetCostumeEvent()
    {
        for(int i = 0; i < 30; i ++)
        {
            Button btn = FindComponent<Button>("InventSlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
    }
   
    // mouse hover to check item information
    /// <summary>
    /// show info panel on pointer enter
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerEnter(PointerEventData event_data)
    {
        // get index
        int index = GetPointerObjectIndex(event_data);

        if(index < 0 || index >= display_invent.Count)
            return;

        /*
        GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, (panel) =>
        {
            panel.DisplayInfo(display_invent[index].item_id, type);
        });
        */
    }
    /// <summary>
    /// hide info panel on pointer exit
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerExit(PointerEventData event_data)
    {
        // GUIController.Controller().HidePanel("InfoPanel");
    }
    // break PointerEvent into useful info token
    private int GetPointerObjectIndex(PointerEventData event_data)
    {
        string name = event_data.pointerEnter.name;
        return Int32.Parse(name.Substring(name.IndexOf("(")+1, name.IndexOf(")")-name.IndexOf("(")));
    }
}
