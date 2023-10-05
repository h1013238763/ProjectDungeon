using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanel : PanelBase
{
    

    public void DisplayInfo(string item_id, string type)
    {
        switch(type)
        {
            case "Equip":
                IDisplayInfo<Equip>(ItemController.Controller().InfoEquip(item_id));
                break;
            case "Item":
                IDisplayInfo<Item>(ItemController.Controller().InfoItem(item_id));
                break;
            case "Potion":
                IDisplayInfo<Potion>(ItemController.Controller().InfoPotion(item_id));
                break;
            default:
                break;
        }
    }
    // implement of display info of item
    private void IDisplayInfo<T>(T item) where T : Item
    {
        
    }
}
