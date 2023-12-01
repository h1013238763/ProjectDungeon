using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SkillController : BaseController<SkillController>
{
    // Player learn skill
    private Dictionary<string, SkillData> dict_skill = new Dictionary<string, SkillData>();
    private Dictionary<string, Sprite> image_skill = new Dictionary<string, Sprite>();
    public Dictionary<SkillCareer, List<string>> career_skill_list = new Dictionary<SkillCareer, List<string>>();
    public PlayerSkill data;


    public SkillData GetSkill(string id)
    {
        if(!dict_skill.ContainsKey(id))
            return null;
        return dict_skill[id];
    }

    public Sprite GetImage(string id)
    {
        if(!image_skill.ContainsKey(id))
            return null;
        return image_skill[id];
    }


    // upgrade a skill
    public void AddSkillLevel(string id, int build_index)
    {
        // skill available check
        if(!dict_skill.ContainsKey(id))
            return;
        // point available check
        if(GetRemainPoint(build_index) <= 0)
            return;

        // add skill into avail
        if( !data.avail_skills[build_index].ContainsKey(id))
        {
            data.avail_skills[build_index].Add(id, 0);

        }

        // skill level cap check
        SkillData skill = dict_skill[id];
        if(data.avail_skills[build_index][id] >= skill.skill_level_cap)
            return;

        data.avail_skills[build_index][id] ++;
        data.career_point[build_index][skill.skill_career] ++;
    }

    // reset skill points
    public void ResetSkill(bool all_career, int build_index, SkillCareer career = SkillCareer.Null)
    {
        if(all_career)
        {
            // reset all career point
            data.career_point[build_index].Clear();
            {
                data.career_point[build_index].Add(SkillCareer.Fighter, 0);
                data.career_point[build_index].Add(SkillCareer.Archer, 0);
                data.career_point[build_index].Add(SkillCareer.Mage, 0);
                data.career_point[build_index].Add(SkillCareer.Priest, 0);
            }
            // clear avail skills and their skill levels
            data.avail_skills[build_index].Clear();
        }
        else
        {
            // reset career point
            data.career_point[build_index][career] = 0;

            // remove skill
            List<string> remove_list = new List<string>();
            foreach(var pair in data.avail_skills[build_index])
            {
                if(dict_skill[pair.Key].skill_career == career)
                    remove_list.Add(pair.Key);
            }
            foreach(string skill_id in remove_list)
            {
                data.avail_skills[build_index].Remove(skill_id);
            }
        }
    }

    public List<string> GetCareerSkills(SkillCareer career)
    {
        return career_skill_list[career];
    }

    public List<string> GetAvailSkills(int build_index)
    {
        List<string> result = new List<string>();

        if(data == null)
            return result;
        if(data.avail_skills == null)
            return result;
        if(data.avail_skills[build_index].Count == 0)
            return result;

        foreach( var pair in data.avail_skills[build_index])
        {
            result.Add(pair.Key);
        }

        return result;
    }

    // get target skill and its level
    public KeyValuePair<SkillData, int> GetSkillInfo(string id)
    {
        int build_index = PlayerController.Controller().data.player_build_index;

        if(id == null || id == "")
            return new KeyValuePair<SkillData, int>(null, 0);
        if(!dict_skill.ContainsKey(id))
            return new KeyValuePair<SkillData, int>(null, 0);

        int level = 0;
        // check avail skill
        if(data.avail_skills[build_index].ContainsKey(id))
            level = data.avail_skills[build_index][id];
        // check passive
        if(data.passive_skills[build_index].ContainsKey(id))
            level = data.passive_skills[build_index][id];

        return new KeyValuePair<SkillData, int>(dict_skill[id], level);
        
    }

    public void UseSkill(string skill_id, int unit_index, int target_center, List<int> exist_enemy)
    {
        SkillData skill_info = GetSkill(skill_id);

        // player target skill sound effect
        AudioController.Controller().StartSound("Skill/"+skill_id);

        // trigger all skill effect
        foreach( SkillData.SkillEffect effect in skill_info.skill_effects)
        {
            // analysis affect target
            int unit_pos = 7 - target_center;
            List<int> target_index = new List<int>();

            // if only affect player
            if(effect.effect_range[0] == -1)
            {
                target_index.Add(-1);
            }
            // enemy in range
            else
            {
                for(int i = 0; i < effect.effect_range.Length; i ++)
                {
                    // target in range
                    if( exist_enemy.Contains(effect.effect_range[i] - unit_pos) )
                        target_index.Add(effect.effect_range[i]-unit_pos);
                }
            }

            // trigger effect to target in range
            for(int i = 0; i < target_index.Count; i ++)
            {
                // effect on player
                TriggerSkillEffect(effect, GetSkillInfo(skill_info.skill_id).Value, unit_index, target_index[i]);
            }
        }
    }

    private void TriggerSkillEffect(SkillData.SkillEffect effect, int level, int from, int to)
    {
        switch(effect.skill_effect)
        {
            // cause damage to 
            case SkillEffectType.Damage:
                BattleController.Controller().CauseDamage(effect.skill_value[level], effect.value_id, effect.weakness, from, to);
                break;
            case SkillEffectType.Heal:
                break;
            case SkillEffectType.Buff:
                break;
            default:
                break;
        }
    }

    // calculate the skill point remain for current build
    public int GetRemainPoint(int build_index)
    {
        int result = data.skill_points;
        result -= data.career_point[build_index][SkillCareer.Fighter];
        result -= data.career_point[build_index][SkillCareer.Archer];
        result -= data.career_point[build_index][SkillCareer.Mage];
        result -= data.career_point[build_index][SkillCareer.Priest];
        return result;
    }

    public int GetSkillLevel(string skill_id)
    {
        int build_index = PlayerController.Controller().data.player_build_index;
        return ( data.avail_skills[build_index].ContainsKey(skill_id)) ? data.avail_skills[build_index][skill_id] : 0;
    }

    // return formatted rich text skill describe
    public string GetDescribe(string skill_id, BattleUnit unit = null)
    {
        SkillData skill = dict_skill[skill_id];
        int build_index = PlayerController.Controller().data.player_build_index;
        int level = (data.avail_skills[build_index].ContainsKey(skill_id)) ? data.avail_skills[build_index][skill_id] : 1;

        string result = "";
        // basic describe
        result += skill.skill_describe + "\n\n";

        // effect describe
        if(unit == null)    // detail
        {
            for(int i = 0; i < skill.skill_effects.Count; i ++)
            {
                result += string.Format(skill.skill_effects[i].effect_describe, (skill.skill_effects[i].skill_value[level-1]*100+"%"));
                result += " ";
            }
        }
        else                // general
        {
            for(int i = 0; i < skill.skill_effects.Count; i ++)
            {
                result += string.Format(skill.skill_effects[i].effect_describe, (skill.skill_effects[i].skill_value[level-1]*100));
                result += " ";
            }
        }

        return result;
    }

    // count all basic attributes buffed by passive
    public int CountPassive(string type)
    {
        return 0;
    }

    public void InitialData()
    {
        // Read all xml file
        TextAsset[] files = Resources.LoadAll<TextAsset>("Object/Skill/");
        foreach(TextAsset file in files)
        {
            SkillData skill = XmlController.Controller().DeserializeFile(typeof(SkillData), file as TextAsset) as SkillData;

            if(skill != null)
            {
                // register skill
                image_skill.Add(skill.skill_id, GetImage(skill.skill_id));
                dict_skill.Add(skill.skill_id, skill);
                // key check
                if(!career_skill_list.ContainsKey(skill.skill_career))
                    career_skill_list.Add(skill.skill_career, new List<string>());
                // value size check
                for( int i = career_skill_list[skill.skill_career].Count; i <= skill.skill_pos; i ++)
                    career_skill_list[skill.skill_career].Add("");
                // assign skill
                career_skill_list[skill.skill_career][skill.skill_pos] = skill.skill_id;
            }
        }

        Sprite[] images = Resources.LoadAll<Sprite>("Image/Skill/");
        if(images != null)
        {
            image_skill.Clear();
            foreach(Sprite image in images)
            {
                image_skill.Add(image.name, image);
            }
        }
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(PlayerSkill), "SkillData") as PlayerSkill;
        if(data == null)
            NewData();
    }
    public void SaveData()
    {
        XmlController.Controller().SaveData(data, "SkillData");
    }
    public void NewData()
    {
        data = new PlayerSkill();

        // player freshman gift
        PlayerBuild build = PlayerController.Controller().data.player_build[0];

        build.skills.Add("PowerfulSlash");
        build.skills.Add("RapidFire");
        build.skills.Add("FireBall");

        AddSkillLevel("PowerfulSlash", 0);
        AddSkillLevel("RapidFire", 0);
        AddSkillLevel("FireBall", 0);

        Debug.Log("SkillNew");
    }
}

public class PlayerSkill
{
    public int skill_points;
    public List<XmlDictionary<SkillCareer, int>> career_point;  // [build] (career, point)
    public List<XmlDictionary<string, int>> avail_skills;          // [build] (skill id, skill level) 
    public List<XmlDictionary<string, int>> passive_skills;        // [build] (skill id, skill level) 

    public PlayerSkill()
    {
        skill_points = 3;
        career_point = new List<XmlDictionary<SkillCareer, int>>();
        for(int i = 0; i < 5; i ++)
        {
            career_point.Add(new XmlDictionary<SkillCareer, int>());
            career_point[i].Add(SkillCareer.Fighter, 0);
            career_point[i].Add(SkillCareer.Archer, 0);
            career_point[i].Add(SkillCareer.Mage, 0);
            career_point[i].Add(SkillCareer.Priest, 0);
        }

        avail_skills = new List<XmlDictionary<string, int>>();
        for(int i = 0; i < 5; i ++)
            avail_skills.Add(new XmlDictionary<string, int>());

        passive_skills = new List<XmlDictionary<string, int>>();
        for(int i = 0; i < 5; i ++)
            passive_skills.Add(new XmlDictionary<string, int>());
    }
}
