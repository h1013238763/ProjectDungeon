using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour {
    
    // id
    public string unit_id;      // the base id of unit
    public string unit_name;    // the name of unit
    public string unit_index;   // the index of unit, player = -1, enemy from 0 ~ 5
    public GameObject unit_object;
    public UnitPanel unit_panel;
    public bool is_player;

    // basic attributes
    public int unit_level;
    public int health_max;
    public int health_curr;

    public int attack;
    public List<int> extra_attack;

    public int defense;
    public List<int> extra_defense;

    // special attribute
    public int shield_max;
    public int shield_curr;
    public bool in_stun;
    public float STUN_DAMAGE_COE = 0.3f;

    public List<string> weakness;
    public float WEAK_DAMAGE_COE = 0.2f;

    public bool in_control;

    public List<Effect> unit_effects;

    public void OnBorn()
    {
        
    }

    // this unit's turn start
    public void OnActionStart()
    {
        // buffs invoke
        foreach(Effect effect in unit_effects)
        {
            
        }

    }

    // during this unit's turn 
    public void OnActionPerform()
    {
        if(in_stun)
            in_stun = false;

        // if player's turn
        if( !is_player )
        {

        }

        // if enemy's turn
    }

    // this unit's turn end
    public void OnActionEnd()
    {

    }

    // Attack other unit, perform action animation
    public int OnAttack()
    {
        
    }

    // receive damage
    public void OnHit(int value, string hit_type)
    {
        // deal tough damage first
        int weak_coe = 1;
        if(weakness.Contains(hit_type))
            weak_coe += WEAK_DAMAGE_COE;
        shield_curr -= value * weak_coe;

        health_curr -= value * ( 1+WEAK_DAMAGE_COE );

        if(health_curr < 0)
            health_curr = 0;

        unit_panel.ChangeHealth(health_curr);

        if(health_curr == 0)
            OnDie();
    }

    // receive heal
    public void OnHeal(int value)
    {
        // set health
        health_curr = ( health_curr + value > health_max) ? health_max : health_curr + value;
        // trigger heal panel anime

        // trigger event
        EventController.Controller().EventTrigger<int>(unit_index+"Heal", value);
    }

    // unit die
    public void OnDie()
    {

    }

    public int GetAttack()
    {
        int result = attack;
        foreach(int num in extra_attack)
            result += num;
        return result;
    }

    public int GetDefense()
    {
        int result = defense;
        foreach(int num in extra_defense)
            result += num;
        return result;
    }

    public void CountEffect()
    {
        foreach(Effect effect in unit_effects)
        {
            effect.effect_action.Invoke();
        }
    }

    public void ClearEffect(bool clear_buff, int num)
    {

    }
}
