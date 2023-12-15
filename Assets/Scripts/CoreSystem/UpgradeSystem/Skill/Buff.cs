using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Buff{
    
    public string buff_id;            // the id of effect

    public bool is_buff;
    public int buff_value;
    public BuffAttribute buff_attribute;
    public EffectiveTime buff_act_time;        // effective every turn, effective when get this

    public int buff_time_max;
    public int buff_time_curr;

    public Buff(){}
}

public enum BuffAttribute
{
    Attack,
    Defense,
    Poison
}

public enum EffectiveTime
{
    Start,
    Every
}