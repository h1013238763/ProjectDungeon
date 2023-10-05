using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define all potion attributes
/// Author: Xiaoyue Zhang
/// Last Change: 9/29
/// </summary>
[CreateAssetMenu(fileName = "New Potion", menuName = "ProjectDungeon/Potion", order = 2)]
public class Potion : Item
{
    public int potion_value;
    public PotionEffect potion_effect;
    public int potion_max;
    
}
