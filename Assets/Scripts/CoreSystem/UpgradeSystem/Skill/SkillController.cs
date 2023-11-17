using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : BaseController<SkillController>
{
    public PlayerSkill data;
    
    public List<Skill> fighter_skill = new List<Skill>();
    public List<Skill> archer_skill = new List<Skill>();
    public List<Skill> mage_skill = new List<Skill>();
    public List<Skill> priest_skill = new List<Skill>();

    // upgrade a skill
    public int AddSkillLevel(string id)
    {
        // skill available check
        Skill skill = DictSkillInfo(id);
        if(skill == null)
            return 0;
        if(skill.skill_level >= 5)
            return 0;
        
        // skill point available check
        int sum = data.point_fighter + data.point_archer + data.point_mage + data.point_priest;
        if( sum >= data.skill_points)
            return 0;

        // add skill level
        skill.skill_level ++;

        // add skill into save data
        if(!data.avail_skill.Contains(skill))
            data.avail_skill.Add(skill);

        // return career point input
        switch(skill.skill_career)
        {
            case SkillCareer.Fighter:
                data.point_fighter ++;
                return data.point_fighter;
            case SkillCareer.Archer:
                data.point_archer ++;
                return data.point_archer;
            case SkillCareer.Mage:
                data.point_mage ++;
                return data.point_mage;
            case SkillCareer.Priest:
                data.point_priest ++;
                return data.point_priest;
            default:
                return 0;
        }
    }

    // reset skill points
    public void ResetSkill(bool all_career, SkillCareer career = SkillCareer.Fighter)
    {
        if(all_career)
        {
            // clear all skill level
            foreach(Skill skill in data.avail_skill)
                skill.skill_level = 0;
            data.avail_skill.Clear();

            // clear all career points
            data.point_fighter = 0;
            data.point_archer = 0;
            data.point_mage = 0;
            data.point_priest = 0;
        }
        else
        {
            // reset career skill level
            List<Skill> reset_list = new List<Skill>();
            foreach(Skill skill in data.avail_skill)
            {
                if(skill.skill_career == career)
                {
                    skill.skill_level = 0;
                    reset_list.Add(skill);
                }
            }

            // remove career from list
            foreach(Skill skill in reset_list)
                data.avail_skill.Remove(skill);

            reset_list.Clear();
        }
    }

    public List<Skill> GetCareerSkills(SkillCareer career)
    {
        if(career == SkillCareer.Fighter)
            return fighter_skill;
        if(career == SkillCareer.Archer)
            return archer_skill;
        if(career == SkillCareer.Mage)
            return mage_skill;
        if(career == SkillCareer.Priest)
            return priest_skill;
        else
            return null;
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
            user.DealMeleeAttack(target);
        else
            user.DealRangeAttack();
    }

    private void UseItem(Skill skill, BattleUnit target)
    {

    }

    public Skill DictSkillInfo(string id)
    {
        return ResourceController.Controller().Load<Skill>("Objects/Skill/"+id);
    }

    public void InitialData()
    {
        Skill[] skills = Resources.LoadAll<Skill>("Objects/Skill/");
        if(skills != null)
        {
            foreach(Skill skill in skills)
            {
                if(skill.skill_career == SkillCareer.Fighter)
                    fighter_skill.Add(skill);
                else if(skill.skill_career == SkillCareer.Archer)
                    archer_skill.Add(skill);
                else if(skill.skill_career == SkillCareer.Mage)
                    mage_skill.Add(skill);
                else if(skill.skill_career == SkillCareer.Priest)
                    priest_skill.Add(skill);
            }
        }
    }
    public void LoadData()
    {

    }
    public void SaveData()
    {

    }
    public void NewData()
    {
        data = new PlayerSkill();
    }
}

public class PlayerSkill
{
    public int skill_points;
    public int point_fighter;
    public int point_archer;
    public int point_mage;
    public int point_priest;
    public List<Skill> avail_skill;

    public PlayerSkill()
    {
        skill_points = 3;
        point_fighter = 0;
        point_archer = 0;
        point_mage = 0;
        point_priest = 0;
        avail_skill = new List<Skill>();
    }
}

public enum ModifyValue
{
    Null,
    Attack,
    Defense,
    Health
}

public enum SkillCareer
{
    Fighter,
    Archer,
    Mage,
    Priest
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

public enum SkillEffect
{
    Rest,
    Damage,
    Use,
}