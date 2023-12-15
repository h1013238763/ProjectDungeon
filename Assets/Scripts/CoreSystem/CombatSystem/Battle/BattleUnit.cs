using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BattleUnit{
    
    // id
    public string unit_id;      // the base id of unit
    public string unit_name;    // the name of unit
    public int unit_index;   // the index of unit, player = -1, enemy from 0 ~ 5
    public UnitPanel unit_panel;

    // basic attributes
    public int unit_level;
    public int health_max;
    public int health_curr;

    public int attack;
    public Dictionary<string, int> extra_attack;

    public int defense;
    public Dictionary<string, int> extra_defense;
    
    // special attribute
    public int tough_max;
    public int tough_curr;
    public bool in_stun;
    public float STUN_DAMAGE_COE = 0.3f;

    public Weakness weakness;
    public float WEAK_DAMAGE_COE = 0.5f;

    public bool in_control;

    public List<Buff> unit_buffs;

    public BattleUnit()
    {
        extra_attack = new Dictionary<string, int>();
        extra_defense = new Dictionary<string, int>();
        unit_buffs = new List<Buff>();
    }

    public void OnBorn()
    {
        
    }
    
    // action flow

    // this unit's turn start
    public void ActionStart()
    {
        foreach(Buff buff in unit_buffs)
        {
            BuffController.Controller().EffectiveBuff(buff, this);
        }
        ActionPerform();
    }

    // during this unit's turn 
    public async void ActionPerform()
    {
        // player action start event
        if(unit_id == "Player")
        {
            if(health_curr <= 0)
            {
                OnDie();
            }
            if(BattleController.Controller().action_point > 0)
            {
                BattleController.Controller().panel.PauseTime(false);
            }
            else
            {
                ActionEnd();
            }   
        }
        else
        {
            if(in_control)
            {
                BattleController.Controller().UnitAnimeList(unit_index, true);
                await Task.Delay(700);

                if(in_stun == true)
                {
                    tough_curr = tough_max;
                    unit_panel.ChangeTough(tough_curr);
                    in_stun = false;
                }

                in_stun = false;
                in_control = false;
                // end stun anime
                unit_panel.ChangeButtonColor("Normal");
                BattleController.Controller().UnitAnimeList(unit_index, false);
            }
            // trigger action;
            else
            {
                // perform action animation
                unit_panel.ActionAnime("Perform");
                // after attack perform finish, end turn
                BattleController.Controller().PerformAction(unit_index, unit_index);
            }            
        }
    }

    // this unit's turn end
    public void ActionEnd()
    {
        if(unit_id == "Player")
        {
            BattleController.Controller().panel.time_curr = -1f;
            BattleController.Controller().panel.PauseTime(true);
            BattleController.Controller().panel.ActiveButtons(false);
            BattleController.Controller().player_action = null;
            BattleController.Controller().panel.ShowTurnText("Enemy Turn");
            EventController.Controller().AddEventListener("TurnFadeIn:Enemy Turn", () => {
                BattleController.Controller().UnitActionEnd(unit_index);
            });
        }
        else
        {
            // show next action
            unit_panel.ChangeAction(BattleController.Controller().GetEnemySkill(unit_index));
            BattleController.Controller().UnitActionEnd(unit_index);
        }
    }

    // actions

    // Attack other unit, perform action animation
    public void OnAttack()
    {
        
    }

    // receive damage
    public void OnHit(int damage, Weakness damage_weakness)
    {     
        // change curr tough, weakness gonna cause extra tough damage
        if(unit_index != -1)
        {
            if(weakness == damage_weakness)
                tough_curr -= (int)((1+WEAK_DAMAGE_COE) * damage);
            else
                tough_curr -= damage;

            // check if stun
            if(tough_curr <= 0)
            {
                in_stun = true;
                in_control = true;
                tough_curr = 0;
                // change unit stun animation
                unit_panel.ChangeButtonColor("Normal");
            }

            unit_panel.ChangeTough(tough_curr);
        }
        
        // change curr health, in stun gonna cause extra health damage
        if(in_stun)
            health_curr -= (int)((1+STUN_DAMAGE_COE) * damage);
        else
            health_curr -= damage;

        // check if die
        if(health_curr <= 0)
        {
            unit_panel.ActionAnime("Die");
        }
        // not die, trigger on hit event
        else
        {
            unit_panel.ActionAnime("Hit");
        }
        // play animation
        unit_panel.ChangeHealth(health_curr);
    }

    // receive heal
    public void OnHeal(int num)
    {
        health_curr += num;
        if(health_curr > health_max)
            health_curr = health_max;
        
        unit_panel.ChangeHealth(health_curr);
    }

    // unit die
    public void OnDie()
    {
        // trigger on die event

        // if player die
        if(unit_index == -1)
        {
            BattleController.Controller().BattleEnd("Fail");
        }
        else
        {
            // remove enemy
            BattleController.Controller().enemy_unit[unit_index] = null;
            GUIController.Controller().RemovePanel("UnitPanel ("+unit_index+")");
            // if no enemy left
            if(BattleController.Controller().ExistEnemy().Count == 0)
            {
                BattleController.Controller().BattleEnd("Victory");
            }
        }
    }

    public int GetAttack()
    {
        int result = attack;
        foreach(var pair in extra_attack)
            result += pair.Value;
        return result;
    }

    public int GetDefense()
    {
        int result = defense;
        foreach(var pair in extra_defense)
            result += pair.Value;
        return result;
    }

    // clear buff, debuff, all
    public void ClearEffect(string clear_type)
    {
        List<Buff> remove_list = new List<Buff>();
        foreach(Buff buff in unit_buffs)
        {
            if(clear_type == "buff" && buff.is_buff)
            {
                remove_list.Add(buff);
            }
            else if(clear_type == "debuff" && !buff.is_buff)
            {
                remove_list.Add(buff);
            }
            else if(clear_type == "all")
            {
                remove_list.Add(buff);
            }
        }

        foreach(Buff buff in remove_list)
        {
            BuffController.Controller().RemoveBuff(buff, this);
            
        }
        
    }
}
