using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipCraftPanel : PanelBase
{
    List<int> enchant_list = new List<int>();
    public Equip equip;
    public int player_level = 50;
    private Color unable_color = new Color(0.9f, 0.3f, 0.3f, 1);
    private Color able_color = new Color(1, 1, 1, 1);

    private string level_item;
    private int level_cost;
    private string enchant_item;
    public  int enchant_cost;

    public override void ShowSelf()
    {
        gameObject.SetActive(true);
        ResetPanel();
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1, (panel) => {
            panel.SetCurrPanel("EquipCraftPanel");
        });
        AddCustomeEvent();
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name.Contains("TagBtn"))
        {
            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            if(!enchant_list.Contains(index))
                enchant_list.Add(index);
            else
                enchant_list.Remove(index);

            ResetTag();
        }
        else if(button_name == "StrengthBtn")
        {
            ItemController.Controller().StrengthenEquip(equip, 1);
            ResetEquip();
        }
        else if(button_name == "EnchantBtn")
        {
            ItemController.Controller().ResetEnchantment(equip, enchant_list);
            enchant_list.Clear();
            ResetTag();
        }
    }

    public void SetEquip(Equip item)
    {
        equip = item;
        EquipBase info = ItemController.Controller().DictEquipInfo(equip.item_id);
        ResetEquip();
        ResetTag();
    }

    private void ResetEquip()
    {
        EquipBase info = ItemController.Controller().DictEquipInfo(equip.item_id);

        FindComponent<Text>("EquipLevel").text = "LV. " + equip.equip_level;

        FindComponent<Image>("EquipImage").sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+equip.item_id);
        FindComponent<Image>("EquipImage").color = new Color(255,255,225,255);

        FindComponent<Text>("EquipName").text = info.item_name;
        FindComponent<Text>("AttackValue").text = (info.equip_attack + info.equip_attack_grow * equip.equip_level).ToString();
        FindComponent<Text>("DefenseValue").text = (info.equip_defense + info.equip_defense_grow * equip.equip_level).ToString();
        FindComponent<Text>("HealthValue").text = (info.equip_health + info.equip_health_grow * equip.equip_level).ToString();

        FindComponent<Text>("AttackGrowValue").text = "+ "+info.equip_attack_grow;
        FindComponent<Text>("DefenseGrowValue").text = "+ "+info.equip_defense_grow;
        FindComponent<Text>("HealthGrowValue").text = "+ "+info.equip_health_grow;
        
        Text level_text = FindComponent<Image>("StrengthCost").transform.transform.GetChild(2).GetComponent<Text>();
        Item item = ItemController.Controller().InventItemInfo(level_item);

        level_text.text = "x " + ItemController.Controller().strengthen_item_cost + " ( ";
        level_text.text += ( item != null) ? item.item_num+" )" : "0 )";

        if( ItemController.Controller().LevelCostCheck(level_cost) )
        {
            level_text.color = able_color;
            FindComponent<Button>("StrengthBtn").interactable = true;
        }
        else
        {
            level_text.color = unable_color;
            FindComponent<Button>("StrengthBtn").interactable = false;
        }

        if(equip.equip_level == player_level)
            FindComponent<Button>("StrengthBtn").interactable = false;
    }

    private void ResetTag()
    {
        if( equip == null )
            return;
        for(int i = 0; i < 5; i ++)
        {
            if( i < equip.equip_enchants.Length)
            {
                if( enchant_list.Contains(i) )
                {
                    FindComponent<Image>("TagBtn ("+i+")").color = new Color(0.8f, 0.8f, 0.8f, 1);
                }
                else
                {
                    FindComponent<Image>("TagBtn ("+i+")").color = new Color(1, 1, 1, 1);
                }
                FindComponent<Button>("TagBtn ("+i+")").interactable = true;
                FindComponent<Button>("TagBtn ("+i+")").transform.GetChild(0).GetComponent<Text>().text = equip.equip_enchants[i].ToString();
            }
            else
            {
                FindComponent<Button>("TagBtn ("+i+")").interactable = false;
                FindComponent<Button>("TagBtn ("+i+")").transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }

        Text enchant_text = FindComponent<Image>("EnchantCost").transform.transform.GetChild(2).GetComponent<Text>();
        string equip_enchant_item = ItemController.Controller().equip_enchant_item;
        Item item = ItemController.Controller().InventItemInfo(equip_enchant_item);

        enchant_text.text = "x " + ItemController.Controller().enchant_item_cost * enchant_list.Count + " ( ";
        enchant_text.text += ( item != null) ? item.item_num+" )" : "0 )";

        if(ItemController.Controller().EnchantCostCheck(enchant_cost * enchant_list.Count))
        {
            enchant_text.color = able_color;
            FindComponent<Button>("EnchantBtn").interactable = true;
        }
        else
        {
            enchant_text.color = unable_color;
            FindComponent<Button>("EnchantBtn").interactable = false;
        }
    }

    private void ResetPanel()
    {
        enchant_list.Clear();
        // basic attributes
        FindComponent<Image>("EquipImage").color = new Color(255,255,225,0);
        FindComponent<Text>("EquipName").text = "";
        FindComponent<Text>("AttackValue").text = "0";
        FindComponent<Text>("DefenseValue").text = "0";
        FindComponent<Text>("HealthValue").text = "0";
        FindComponent<Text>("AttackGrowValue").text = "+ 0";
        FindComponent<Text>("DefenseGrowValue").text = "+ 0";
        FindComponent<Text>("HealthGrowValue").text = "+ 0";
        // tags
        for(int i = 0; i < 5; i ++)
        {
            FindComponent<Button>("TagBtn ("+i+")").interactable = false;
            FindComponent<Button>("TagBtn ("+i+")").transform.GetChild(0).GetComponent<Text>().text = "";
        }
        FindComponent<Text>("TagDescribeText").text = "";
        // Strengthen Cost
        Transform cost_component = FindComponent<Image>("StrengthCost").transform;
        string cost_item_name = ItemController.Controller().equip_strengthen_item;
        Item cost_item = ItemController.Controller().InventItemInfo(cost_item_name);

        cost_component.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+cost_item_name);
        cost_component.GetChild(1).GetComponent<Text>().text = ItemController.Controller().DictItemInfo(cost_item_name).item_name;
        if(cost_item != null)
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( "+cost_item.item_num+" )";
        else
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( 0 )";
        // Enchant Cost
        cost_component = FindComponent<Image>("EnchantCost").transform;
        cost_item_name = ItemController.Controller().equip_enchant_item;
        cost_item = ItemController.Controller().InventItemInfo(cost_item_name);

        cost_component.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+cost_item_name);
        if(cost_item != null)
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( "+cost_item.item_num+" )";
        else
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( 0 )";
        // Buttons
        FindComponent<Button>("StrengthBtn").interactable = false;
        FindComponent<Button>("EnchantBtn").interactable = false;
    }

    /// <summary>
    /// Add custom event listener on each slot, show item information
    /// </summary>
    private void AddCustomeEvent()
    {
        for(int i = 0; i < 5; i ++)
        {
            Button slot = FindComponent<Button>("TagBtn (" + i + ")");
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

        if(index < 0)
            return;
        if(FindComponent<Button>("TagBtn ("+index+")").interactable == false )
            return;

        FindComponent<Text>("TagDescribeText").text =  ItemController.Controller().EnchantInfo(equip.equip_enchants[index]).enchant_describe;
    }
    /// <summary>
    /// hide info panel on pointer exit
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerExit(PointerEventData event_data)
    {
        FindComponent<Text>("TagDescribeText").text = "";
    }
    // break PointerEvent into useful info token
    private int GetPointerObjectIndex(PointerEventData event_data)
    {
        string name = event_data.pointerEnter.name;
        return int.Parse( name.Substring(name.IndexOf("(")+1, 1) );
    }

    public int GetLevelCost(Equip equip)
    {
        return equip.equip_level / 5;
    }

    // return if enough item for enchant change
    public int GetEnchantCose()
    {
        return equip.item_tier / 2 * enchant_list.Count;
    }
}
