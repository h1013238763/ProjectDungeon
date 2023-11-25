using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Effect", menuName = "ProjectDungeon/Effect", order = 0)]
public class Effect {
    
    public string effect_id;            // the id of effect
    public string effect_name;          // the name of effect
    public string effect_describe;      // the describe of effect
    public int effect_stack_max;        // the celling of effect stacks
    public bool is_buff;                // is this effect is a buff
    public int effect_turn;

    public UnityAction<BattleUnit> awake_effect;
    public UnityAction<BattleUnit> lasting_effect;
    public UnityAction<BattleUnit> remove_effect;

    public Effect(string id, string name, string describe, int stack_max, bool buff, int turn_max)
    {
        effect_id = id;
        effect_name = name;
        effect_describe = describe;
        effect_stack_max = stack_max;
        is_buff = buff;
        effect_turn = turn_max;
    }
}

