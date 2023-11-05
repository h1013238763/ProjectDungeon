using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : BaseController<SkillController>
{
    private PlayerController player;

    public Dictionary<string, Skill> dict_skill = new Dictionary<string, Skill>();

    public SkillController()
    {
        // player = PlayerController.Controller();

        // foreach(Skill skill in Resources.LoadAll<Skill>("Objects/Skill"))
            // dict_skill.Add(skill.skill_id, skill); 
    }

    public void PerformEffect(Skill skill, BattleUnit user, BattleUnit target)
    {
        switch(skill.skill_effect)
        {
            case SkillEffect.Damage:
                DealDamage(skill, user, target);
                break;
            case SkillEffect.Use:
                UseItem(skill, target);
                break;
            default:
                break;
        }
    }

    private void DealDamage(Skill skill, BattleUnit user, BattleUnit target)
    {
        if(skill.is_melee)
            user.OnMelee(target);
        else
            user.OnRange();
    }

    private void UseItem(Skill skill, BattleUnit target)
    {
        // perform animation
        BattleController.Controller().player_unit.OnPerform();

        foreach(Potion potion in player.player_build[player.player_build_index].potions)
        {
            if(potion.item_id == skill.skill_value)
            {
                potion.item_num --;

            }
        }
    }

    public Skill DictSkillInfo(string id)
    {
        if(dict_skill.ContainsKey(id))
            return dict_skill[id];
        return null;
    }
}

public enum SkillEffect
{
    Rest,
    Damage,
    Use,
}

public enum SkillWeakness
{
    normal,
    slash, 
    strike, 
    thrust,
    fire,
    froze,
    poison
}