using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string item_id;      // the id of item
    public int item_num;        // the number of item
    public int item_tier;       // the tier of item

    public Item( string id, int num )
    {
        item_id = id;
        item_num = num;
        if(this.GetType() == typeof(Item))
            item_tier = ItemController.Controller().DictItemInfo(id).item_tier;
    }

    public override string ToString()
    {
        return "Item ["+item_id+", "+item_num+"]";
    }
}