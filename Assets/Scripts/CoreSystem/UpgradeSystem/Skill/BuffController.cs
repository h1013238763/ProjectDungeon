using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BuffController : BaseController<BuffController>
{

    private Dictionary<string, Sprite> image_buff = new Dictionary<string, Sprite>();

    public Sprite GetImage(string id)
    {
        if(!image_buff.ContainsKey(id))
            return null;
        return image_buff[id];
    }

    public void AddBuff(BuffAttribute attribute, int value, int time, BattleUnit unit)
    {
        Buff buff = new Buff();
        buff.buff_value = value;
        buff.buff_attribute = attribute;
        buff.buff_time_max = time;
        buff.buff_time_curr = time;

        switch(attribute)
        {
            case BuffAttribute.Attack:
                if(value > 0)
                {
                    buff.buff_id = "AttackUp";
                    buff.is_buff = true;
                }
                else
                {
                    buff.buff_id = "AttackDown";
                    buff.is_buff = false;
                }

                if(unit.extra_attack.ContainsKey(buff.buff_id))
                {
                    unit.extra_attack[buff.buff_id] += value;
                }
                else
                {
                    unit.extra_attack.Add(buff.buff_id, value);
                }
                break;
            case BuffAttribute.Defense:
                if(value > 0)
                {
                    buff.buff_id = "DefenseUp";
                    buff.is_buff = true;
                }
                else
                {
                    buff.buff_id = "DefenseDown";
                    buff.is_buff = false;
                }

                if(unit.extra_attack.ContainsKey(buff.buff_id))
                {
                    unit.extra_defense[buff.buff_id] += value;
                }
                else
                {
                    unit.extra_defense.Add(buff.buff_id, value);
                }
                break;
            case BuffAttribute.Poison:
                buff.buff_id = "Poison";
                buff.is_buff = false;
                break;
            default:
                break;
        }

        unit.unit_buffs.Add(buff);
        unit.unit_panel.SetBuff();
    }

    public void EffectiveBuff(Buff buff, BattleUnit unit)
    {
        switch(buff.buff_attribute)
        {
            case BuffAttribute.Poison:
                unit.OnHit(buff.buff_value/100, Weakness.Poison);
                buff.buff_time_curr --;
                if(buff.buff_time_curr <= 0)
                {
                    RemoveBuff(buff, unit);
                }
                break;
            default:
                buff.buff_time_curr --;
                if(buff.buff_time_curr <= 0)
                {
                    RemoveBuff(buff, unit);
                }
                break;
        }
        unit.unit_panel.SetBuff();
    }

    public void RemoveBuff(Buff buff, BattleUnit unit)
    {
        switch(buff.buff_attribute)
        {
            case BuffAttribute.Attack:
                unit.extra_attack[buff.buff_id] -= buff.buff_value;
                break;
            case BuffAttribute.Defense:
                unit.extra_defense[buff.buff_id] -= buff.buff_value;
                break;
            default:
                break;
        }
        unit.unit_buffs.Remove(buff);
        unit.unit_panel.SetBuff();
    }

    public void InitialData()
    {
        Sprite[] images = Resources.LoadAll<Sprite>("Image/Buff/");
        if(images != null)
        {
            foreach(Sprite image in images)
            {
                image_buff.Add(image.name, image);
            }
        }
    }
}