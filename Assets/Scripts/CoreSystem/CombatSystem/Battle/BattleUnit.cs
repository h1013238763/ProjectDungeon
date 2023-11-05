using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour {
    public string unit_id;
    public string unit_name;

    public int unit_level;
    public int health_max;
    public int health_curr;

    public int attack;
    public int defense;

    public int tough_max;
    public int tough_curr;
    public bool in_stun;

    public List<Effect> unit_effects;

    public void OnBorn()
    {
        
    }

    public void OnMelee(BattleUnit target)
    {

    }

    public void OnRange()
    {

    }

    public void OnPerform()
    {

    }

    public void OnHit()
    {

    }

    public void OnDie()
    {

    }

    public void CountEffect()
    {
        foreach(Effect effect in unit_effects)
        {
            effect.effect_action.Invoke();
        }
    }
}
