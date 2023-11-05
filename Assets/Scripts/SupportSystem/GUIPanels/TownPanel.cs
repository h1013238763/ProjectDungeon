using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        switch(button_name)
        {
            case "CraftPotionBtn":
                GUIController.Controller().ShowPanel<PotionCraftPanel>("PotionCraftPanel", 1);
                break;
            case "CraftEquipBtn":
                // TODO : set level to player level
                GUIController.Controller().ShowPanel<EquipCraftPanel>("EquipCraftPanel", 1, (panel) =>{
                    panel.player_level = 5;
                });
                break;
            case "CharacterBtn":
                break;
            default:
                break;
        }
    }
}
