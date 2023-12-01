using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : BaseController<BattleController>
{
    // Maze info
    public MazeController maze_control;

    // difficulty modifier
    public int damage_modifier;     // the modifier to manager the damage curve, = maze level * 10
    public int maze_level;

    // player only battle variables
    public int action_point;
    public int action_point_max = 10;
    public int action_point_recover = 2;
    public List<int> item_remain;
    public List<int> skill_cold;
    public List<int> cold_remain;
    public string player_action;       // current action assign to player
    public List<int> skill_cost;
    public List<int> potion_cost;

    // turn variables
    public int turn_count;
    public float turn_time = 16f;
    public int curr_unit_index;

    public PlayerBuild player_build;
    public BattlePanel panel;

    // Units
    public BattleUnit player_unit;
    public BattleUnit[] enemy_unit;
    public int selecting_unit;          // the unit selected by player

    // reward
    public int enemy_level;
    public int[] enemy_num;

    public List<int> in_anime_list;

    // Battle Flow
    public void BattleStart(Room room, Maze maze_base, int level)
    {
        // Initial Battle variables
        maze_control = MazeController.Controller();
        // change stage
        StageController.Controller().stage = Stage.Battle;
        maze_level = maze_control.maze_base.maze_level;
        enemy_level = level;
        cold_remain = new List<int>(new int[10]);

        in_anime_list = new List<int>();

        if(player_build == null)
            player_build = PlayerController.Controller().GetCurrBuild();

        

        // generate & store enemy
        if(room.room_enemy == null)
        {
            room.room_enemy = maze_base.GetRandomEnemy(room.room_type);
        }
        CreateEnemy(room.room_enemy, level);

        // assign battle unit
        player_unit = maze_control.player_unit;

        // reset action point and turn count
        turn_count = 0;
        action_point = 0;

        // create usage record
        if(item_remain == null)
        {
            item_remain = new List<int>(new int[8]);
            for(int i = 0; i < player_build.potions.Count; i ++)
            {
                int num_max = ItemController.Controller().DictPotionInfo(player_build.potions[i]).potion_max;
                int num_have = ItemController.Controller().InventPotionInfo(player_build.potions[i]).item_num;

                item_remain[i] = (num_max < num_have) ? num_max : num_have;
            }
        }   
        skill_cold = new List<int>(new int[8]);

        // show GUI
        GUIController.Controller().ShowPanel<BattlePanel>("BattlePanel", 2, (p) => 
        {
            panel = p;
        });

        // start battle after panel finish initial
        EventController.Controller().AddEventListener("TurnFadeOut:Battle Start", () => {
            NextTurn();
        });
    }

    public void NextTurn()
    {
        // turn increase
        turn_count ++;

        action_point += action_point_recover;
        if(action_point > action_point_max)
            action_point = action_point_max;

        curr_unit_index = -1;

        panel.SetTime(turn_time);

        panel.ShowTurnText("Player Turn");
        panel.PlayerTurn(turn_time, action_point, turn_count);

        // set skill colddown remain
        for(int i = 0; i < cold_remain.Count; i ++)
        {
            cold_remain[i] -= (cold_remain[i] > 0) ? 1 : 0;
        }
        panel.ResetSkillInfo();

        EventController.Controller().AddEventListener("TurnFadeOut:Player Turn", () => {
            // Turn Start
            DisplayRange("Reset");
            panel.PauseTime(false);
            player_unit.ActionStart();
        });
    }

    public void UnitActionEnd(int index)
    {
        if(index == -1)
        {
            panel.EnemyTurn();
        }
            
        // start next unit action
        for(int i = index+1; i < 6; i ++)
        {
            if(enemy_unit[i] != null)
            {
                curr_unit_index = i;
                enemy_unit[i].ActionStart();
                return;
            }
        }
        // all units have finished action
        NextTurn();
    }

    // end the battle
    public void BattleEnd(string reason)
    {
        if(reason == "Victory")
        {
            int exp = enemy_num[0]*(enemy_level*enemy_level+5) + enemy_num[1]*(enemy_level*enemy_level*4+5) + enemy_num[2]*enemy_level*enemy_level*15;
            int money = enemy_num[0]*enemy_level*1 + enemy_num[1]*enemy_level*4 + enemy_num[2]*enemy_level*15;
            List<Item> items = new List<Item>();

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < enemy_num[i]; j++)
                {
                    items.Add(maze_control.maze_base.GetRandomDrop(j));
                }
            }

            maze_control.reward_exp += exp;
            maze_control.reward_money += money;
            foreach(Item item in items)
                maze_control.reward_item.Add(item);
            
            // clear data and hide panel
            GUIController.Controller().HidePanel("BattlePanel");

            maze_control.CompleteRoom();
        }
        else if(reason == "Fail")
        {
            maze_control.ExitMaze("Fail");
        }
        else if(reason == "Exit")
        {
            maze_control.ExitRoom();
        }
    }

    // create enemies
    public void CreateEnemy(EnemyGroup group, int level)
    {
        enemy_unit = new BattleUnit[6];
        enemy_num = new int[3];
        // loop through enemy slots to create
        for(int i = 0; i < 6; i ++)
        {
            if(group.enemies[i] == null || group.enemies[i] == "")
            {
                continue;
            }  

            EnemyBase enemy = EnemyController.Controller().EnemyInfo(group.enemies[i]);
            BattleUnit unit = new BattleUnit();

            unit.unit_id = enemy.enemy_id;      // the base id of unit
            unit.unit_name = enemy.enemy_name;

            unit.unit_index = i;

            // basic attributes
            unit.unit_level = level;
            if(enemy.enemy_tier == 3)
            {
                unit.unit_level = maze_level * 10;
                AudioController.Controller().StartSound("Mob/"+enemy.enemy_id);
                AudioController.Controller().StartMusic("TutorialBoss");
            }

            unit.health_max = enemy.GetHealth(level);
            unit.health_curr = unit.health_max;

            unit.attack = enemy.GetAttack(level);
            unit.extra_attack = new List<int>();

            unit.defense = enemy.GetDefense(level);
            unit.extra_defense = new List<int>();
    
            // special attribute
            unit.tough_max = enemy.GetTough(level);
            unit.tough_curr = unit.tough_max;

            unit.in_stun = false;

            unit.weakness = enemy.enemy_weakness;
            unit.in_control  = false;

            unit.unit_effects = new List<Buff>();

            enemy_num[enemy.enemy_tier-1] ++;

            // show enemy object
            GUIController.Controller().ShowPanel<UnitPanel>("UnitPanel ("+i+")", 1, (p) => {
                p.unit = unit;
                unit.unit_panel = p; 
                p.ChangeAction(enemy.enemy_action[0]);
            });

            enemy_unit[i] = unit;
        }
    }


    // display the action range once player assign an action
    public void DisplayRange(string type)
    {
        List<int> range = null;
        // reset all unit range effect
        if(type == "Reset")
        {
            player_unit.unit_panel.ChangeButtonColor("Normal");
            for(int i = 0; i < 6; i ++)
            {
                if(enemy_unit[i] != null)
                    enemy_unit[i].unit_panel.ChangeButtonColor("Normal");
            }
        }
        // disable all unit could not be select by player action
        else if(type == "Select")
        {
            if(player_action == null)
                return;

            string action_id = player_action.Substring(player_action.IndexOf(":")+1);

            bool no_front = true;
            for(int i = 0; i < 3; i ++)
                if(enemy_unit[i] != null)
                    no_front = false;

            if(player_action.Contains("Skill:"))
            {
                SkillData skill_info = SkillController.Controller().GetSkill(action_id);
                range = new List<int>(skill_info.show_range);

                // if this skill only affect player                
                if( skill_info.affect_ally )
                {
                    player_unit.unit_panel.ChangeButtonColor("Normal");

                    for(int i = 0; i < 6; i ++)
                    {
                        if(enemy_unit[i] != null)
                            enemy_unit[i].unit_panel.ChangeButtonColor("Disable");
                    }
                }
                // if this skill is range attack
                else if( !skill_info.is_melee )
                {
                    player_unit.unit_panel.ChangeButtonColor("Disable");

                    for(int i = 0; i < 6; i ++)
                    {
                        if(enemy_unit[i] != null)
                            enemy_unit[i].unit_panel.ChangeButtonColor("Normal");
                    }
                }
                // if all front line enemy died
                else if( no_front )
                {
                    player_unit.unit_panel.ChangeButtonColor("Disable");

                    for(int i = 0; i < 6; i ++)
                    {
                        if(enemy_unit[i] != null)
                            enemy_unit[i].unit_panel.ChangeButtonColor("Normal");
                    }
                }
                // else
                else
                {
                    player_unit.unit_panel.ChangeButtonColor("Disable");

                    for(int i = 0; i < 6; i ++)
                    {
                        if(i < 3)
                        {
                            if(enemy_unit[i] != null)
                                enemy_unit[i].unit_panel.ChangeButtonColor("Normal");
                        }
                        else
                        {
                            if(enemy_unit[i] != null)
                                enemy_unit[i].unit_panel.ChangeButtonColor("Disable");
                        }  
                    }
                }
            }

            else if(player_action.Contains("Potion:"))
            {

            }
        }
        // highlight all unit will affect by this action
        else if(type == "Effect")
        {
            string action_id = player_action.Substring(player_action.IndexOf(":")+1);
            SkillData skill_info = SkillController.Controller().GetSkill(action_id);
            range = new List<int>(skill_info.show_range);

            // selecting player
            if(selecting_unit == -1)
            {
                player_unit.unit_panel.ChangeButtonColor("Highlight");
            }
            // assign the center of range to select unit's position
            else
            {
                int unit_pos = 7 - selecting_unit;
                List<int> enemies = ExistEnemy();

                for(int i = 0; i < enemies.Count; i ++)
                {
                    // if enemy in range
                    if( range.Contains(enemies[i]+unit_pos) )
                    {
                        enemy_unit[enemies[i]].unit_panel.ChangeButtonColor("Highlight");
                    }
                }
            }
        }
    }

    // perform current player action
    public void PerformAction(int unit_index, int target_index)
    {
        // after perform action, trigger unit action end
        string skill_id = null;
        // player action
        if(unit_index == -1)
        {
            player_unit.unit_panel.ActionAnime("Perform");
            // if assign action is skill
            if(player_action.Contains("Skill:"))
            {
                skill_id = player_action.Substring(player_action.IndexOf(":")+1);

                int action_index = PlayerController.Controller().GetCurrBuild().skills.IndexOf(skill_id);
                cold_remain[action_index] = skill_cold[action_index];
                action_point -= skill_cost[action_index];
                panel.ResetSkillInfo();
            }
            // if assign action is item
            else if(player_action.Contains("Potion:"))
            {
                panel.ResetPotionInfo();
            }
        }
        // enemy action
        else
        {
            // get enemy action flow
            EnemyBase enemy = EnemyController.Controller().EnemyInfo(enemy_unit[unit_index].unit_id);
            skill_id = SkillController.Controller().GetSkill(enemy.enemy_action[(turn_count-1)%enemy.enemy_action.Count]).skill_id;
        }
        //  trigger action
        if( skill_id != null )
            SkillController.Controller().UseSkill( skill_id, unit_index, target_index, ExistEnemy() );

        panel.SetActionPoint(action_point);
        player_action = null;
        DisplayRange("Reset");
        panel.ActiveButtons(true);
    }

    // get the list of exist enemy
    public List<int> ExistEnemy()
    {
        List<int> result = new List<int>();
        for(int i = 0; i < 6; i ++)
        {
            if(enemy_unit[i] != null)
                result.Add(i);
        }

        return result;
    }

    // get target enemy's skill by turn count
    public string GetEnemySkill(int enemy_index)
    {
        EnemyBase enemy = EnemyController.Controller().EnemyInfo(enemy_unit[enemy_index].unit_id);
        return SkillController.Controller().GetSkill( enemy.enemy_action[(turn_count)%enemy.enemy_action.Count] ).skill_id;
    }

    // check units' anime, trigger action after anime finish
    public void UnitAnimeList(int anime_unit, bool add )
    {
        if(add)
        {
            in_anime_list.Add(anime_unit);
        }
        else
        {
            in_anime_list.Remove(anime_unit);
            if(in_anime_list.Count == 0)
            {
                if(curr_unit_index == -1)
                {
                    player_unit.ActionPerform();
                }
                else
                {
                    enemy_unit[curr_unit_index].ActionEnd();
                }
            }
        }
    }

    // cause damage to another unit
    public void CauseDamage(float value, string modifier_id, Weakness weakness, int from, int to)
    {

        BattleUnit attacker = (from == -1) ? player_unit : enemy_unit[from];
        BattleUnit defenser = (to == -1) ? player_unit : enemy_unit[to];

        float damage = 0;
        int defense = defenser.GetDefense();

        // damage modify by attack
        if(modifier_id == "Attack")
        {
            damage = value*attacker.GetAttack();
        }        

        float final_damage = damage * ( 1.0f -  defense / (defense + (attacker.unit_level-defenser.unit_level )*maze_level + 20f*maze_level*maze_level ));
        if(final_damage > 1000000)
            final_damage = 999999;

        defenser.OnHit((int)final_damage, weakness);
    }
}