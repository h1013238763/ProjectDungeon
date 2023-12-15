using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController<PlayerController>
{
    public PlayerData data;
    public PlayerPanel panel;

    public int skill_limit = 8;
    public int potion_limit = 8;

    public bool Equip(string type, Item item, string id = null)
    {
        PlayerBuild build = data.player_build[data.player_build_index];

        if(type == "Item")
        {
            return false;
        }
        else if(type == "Equip")
        {
            Equip equip = item as Equip;
            build.equips[(int)equip.equip_type] = equip;
            panel.ResetPanel();
            return true;
        }
        else if(type == "Potion")
        {
            if(build.potions.Count < potion_limit && !build.potions.Contains(item.item_id))
            {
                build.potions.Add(item.item_id);
                panel.ResetPanel();
                return true;
            }
        }
        else if(type == "Skill")
        {
            if(build.skills.Count < skill_limit && !build.skills.Contains(id))
            {
                build.skills.Add(id);
                return true;
            }
        }
        return true;
    }

    public BattleUnit CreateUnit()
    {
        BattleUnit unit = new BattleUnit();

        unit.unit_id = "Player";
        unit.unit_name = "Player";
        unit.unit_index = -1;

        // basic attributes
        unit.unit_level = data.player_level;
        unit.health_max = GetHealth();
        unit.health_curr = unit.health_max;

        unit.attack = GetAttack();
        unit.extra_attack = new Dictionary<string, int>();

        unit.defense = GetDefense();
        unit.extra_defense = new Dictionary<string, int>();
        
        unit.in_control = false;

        unit.unit_buffs = new List<Buff>();

        return unit;
    }

    public int GetAttack()
    {
        float result = 0;
        // calculate basic attributes
        result = 10 + 0.9f * data.player_level * data.player_level;
        // value = basic + growth * level^2
        // buff from passive
        result += SkillController.Controller().CountPassive("Attack");

        // buff from equip
        foreach(Equip equip in data.player_build[data.player_build_index].equips )
        if(equip != null)
            result += equip.GetAttributes("Attack");

        // return
        return (int)result;
    }

    public int GetDefense()
    {
        float result = 0;
        // calculate basic attributes
        result = 10 + 0.9f * data.player_level * data.player_level;
        // value = basic + growth * level^2
        // buff from passive
        result += SkillController.Controller().CountPassive("Defense");

        // buff from equip
        foreach(Equip equip in data.player_build[data.player_build_index].equips )
        if(equip != null)
            result += equip.GetAttributes("Defense");

        // return
        return (int)result;
    }

    public int GetHealth()
    {
        float result = 0;
        // calculate basic attributes
        result = 100 + 2.4f * data.player_level * data.player_level;
        // value = basic + growth * level^2
        // buff from passive
        result += SkillController.Controller().CountPassive("Health");

        // buff from equip
        foreach(Equip equip in data.player_build[data.player_build_index].equips )
        if(equip != null)
            result += equip.GetAttributes("Health");

        // return
        return (int)result;
    }

    public int GetLevelExp(int level)
    {
        // level exp = 10 * (level / 5) ^ 2 + 100;
        return 10 * (level/5) * (level/5) + 100;
    }

    public void GetExp(int exp)
    {
        data.player_exp += exp;
        while(data.player_exp >= GetLevelExp(data.player_level))
        {
            data.player_exp -= GetLevelExp(data.player_level);
            data.player_level ++;
        }
    }

    public PlayerBuild GetCurrBuild()
    {
        return data.player_build[data.player_build_index];
    }

    public void InitialData()
    {
        data = XmlController.Controller().LoadData(typeof(PlayerData), "PlayerData") as PlayerData;
    }
    public void SaveData()
    {
        XmlController.Controller().SaveData(data, "PlayerData");
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(PlayerData), "PlayerData") as PlayerData;

        Debug.Log("name : " + data.player_name);
        Debug.Log("level : " + data.player_level);
        Debug.Log("exp : " + data.player_exp);
        Debug.Log("build_index : " + data.player_build_index);

        foreach(Equip equip in data.player_build[data.player_build_index].equips)
            Debug.Log(equip);
        foreach(string potion in data.player_build[data.player_build_index].potions)
            Debug.Log(potion);
        foreach(string skill in data.player_build[data.player_build_index].skills)
            Debug.Log(skill);
    }
    public void NewData()
    {
        data = new PlayerData();
    }
}

public class PlayerData
{
    public string player_name;      // the name of player

    public int player_exp;          // the exp player own
    public int player_level;        // the level of player
    public int player_money;        // the money player own
    public List<PlayerBuild> player_build;
    public int player_build_index;  
    public int maze_progress;

    public PlayerData()
    {
        player_name = "Player";

        player_exp = 0;
        player_level = 1;
        player_money = 0;

        player_build = new List<PlayerBuild>();
        for(int i = 0; i < 5; i ++)
            player_build.Add(new PlayerBuild());

        player_build_index = 0;
        maze_progress = 0;
    }
}

public class PlayerBuild
{
    public List<Equip> equips;
    public List<string> skills;
    public List<string> potions;

    public PlayerBuild()
    {
        equips = new List<Equip>(new Equip[6]);
        skills = new List<string>();
        potions = new List<string>();
    }
}