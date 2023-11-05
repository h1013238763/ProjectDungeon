using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController<EnemyController>
{
    public Dictionary<string, EnemyBase> enemy_dict = new Dictionary<string, EnemyBase>();
    public List<EnemyGroup> normal_enemy_groups = new List<EnemyGroup>();
    public List<EnemyGroup> elite_enemy_groups = new List<EnemyGroup>();
    public List<EnemyGroup> boss_enemy_groups = new List<EnemyGroup>();
    public List<EnemyGroup> quest_enemy_groups = new List<EnemyGroup>();

    public EnemyController()
    {
        for(int i = 1; i <= 5; i ++)
        {
            // add normal enemies
            for(int j = 1; j <= 5; j ++)
            {
                normal_enemy_groups.Add(ResourceController.Controller().Load<EnemyGroup>("Objects/EnemyGroups/"+"Maze_"+i+"_Normal_"+j));
            }
            // add elite enemies

            // add boss enemies
        }
        
    }

    public EnemyGroup GetRandomEnemy(int maze_level, int enemy_tier)
    {
        if(enemy_tier == 1)
            return normal_enemy_groups[maze_level*5 + Random.Range(0, 4)];
        else if(enemy_tier == 2)
            return elite_enemy_groups[maze_level*3 + Random.Range(0, 2)];
        else if(enemy_tier == 3)
            return boss_enemy_groups[maze_level];
        else
            return null;
    }

    public EnemyGroup GetQuestEnemy()
    {
        // TODO : return target qeust enemy
        return null;
    }
}

public enum EnemyEffect
{
    Normal,     // no effect or effect have been triggered and lost
    Fly,        // Normally only takes 70% damage, but takes 200% damage when stunned.
    Undead,     // recover 30% when first time die
    Element     // Immunity to certain types of damage
}
