using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    public Potion( string id, int num ) : base(id, num)
    {
        item_tier = ItemController.Controller().DictPotionInfo(id).item_tier;
    }

    public override int GetPrice()
    {
        return ItemController.Controller().DictPotionInfo(item_id).item_price;
    }

    public override string ToString()
    {
        return "Potion ["+item_id+", "+item_num+"]";
    }
}
