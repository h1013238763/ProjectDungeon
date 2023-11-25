using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The maze generator and maze event controller
/// </summary>

[CreateAssetMenu(fileName = "Maze", menuName = "ProjectDungeon/Maze", order = 0)]
public class Maze : ScriptableObject {
    
    public int maze_level;

    public List<EnemyGroup> normal_enemy_groups;   // possible normal enemy
    public List<EnemyGroup> elite_enemy_groups;    // possible elite
    public EnemyGroup boss_enemy_groups;     // the boss

    public List<Item> normal_drops;
    public List<Item> elite_drops;
    public List<Item> boss_drops;

    public EnemyGroup GetRandomEnemy(string type, bool is_quest)
    {
        if(is_quest)
        {
            return null;
        }
        else
        {
            return null;
        }
    }

    public Item[] GetRandomDrop(int difficulty)
    {
        // normal drop
        if(difficulty == 1)
        {
            return null;
        }
        // elite drop
        else if(difficulty == 2)
        {
            return null;
        }
        // boss drop
        else
        {
            return null;
        }

    }

}