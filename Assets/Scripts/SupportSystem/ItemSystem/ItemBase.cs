using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all items
/// Author: Xiaoyue Zhang
/// Last Change: 9/29
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "ProjectDungeon/Item", order = 0)]
public class ItemBase : ScriptableObject
{
    public string item_id;      // the id of item
    public string item_name;    // the name of item
    [TextArea]
    public string item_describe;// the description of ite
    public int item_price;      // the price of item
    public int item_tier;       // the tier of item
}
