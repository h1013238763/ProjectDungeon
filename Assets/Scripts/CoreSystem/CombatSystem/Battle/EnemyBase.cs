using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ProjectDungeon/Enemy", order = 0)]
public class EnemyBase : ScriptableObject
{
    public string enemy_id;
    public string enemy_name;
    public int enemy_tier;

    public int enemy_health;
    public int enemy_attack;
    public int enemy_defense;

    public float tough_percent;         // tough = percentage of health, normal about 1/2, elite about 1/3, boss about 1/4
    public float enemy_health_grow;
    public float enemy_attack_grow;
    public float enemy_defense_grow;

    public Weakness enemy_weakness;

    public List<string> enemy_action;

    public int GetAttack(int level)
    {
        return (int)(enemy_attack + enemy_attack_grow * level);
    }

    public int GetDefense(int level)
    {
        return (int)(enemy_defense + enemy_defense_grow * level);
    }

    public int GetHealth(int level)
    {
        return (int)(enemy_health + enemy_health_grow * level);
    }

    public int GetTough(int level)
    {
        return (int)(GetHealth(level) * tough_percent);
    }
}
