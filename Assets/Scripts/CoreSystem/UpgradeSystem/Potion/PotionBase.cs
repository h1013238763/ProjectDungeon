using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define all potion attributes
/// Author: Xiaoyue Zhang
/// Last Change: 9/29
/// </summary>
[CreateAssetMenu(fileName = "New Potion", menuName = "ProjectDungeon/Potion", order = 2)]
public class PotionBase : ItemBase
{
    public int potion_value;
    public PotionEffect potion_effect;
    public bool potion_effect_ally;
    public int potion_max;
    public int potion_cost;
}

public enum PotionEffect
{
    Heal
}
