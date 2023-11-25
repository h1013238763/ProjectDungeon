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

    private string level_item;
    private string enchant_item;

    public Quest quest;

    public ItemController controller;

    public override void ShowSelf()
    {
        gameObject.SetActive(true);
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1, (p) => {
            p.panel = "EquipCraftPanel";
        });

        controller = ItemController.Controller();
        level_item = controller.equip_strengthen_item;
        enchant_item = controller.equip_enchant_item;

        ResetPanel();
        AddCustomeEvent();
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name == "CloseBtn")
        {
            AudioController.Controller().StartSound("ButtonClick");

            GUIController.Controller().RemovePanel("EquipCraftPanel");
            GUIController.Controller().RemovePanel("InventPanel");
        }

        if(button_name == "EquipSlot")
        {
            AudioController.Controller().StartSound("Equip");

            equip = null;
            GUIController.Controller().GetPanel<InventPanel>("InventPanel").ResetInventPanel();
            ResetEquip();
        }
        else if(button_name.Contains("TagBtn"))
        {
            AudioController.Controller().StartSound("Equip");

            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            if(!enchant_list.Contains(index))
                enchant_list.Add(index);
            else
                enchant_list.Remove(index);

            ResetTag();
        }
        else if(button_name == "StrengthBtn")
        {
            AudioController.Controller().StartSound("Enhence");

            controller.StrengthenEquip(equip);
            ResetEquip();
        }
        else if(button_name == "EnchantBtn")
        {
            AudioController.Controller().StartSound("Enhence");

            controller.ResetEnchantment(equip, enchant_list);
            enchant_list.Clear();
            ResetTag();
        }
        else if(button_name == "QuestTip")
        {
            AudioController.Controller().StartSound("AcceptQuest");

            EventController.Controller().EventTrigger<Quest>("AcceptQuest", quest);
            quest = null;
            ResetPanel();
        }
    }

    public void SetEquip(Equip item)
    {
        equip = item;
        ResetEquip();
    }

    private void ResetEquip()
    {
        EquipBase info = controller.DictEquipInfo(equip.item_id);

        FindComponent<Text>("EquipLevel").text = "LV. " + equip.equip_level;

        FindComponent<Image>("EquipImage").sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+equip.item_id);
        FindComponent<Image>("EquipImage").color = new Color(255,255,225,255);

        FindComponent<Text>("EquipName").text = controller.DictEquipInfo(equip.item_id).item_name;
        FindComponent<Text>("AttackValue").text = equip.GetAttributes("Attack").ToString();
        FindComponent<Text>("DefenseValue").text = equip.GetAttributes("Defense").ToString();
        FindComponent<Text>("HealthValue").text = equip.GetAttributes("Health").ToString();
        FindComponent<Text>("AttackGrowValue").text = "+ "+equip.GetAttributesIncrease("Attack").ToString();
        FindComponent<Text>("DefenseGrowValue").text = "+ "+equip.GetAttributesIncrease("Defense").ToString();
        FindComponent<Text>("HealthGrowValue").text = "+ "+equip.GetAttributesIncrease("Health").ToString();
        
        Text level_text = FindComponent<Image>("StrengthCost").transform.transform.GetChild(2).GetComponent<Text>();
        Item item = controller.InventItemInfo(level_item);

        level_text.text = "x " + controller.GetStrengthCost(equip) + " ( ";
        level_text.text += ( item != null) ? item.item_num+" )" : "0 )";

        FindComponent<Button>("StrengthBtn").interactable = controller.LevelCostCheck(equip);
        if( equip.equip_level >= player_level)
            FindComponent<Button>("StrengthBtn").interactable = false;

        ResetTag();
    }

    private void ResetTag()
    {
        if( equip == null )
            return;
        for(int i = 0; i < 5; i ++)
        {
            if( i < equip.enchant_limit)
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

        Text level_text = FindComponent<Image>("EnchantCost").transform.transform.GetChild(2).GetComponent<Text>();
        Item item = controller.InventItemInfo(enchant_item);

        level_text.text = "x " + controller.GetEnchantCost(equip, enchant_list.Count) + " ( ";
        level_text.text += ( item != null) ? item.item_num+" )" : "0 )";

        FindComponent<Button>("EnchantBtn").interactable = controller.EnchantCostCheck(equip, enchant_list.Count);

    }

    private void ResetPanel()
    {
        FindComponent<Button>("QuestTip").gameObject.SetActive(quest != null);

        enchant_list.Clear();
        // basic attributes
        if(equip == null)
        {
            FindComponent<Image>("EquipImage").color = new Color(255,255,225,0);
            FindComponent<Text>("EquipName").text = "";
            FindComponent<Text>("AttackValue").text = "0";
            FindComponent<Text>("DefenseValue").text = "0";
            FindComponent<Text>("HealthValue").text = "0";
            FindComponent<Text>("AttackGrowValue").text = "+ 0";
            FindComponent<Text>("DefenseGrowValue").text = "+ 0";
            FindComponent<Text>("HealthGrowValue").text = "+ 0";
        }
        
        // tags
        for(int i = 0; i < 5; i ++)
        {
            FindComponent<Button>("TagBtn ("+i+")").interactable = false;
            FindComponent<Button>("TagBtn ("+i+")").transform.GetChild(0).GetComponent<Text>().text = "";
        }
        FindComponent<Text>("TagDescribeText").text = "";
        // Strengthen Cost
        Transform cost_component = FindComponent<Image>("StrengthCost").transform;
        Item cost_item = controller.InventItemInfo(level_item);

        cost_component.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+cost_item.item_id);
        cost_component.GetChild(1).GetComponent<Text>().text = controller.DictItemInfo(cost_item.item_id).item_name;

        if(cost_item != null)
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( "+cost_item.item_num+" )";
        else
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( 0 )";
        // Enchant Cost
        cost_component = FindComponent<Image>("EnchantCost").transform;
        cost_item = controller.InventItemInfo(enchant_item);

        cost_component.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+cost_item.item_id);
        cost_component.GetChild(1).GetComponent<Text>().text = controller.DictItemInfo(cost_item.item_id).item_name;
        if(cost_item != null)
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( "+cost_item.item_num+" )";
        else
            cost_component.GetChild(2).GetComponent<Text>().text = "x 0 ( 0 )";
        // Buttons
        FindComponent<Button>("StrengthBtn").interactable = false;
        FindComponent<Button>("EnchantBtn").interactable = false;
    }


    /// Add custom event listener on each slot, show item information
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
    // show info panel on pointer enter
    private void OnPointerEnter(PointerEventData event_data)
    {
        // get index
        int index = GetPointerObjectIndex(event_data);

        if(index < 0)
            return;
        if(FindComponent<Button>("TagBtn ("+index+")").interactable == false )
            return;

        FindComponent<Text>("TagDescribeText").text =  equip.equip_enchants[index].enchant_describe;
    }
    // hide info panel on pointer exit
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
}
