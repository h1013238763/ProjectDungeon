using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Skill", menuName = "ProjectDungeon/Skill", order = 0)]
public class Skill : ScriptableObject {

    public string skill_id;
    public string skill_name;

    [TextArea]
    public string skill_describe;

    public int skill_level;

    public int skill_cost;
    public int skill_cold;
    public int cold_left;

    public bool is_melee;

    public string[] skill_value;

    public SkillCareer skill_career;

    public SkillEffect skill_effect;
    public string skill_weak;

    public int skill_pos;

    public bool affect_ally;
}

public enum SkillCareer
{
    Null,
    Fighter,
    Archer,
    Mage,
    Priest
}

public enum SkillEffect
{
    Rest,
    Damage,
    Use,
}