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
    private List<Item> display_invent = new List<Item>();  // current display list
    private string curr_panel = "";

    public override void ShowSelf()
    {
        // reset type and page
        type = "Equip";
        page = 0;
        // reset search input
        ResetSearchInput();
        // reset money, page buttons and slots
        ResetMoney();
        ResetInventPanel();
        // add slots pointer hover events
        AddCustomeEvent();
    }

    /// <summary>
    /// Invent Panel button click events
    /// </summary>
    /// <param name="button_name"></param>
    protected override void OnButtonClick(string button_name)
    {
        // click close button to close panel
        if(button_name == "CloseBtn")
        {
            GUIController.Controller().RemovePanel("InventPanel");
            if(curr_panel == "EquipCraftPanel")
                GUIController.Controller().RemovePanel("EquipCraftPanel");
        }

        // display type buttons
        // click type buttons to switch invent
        else if(button_name.Contains("TypeBtn"))
        {
            type = button_name.Substring(0, button_name.IndexOf("TypeBtn"));
            page = 0;
            ResetInventPanel();
        }

        // Player search for item
        else if(button_name == "SearchBtn")
        {
            Debug.Log("Input: " + FindComponent<Text>("SearchText").text);
            page = 0;
            if(FindComponent<Text>("SearchText").text != "")
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
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            if(curr_panel == "EquipCraftPanel")
            {
                GUIController.Controller().GetPanel<EquipCraftPanel>("EquipCraftPanel").SetEquip((Equip)display_invent[slot]);
            }
        }
    }

    /// <summary>
    /// display player money text
    /// </summary>
    private void ResetMoney()
    {
        FindComponent<Text>("MoneyText").text = ItemController.Controller().money.ToString();
    }

    /// <summary>
    /// display all player items in invent
    /// </summary>
    private void ResetInventPanel()
    {
        // add items into display list and get total page
        int page_totel = ItemController.Controller().SetDisplayInvent(display_invent, type, page, FindComponent<Text>("SearchText").text);
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
                }
                else
                {
                    slot.GetChild(1).gameObject.SetActive(false);
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

    /// <summary>
    /// Reset the search input gui
    /// </summary>
    private void ResetSearchInput()
    {
        FindComponent<Text>("SearchText").text = "";
        FindComponent<Text>("SearchPlaceholder").text = "Search By Name ...";
    }

    public void SetCurrPanel(string panel)
    {
        curr_panel = panel;
        
        switch(panel)
        {
            case "EquipCraftPanel":
                FindComponent<Button>("PotionTypeBtn").interactable = false;
                FindComponent<Button>("ItemTypeBtn").interactable = false;
                break;
            case "CharacterPanel":
                FindComponent<Button>("PotionTypeBtn").interactable = true;
                FindComponent<Button>("ItemTypeBtn").interactable = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Add custom event listener on each slot, show item information
    /// </summary>
    private void AddCustomeEvent()
    {
        for(int i = 0; i < 30; i ++)
        {
            Button slot = FindComponent<Button>("InventSlot (" + i + ")");
            GUIController.AddCustomEventListener(slot,
                EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(slot, 
                EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
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
        return int.Parse( name.Substring(name.IndexOf("(")+1, 1) );
    }
}
