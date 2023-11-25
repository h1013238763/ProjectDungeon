using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController<PlayerController>
{
    public PlayerData data;
    public PlayerPanel panel;

    public PlayerController()
    {

    }

    public bool Equip(Item item, string type)
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
        }
        else if(type == "Potion")
        {
            // size = 10
            for(int i = 0; i < build.potions.Count; i ++)
            {
                if(build.potions[i] == null)
                {
                    build.potions[i] = item as Potion;
                    panel.ResetPanel();
                    return true;
                } 
            }
        }
        return true;
    }

    public int GetLevelExp(int level)
    {
        // level exp = 10 * (level / 5) ^ 2 + 100;
        return 10 * (level/5) * (level/5) + 100;
    }

    public void GetExp(int exp)
    {
        data.player_exp += exp;
        if(data.player_exp >= GetLevelExp(data.player_level))
        {
            data.player_exp -= GetLevelExp(data.player_level);
            data.player_level ++;
        }
    }

    public void SaveData()
    {
        XmlController.Controller().SaveData(data, "PlayerData");
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(PlayerData), "PlayerData") as PlayerData;            
    }
    public void NewData()
    {
        data = new PlayerData();
    }
}

public class PlayerData
{
    public string player_name;      // the name of player

    public int base_attack;
    public int base_defense;
    public int base_health;

    public int player_exp;          // the exp player own
    public int player_level;        // the level of player
    public int player_money;   // the money player own
    public List<PlayerBuild> player_build;
    public int player_build_index;

    public PlayerData()
    {
        player_name = "";

        base_attack = 10;
        base_defense = 10;
        base_health = 100;

        player_exp = 0;
        player_level = 1;
        player_money = 0;

        player_build = new List<PlayerBuild>();
        for(int i = 0; i < 5; i ++)
            player_build.Add(new PlayerBuild());

        player_build_index = 0;
    }
}

public class PlayerBuild
{
    public List<Equip> equips;
    public List<Skill> skills;
    public List<Potion> potions;

    public PlayerBuild()
    {
        equips = new List<Equip>(new Equip[6]);
        skills = new List<Skill>(new Skill[8]);
        potions = new List<Potion>(new Potion[8]);
    }
}



public class PlayerItem
{

}