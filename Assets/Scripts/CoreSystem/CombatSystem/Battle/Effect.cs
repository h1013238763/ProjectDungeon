using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect
{
    public string effect_id;            // the id of effect
    public string effect_name;          // the name of effect
    public string effect_describe;      // the describe of effect
    public int effect_stack;            // the stacks of effect
    public int effect_stack_max;        // the celling of effect stacks
    public bool is_buff;                // is this effect is a buff
    public int effect_turn_remain;      // the remain turn time of buff

    public UnityAction effect_action;   // the actual function to perform
}
