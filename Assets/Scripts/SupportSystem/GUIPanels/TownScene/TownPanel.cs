using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ButtonClick");
        
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
}
