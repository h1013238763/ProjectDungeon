using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : BaseController<BattleController>
{
    public int turn_count;
    public int action_point;
    public int action_point_max = 5;
    public float turn_time = 30f;
    public bool player_turn;

    public List<Skill> player_skills;

    public int damage_modifier;     // the modifier to manager the damage curve, = maze level * 10
    public int maze_level;

    public BattlePanel frontend;

    public EnemyGroup enemy_group;

    public BattleUnit player_unit;
    public BattleUnit[] enemy_unit;

    public void BattleStart(Room room, int maze_level, int enemy_level, int enemy_tier)
    {
        StageController.Controller().stage = Stage.Battle;

        if(room.room_enemy != null)
            enemy_group = room.room_enemy;
        else
        {
            enemy_group = EnemyController.Controller().GetRandomEnemy(maze_level, enemy_tier);
            room.room_enemy = enemy_group;
        }

        turn_count = 0;
        action_point = 5;

        GUIController.Controller().ShowPanel<BattlePanel>("BattlePanel", 2, (p) => 
        {
            frontend = p;
            // start from player turn
            player_turn = false;
        });
    }

    public void ChangeTurn()
    {
        // switch turn
        player_turn = !player_turn;

        if(player_turn)
            PlayerTurn();
        else
            EnemyTurn();
    }

    // Enter the player's turn
    public void PlayerTurn()
    {
        // turn increase
        turn_count ++;
        action_point += (action_point > 2) ? 5-action_point : 3;

        Debug.Log(frontend);
        Debug.Log(turn_time);
        frontend.SetTime(turn_time);
        // start count time
        frontend.pause = false;
        // show player action panel to allow player perform action
        frontend.PlayerTurn(turn_time, action_point, turn_count);
    }

    public void PlayerAction(int action_index, bool is_skill, int enemy_index)
    {
        // stop time counting
        frontend.pause = true;
        // perform action 0-9 skill, 10-20 item
        Skill skill;
        if(is_skill)
        {
            skill = player_skills[action_index];
        }
        else
        {
            skill = ResourceController.Controller().Load<Skill>("Objects/Skill/UsePotion");
            if(PlayerController.Controller().player_build[PlayerController.Controller().player_build_index].potions[action_index].item_num > 0)
                skill.skill_value = PlayerController.Controller().player_build[PlayerController.Controller().player_build_index].potions[action_index].item_id;
        }
        if(skill.skill_cost > action_point)
            return;
        // reduce action point 
        action_point -= skill.skill_cost;

        // perform action
        SkillController.Controller().PerformEffect(skill, player_unit, enemy_unit[enemy_index]);
    }

    public void EnemyTurn()
    {
        frontend.pause = true;
        // show player action panel to allow player perform action
        frontend.EnemyTurn();
    }

    public void CauseDamage(BattleUnit from, BattleUnit to)
    {
        int damage = from.attack * ( 1 - ( to.defense / (to.defense + ( from.unit_level - to.unit_level )*maze_level + maze_level*maze_level*20 ) ) );
        to.health_curr -= damage;

        if(to.health_curr <= 0)
            to.OnDie();
    }

    public void ExitBattle()
    {
        MazeController.Controller().ExitRoom();
    }

    public void BattleEnd()
    {
        

        // end battle

        // 
    }

    public void Victory()
    {
        // get reward

        // end battle

        // complete room
    }

    public void Fail()
    {
        // end maze

    }
}