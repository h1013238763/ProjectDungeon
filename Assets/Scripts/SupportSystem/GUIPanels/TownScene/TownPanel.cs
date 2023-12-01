using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TownPanel : PanelBase
{

    public override void ShowSelf()
    {
        SetCostumeEvent();
    }

    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ShopRing");
        
        switch(button_name)
        {
            case "CraftPotionBtn":
                GUIController.Controller().ShowPanel<PotionCraftPanel>("PotionCraftPanel", 1);
                break;
            case "CraftEquipBtn":
                // TODO : set level to player level
                GUIController.Controller().ShowPanel<EquipCraftPanel>("EquipCraftPanel", 1);
                break;
            case "CharacterBtn":
                GUIController.Controller().ShowPanel<PlayerPanel>("PlayerPanel", 1);
                break;
            case "SkillLearnBtn":
                GUIController.Controller().ShowPanel<SkillLearnPanel>("SkillLearnPanel", 1);
                break;
            // Shops
            case "EquipShopBtn":
                GUIController.Controller().ShowPanel<ShopPanel>("EquipShopPanel", 1, (panel) =>{
                    panel.type = "Equip";
                });
                break;
            case "PotionShopBtn":
                GUIController.Controller().ShowPanel<ShopPanel>("PotionShopPanel", 1, (panel) =>{
                    panel.type = "Potion";
                });
                break;
            case "ItemShopBtn":
                GUIController.Controller().ShowPanel<ShopPanel>("ItemShopPanel", 1, (panel) =>{
                    panel.type = "Item";
                });
                break;
            default:
                break;
        }
    }

    // Add custom event listener on each slot, show item information
    public void SetCostumeEvent()
    {
        List<Button> btns = new List<Button>();
        btns.Add(FindComponent<Button>("CraftPotionBtn"));
        btns.Add(FindComponent<Button>("CraftEquipBtn"));
        btns.Add(FindComponent<Button>("CharacterBtn"));
        btns.Add(FindComponent<Button>("SkillLearnBtn"));
        btns.Add(FindComponent<Button>("EquipShopBtn"));
        btns.Add(FindComponent<Button>("PotionShopBtn"));
        btns.Add(FindComponent<Button>("ItemShopBtn"));

        foreach(Button btn in btns)
        {
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
    }

    // visual effect
    private void OnPointerEnter(PointerEventData event_data)
    {
        FindComponent<Button>(event_data.pointerEnter.name).gameObject.GetComponent<Outline>().enabled = true;
    }
    private void OnPointerExit(PointerEventData event_data)
    {
        FindComponent<Button>(event_data.pointerEnter.name).gameObject.GetComponent<Outline>().enabled = false;
    }
    
}
