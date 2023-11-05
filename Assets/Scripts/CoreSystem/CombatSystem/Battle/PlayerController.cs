using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController<PlayerController>
{
    public string player_name;      // the name of player
    public int player_exp;          // the exp player own
    public int player_level;        // the level of player
    public int player_skill_point;  // the skill point player own
    public int player_money;   // the money player own
    public List<PlayerBuild> player_build;
    public int player_build_index;

    public PlayerController()
    {
        // test intial
        player_build = new List<PlayerBuild>();
        player_build_index = 0;

        player_build.Add( new PlayerBuild());
        player_build[0].skills = new Skill[10];
        player_build[0].potions = new Potion[10];
        player_build[0].skills[0] = SkillController.Controller().DictSkillInfo("NormalAttack");
        player_build[0].potions[0] = new Potion("NormalHealingPotion", 10);
    }
}

public class PlayerBuild
{
    public Equip[] equips;
    public Skill[] skills;
    public Potion[] potions;
}