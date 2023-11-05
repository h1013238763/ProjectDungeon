using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// prototype of all recipes
/// </summary>
[CreateAssetMenu(fileName = "new Recipe", menuName = "ProjectDungeon/Recipe", order = 3)]
public class Recipe : ScriptableObject {
    public string recipe_id;
    public string recipe_name;
    public string[] recipe_consume;
    public int[] recipe_consume_num;
    public string recipe_result;
    public int recipe_result_num;
    public bool recipe_unlock;
}