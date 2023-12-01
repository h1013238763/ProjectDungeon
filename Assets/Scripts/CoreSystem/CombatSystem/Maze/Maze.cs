using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The maze generator and maze event controller
/// </summary>

[CreateAssetMenu(fileName = "Maze", menuName = "ProjectDungeon/Maze", order = 0)]
public class Maze : ScriptableObject {
    
    public string maze_id;
    public int maze_level;

    public List<EnemyGroup> normal_enemy_groups;   // possible normal enemy
    public List<EnemyGroup> elite_enemy_groups;    // possible elite
    public EnemyGroup boss_enemy_groups;     // the boss

    public List<string> normal_drops;
    public List<string> elite_drops;
    public List<string> boss_drops;

    public string route_music;
    public string boss_music;

    public Sprite background;

    public EnemyGroup GetRandomEnemy(RoomType type)
    {
        if(type == RoomType.Enemy)
        {
            return normal_enemy_groups[Random.Range(0, normal_enemy_groups.Count-1)];
        }
        else if(type == RoomType.Elite)
        {
            return elite_enemy_groups[Random.Range(0, elite_enemy_groups.Count-1)];
        }
        else if(type == RoomType.Boss)
        {
            return boss_enemy_groups;
        }
        else if(type == RoomType.Quest)
        {
            return elite_enemy_groups[Random.Range(0, elite_enemy_groups.Count-1)];
        }
        return null;
    }

    public Item GetRandomDrop(int difficulty)
    {
        // normal drop
        if(difficulty == 1)
        {
            string item = normal_drops[Random.Range(0, normal_drops.Count-1)];
            Item result = new Item(item, 1);
            return result;
        }
        // elite drop
        else if(difficulty == 2)
        {
            string item = normal_drops[Random.Range(0, elite_drops.Count-1)];
            Item result = new Item(item, 1);
            return result;
        }
        // boss drop
        else
        {
            string item = normal_drops[Random.Range(0, boss_drops.Count-1)];
            Item result = new Item(item, 1);
            return result;
        }

    }

}