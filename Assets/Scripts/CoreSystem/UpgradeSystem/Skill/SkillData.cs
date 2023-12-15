using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillData{

    // basic info
    public string skill_id;
    public string skill_name;
    public string skill_describe; 
    public int skill_level_cap;         // the max level of skill
    public bool skill_active;

    // selection value
    public bool affect_ally;    // the targe to select (enemy or self)
    public bool is_melee;       // will this skill able to select back line enemy
    public int[] show_range;  // the range of this skill shows to player, top left to botton right, in colume , 0 to 14, 7 is center and this slot

    // cost value
    public int[] skill_cost;
    public int[] skill_cold;

    // effect type and value
    public List<SkillEffect> skill_effects;    // the list of skill actual function
    
    // front-end related
    public SkillCareer skill_career;    //
    public int skill_pos;               // the position of skill in skill learn panel
    public SkillData(){}

    public void SkillXmlWriter()
    {
        SkillData skill = new SkillData();
        skill.skill_id = "AcidSpit";
        skill.skill_name = "Acid Spit";
        skill.skill_describe = "Slime sprays acid at you.";
        skill.skill_level_cap = 1;
        skill.skill_active = true;

        skill.affect_ally = false;
        skill.is_melee = false;
        skill.show_range = new int[]{};

        skill.skill_cost = new int[]{};
        skill.skill_cold = new int[]{};

        SkillEffect effect = new SkillEffect();

        effect.weakness = Weakness.Null;
        effect.skill_value = new float[]{0.5f};
        effect.value_id = "Attack";
        effect.skill_effect = SkillEffectType.Damage;
        effect.effect_describe = "Causes {0} damage to player";
        effect.effect_range = new int[]{-1};

        skill.skill_effects = new List<SkillEffect>();
        skill.skill_effects.Add(effect);

        skill.skill_career = SkillCareer.Mob;
        skill.skill_pos = 0;

        XmlController.Controller().SaveData(skill, skill.skill_id);
    }

    // a inner class to create skill effects
    public struct SkillEffect
    {
        public Weakness weakness;               // the damage type if its a damage effect
        public float[] skill_value;            // the value of this effect for each level
        /* value format:
            Buff effect = buff relate value;
            Damage, heal or shield effect = modifier value;
        */
        public string value_id;
        /* value format:
            Buff effect = buff_id;
            Damage, heal or shield effect =  modifier type;
        */
        public SkillEffectType skill_effect;    // what this effect gonna do

        public string effect_describe;

        // affect target value
        public int[] effect_range;  // the range of this skill gonna affect, top left to botton right, in colume , 0 to 14 is enemy slot, 7 is center and this slot, -1 is player
    }
}

public enum SkillCareer
{
    Null,
    Mob,
    Fighter,
    Archer,
    Mage,
    Priest
}

public enum SkillEffectType
{
    Null,
    Damage,
    Heal,
    Buff,
}