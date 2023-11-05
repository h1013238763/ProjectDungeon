using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ProjectDungeon/Enemy", order = 0)]
public class EnemyBase : ScriptableObject
{
    public string enemy_id;
    public string enemy_name;

    public int enemy_health;
    public int enemy_attack;
    public int enemy_defense;
    public int enemy_tough;

    public int enemy_health_grow;
    public int enemy_attack_grow;
    public int enemy_defense_grow;

    public EnemyEffect enemy_effect;

    public List<Skill> enemy_action;
}
