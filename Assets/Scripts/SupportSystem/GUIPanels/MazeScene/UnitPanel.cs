using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class UnitPanel : PanelBase
{
    public int health_max;
    public int health_curr;
    public int tough_max;
    public int tough_curr;

    public List<Buff> effects;

    public float health_bar_anime_time = 0.3f;
    public float time;

    public string skill;

    public BattleUnit unit;

    /* TODO : 
        1. disappear when kinda out of battle
        2. appear when mouse move in
        3. disappear when mouse move out and out of battle
        4. change frame depend on the enemy type
        5. higher panel depend on the enemy type
        6. hide tough bar if is player
    */

    public override void ShowSelf()
    {
        // control panel component active depend on unit
        // hide active and tough bar if is player
        FindComponent<Image>("ToughBar").gameObject.SetActive(unit.unit_id != "Player");
        FindComponent<Image>("ActionSlot").gameObject.SetActive(unit.unit_id != "Player");

        // assign values
        health_max = unit.health_max;
        health_curr = health_max;

        if(unit.unit_id != "Player")
        {
            tough_max = unit.tough_max;
            tough_curr = tough_max;
        }

        effects = unit.unit_effects;
        SetBuff();

        if(unit.unit_index != -1)
            FindComponent<Image>("Unit").sprite = EnemyController.Controller().GetImage(unit.unit_id);

        // unit button event
        Button btn = FindComponent<Button>("Unit");
        GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {    
            // set player action highlight effect
            if(BattleController.Controller().player_action != null)
            {
                BattleController.Controller().selecting_unit = unit.unit_index;
                BattleController.Controller().DisplayRange("Effect");
            }
        });
        GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => { 
            // set player action highlight effect
            BattleController.Controller().DisplayRange("Select");
        }); 

        // Enemy Action event
        Image img = FindComponent<Image>("ActionSlot");
        // in: show skill info
        GUIController.AddCustomEventListener(img, EventTriggerType.PointerEnter, (data) => {    
            GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, (p) =>
            {
                p.info_type = "Skill";
                p.info_skill = skill;
                p.mouse_pos = img.transform.position;
            });
        });
        GUIController.AddCustomEventListener(img, EventTriggerType.PointerExit,  (data) => { 
            GUIController.Controller().RemovePanel("InfoPanel");
        }); 
        // out: hide info
    }

    // click this unit to perform action
    protected override void OnButtonClick(string button_name)
    {
        if(BattleController.Controller().player_action != null)
        {
            BattleController.Controller().PerformAction(-1, unit.unit_index);
        }
    }

    public void ChangeHealth(int health)
    {
        Image bar_curr = FindComponent<Image>("Health");
        Image bar_anime = FindComponent<Image>("HealthAnime");
        Vector3 new_scale = new Vector3((1.0f * health / health_max), 1, 0);

        // heal
        if(health > health_curr)
        {
            bar_anime.transform.localScale = new_scale;
            TweenController.Controller().ChangeSizeTo(bar_curr.transform, new_scale, health_bar_anime_time, TweenType.Smooth);
        }
        // take damage
        else
        {
            bar_curr.transform.localScale = new_scale;
            TweenController.Controller().ChangeSizeTo(bar_anime.transform, new_scale, health_bar_anime_time, TweenType.Smooth);
        }

        HealthNumAnime(health - health_curr);

        health_curr = health;
    }
    public void HealthNumAnime(int num)
    {
        float damage_anime_time = 0.4f;

        // set Text
        FindComponent<Text>("DamageNum").text = num.ToString();
        Transform damage_text = FindComponent<Text>("DamageNum").transform;

        // show text
        damage_text.GetComponent<CanvasGroup>().alpha = 1;
        damage_text.localPosition = new Vector3(0, 350, 0);

        // perform animation
        TweenAction action = TweenController.Controller().MoveToPosition(damage_text, new Vector3(0, 380, 0), damage_anime_time, true, TweenType.Smooth);
        TweenController.Controller().AddEventTrigger(action, "DamageAnimeFinish:"+unit.unit_id);

        EventController.Controller().AddEventListener("DamageAnimeFinish:"+unit.unit_id, () => {
            TweenController.Controller().ChangeAlpha(damage_text, 0, damage_anime_time);
        });
    }

    public void ChangeTough(int tough)
    {
        Image bar_curr = FindComponent<Image>("Tough");
        Image bar_anime = FindComponent<Image>("ToughAnime");
        Vector3 new_scale = new Vector3((1.0f * tough / tough_max), 1, 0);

        // heal
        if(tough > tough_curr)
        {
            bar_anime.transform.localScale = new_scale;
            TweenController.Controller().ChangeSizeTo(bar_curr.transform, new_scale, health_bar_anime_time, TweenType.Smooth);
        }
        // take damage
        else
        {
            bar_curr.transform.localScale = new_scale;
            TweenController.Controller().ChangeSizeTo(bar_anime.transform, new_scale, health_bar_anime_time, TweenType.Smooth);
        }
        tough_curr = tough;
    }

    // action slot fade out and fade in, show enemy next action
    public void ChangeAction(string skill_id)
    {
        skill = skill_id;

        float action_time = 0.5f;
        
        Transform skill_slot = FindComponent<Image>("ActionSlot").transform;

        EventController.Controller().AddEventListener(skill_slot.gameObject.name+"TweenFinish", () => {
            // change image
            FindComponent<Image>("ActionSlot").sprite = SkillController.Controller().GetImage(skill_id);
            // fade in
            TweenController.Controller().ChangeAlpha(skill_slot, 1, action_time, TweenType.Smooth);
        });
        // fade out
        TweenAction action = TweenController.Controller().ChangeAlpha(skill_slot, 0, action_time, TweenType.Smooth);
        TweenController.Controller().AddEventTrigger(action, skill_slot.gameObject.name+"TweenFinish");
    }

    public async void ActionAnime(string type)
    {
        Transform unit_transform = FindComponent<Image>("Unit").transform;
        float action_anime_time = 0.2f;

        BattleController.Controller().UnitAnimeList(unit.unit_index, true);

        if(unit.unit_index == -1)
        {
            // if perform action, unit move forward, then back
            if(type == "Perform")
            {
                // register anime
                BattleController.Controller().panel.PauseTime(true);

                // move forward
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(30, 0, 0), action_anime_time, true);
                TweenController.Controller().AddEventTrigger(action, "PerformAnimeStart:"+unit.unit_index);
                // wait a while then move back
                EventController.Controller().AddEventListener("PerformAnimeStart:"+unit.unit_index, async () => {
                    await Task.Delay(700);
                    action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(0, 0, 0), action_anime_time, true);
                    TweenController.Controller().AddEventTrigger(action, "PerformAnimeFinish:"+unit.unit_index);
                });

                EventController.Controller().AddEventListener("PerformAnimeFinish:"+unit.unit_index, () => {
                    BattleController.Controller().UnitAnimeList(unit.unit_index, false);
                });
            }
            // if on hit, unit move backward, then front
            else if(type == "Hit")
            {
                // move backward
                await Task.Delay(300);
                unit_transform.localPosition = new Vector3(-10, 0, 0);
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(-20, 0, 0), action_anime_time, true, TweenType.Smooth);
                TweenController.Controller().AddEventTrigger(action, "HitAnimeStart:"+unit.unit_index);
                // then move forward
                EventController.Controller().AddEventListener("HitAnimeStart:"+unit.unit_index, () => {
                    action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(0, 0, 0), action_anime_time, true, TweenType.Smooth);
                    TweenController.Controller().AddEventTrigger(action, "HitAnimeFinish:"+unit.unit_index);
                });

                EventController.Controller().AddEventListener("HitAnimeFinish:"+unit.unit_index, () => {
                    BattleController.Controller().UnitAnimeList(unit.unit_index, false);
                });
            }
            // if die, unit become black, then fade out
            else if(type == "Die")
            {
                // perform on hit
                await Task.Delay(300);
                unit_transform.localPosition = new Vector3(-10, 0, 0);
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(-20, 0, 0), action_anime_time, true, TweenType.Smooth);
                TweenController.Controller().AddEventTrigger(action, "DieAnimeStart:"+unit.unit_index);
                // replace move back with turn black then fade out
                EventController.Controller().AddEventListener("DieAnimeStart:"+unit.unit_index, async () => {
                    TweenController.Controller().ChangeImageColor(unit_transform, new Vector3(0, 0, 0), action_anime_time, TweenType.Smooth);
                    await Task.Delay((int)(action_anime_time * 1000));
                    action = TweenController.Controller().ChangeAlpha(unit_transform, 0, action_anime_time, TweenType.Smooth);
                    TweenController.Controller().AddEventTrigger(action, "DieAnimeFinish:"+unit.unit_index);
                });

                EventController.Controller().AddEventListener("DieAnimeFinish:"+unit.unit_index, () => {
                    unit.OnDie();
                });
            }
        }
        // if enemy, anime is opposite
        else
        {
            // if perform action, unit move forward, then back
            if(type == "Perform")
            {
                // move forward
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(-30, 0, 0), action_anime_time, true);
                TweenController.Controller().AddEventTrigger(action, "PerformAnimeStart:"+unit.unit_index);
                // wait a while then move back
                EventController.Controller().AddEventListener("PerformAnimeStart:"+unit.unit_index, async () => {
                    await Task.Delay(700);
                    action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(0, 0, 0), action_anime_time, true);
                    TweenController.Controller().AddEventTrigger(action, "PerformAnimeFinish:"+unit.unit_index);
                });

                EventController.Controller().AddEventListener("PerformAnimeFinish:"+unit.unit_index, () => {
                    BattleController.Controller().UnitAnimeList(unit.unit_index, false);
                });

            }
            // if on hit, unit move backward, then front
            else if(type == "Hit")
            {
                // move backward
                await Task.Delay(300);
                unit_transform.localPosition = new Vector3(10, 0, 0);
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(20, 0, 0), action_anime_time, true, TweenType.Smooth);
                TweenController.Controller().AddEventTrigger(action, "HitAnimeStart:"+unit.unit_index);
                // then move forward
                EventController.Controller().AddEventListener("HitAnimeStart:"+unit.unit_index, () => {
                    action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(0, 0, 0), action_anime_time, true, TweenType.Smooth);
                    TweenController.Controller().AddEventTrigger(action, "HitAnimeFinish:"+unit.unit_index);
                });

                EventController.Controller().AddEventListener("HitAnimeFinish:"+unit.unit_index, () => {
                    BattleController.Controller().UnitAnimeList(unit.unit_index, false);
                });
            }
            // if die, unit become black, then fade out
            else if(type == "Die")
            {
                // perform on hit
                await Task.Delay(300);
                unit_transform.localPosition = new Vector3(10, 0, 0);
                TweenAction action = TweenController.Controller().MoveToPosition(unit_transform, new Vector3(20, 0, 0), action_anime_time, true, TweenType.Smooth);
                TweenController.Controller().AddEventTrigger(action, "DieAnimeStart:"+unit.unit_index);
                // replace move back with turn black then fade out
                EventController.Controller().AddEventListener("DieAnimeStart:"+unit.unit_index, async () => {
                    TweenController.Controller().ChangeImageColor(unit_transform, new Vector3(0, 0, 0), action_anime_time, TweenType.Smooth);
                    await Task.Delay((int)(action_anime_time * 1000));
                    action = TweenController.Controller().ChangeAlpha(unit_transform, 0, action_anime_time, TweenType.Smooth);
                    TweenController.Controller().AddEventTrigger(action, "DieAnimeFinish:"+unit.unit_index); 
                });
                
                EventController.Controller().AddEventListener("DieAnimeFinish:"+unit.unit_index, () => {
                    unit.OnDie();
                    BattleController.Controller().UnitAnimeList(unit.unit_index, false);
                });
            }
        }
    }

    public void SetBuff()
    {
        for(int i = 0; i < 8; i ++)
        {
            Image buff_image = FindComponent<Image>("Buff ("+i+")");
            buff_image.gameObject.SetActive( i < effects.Count);

            if( i < effects.Count)
            {
                buff_image.sprite = BuffController.Controller().GetImage(effects[i].buff_id); 
            }
        }
    }

    public void AddBuff()
    {
        
    }

    public void RemoveBuff()
    {

    }

    public void ChangeButtonColor(string type)
    {
        if(type == "Normal")
        {
            if( !unit.in_control )
            {
                FindComponent<Image>("Unit").raycastTarget = true;
                FindComponent<Image>("Unit").color = new Color(0.87f, 0.87f, 0.87f, 1);
            }
            else
            {
                FindComponent<Image>("Unit").raycastTarget = true;
                FindComponent<Image>("Unit").color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
        else if(type == "Disable")
        {
            FindComponent<Image>("Unit").raycastTarget = false;
            FindComponent<Image>("Unit").color = new Color(0.35f, 0.35f, 0.35f, 1);
        }
        else if(type == "Highlight")
        {
            FindComponent<Image>("Unit").raycastTarget = true;
            FindComponent<Image>("Unit").color = new Color(1f, 1f, 1f, 1);
        }
    }
}
