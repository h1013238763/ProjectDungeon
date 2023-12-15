using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class BattlePanel : PanelBase
{
    public BattleController battle_control;
    public float time_curr;             // current time count
    private bool pause;                  // stop time count
    public bool player_turn;            // is player's turn
    private int action_point_index;     // 
    private int curr_action_point;

    private float back_anime_time = 0.3f;
    private float point_anime_time = 0.2f;
    private float text_anime_time = 0.15f;

    public int click_stage;
    public bool count_sound;

    private void FixedUpdate()
    {
        if(!pause)
        {
            if(time_curr > -1f)
                time_curr -= Time.deltaTime;

            if(time_curr <= 0)
            {
                // if time up, end player's turn
                if(player_turn)
                {
                    Debug.Log(battle_control.in_anime_list.Count);
                    if(battle_control.in_anime_list.Count == 0)
                    {
                        EnemyTurn();
                        battle_control.player_unit.ActionEnd();
                        time_curr = -1f;
                        pause = true;
                    }
                    else
                    {
                        time_curr = 0.2f;
                    }
                }
            }
            if(time_curr <= 5 && !count_sound)
            {
                count_sound = true;
                AudioController.Controller().StartSound("CountDown");
            }
            else
            {
                SetTimeText();
            }
        }
    }

    public override void ShowSelf()
    {
        // initial variable
        this.gameObject.SetActive(true);
        pause = true;
        battle_control = BattleController.Controller();
        curr_action_point = 0;
        action_point_index = 0;
        // disable button
        FindComponent<Button>("TurnTime").interactable = false;
        FindComponent<Text>("TurnText").GetComponent<CanvasGroup>().alpha = 0;

        // reset icons
        ResetActionIcon();
        // reset infos
        ResetSkillInfo();
        ResetPotionInfo();

        ActiveButtons(false);

        // show panel anime
        TweenController.Controller().MoveToPosition(FindComponent<Image>("TurnTime").transform, new Vector3(0, 488, 0), back_anime_time, true, TweenType.Smooth);
        TweenAction action = 
        TweenController.Controller().MoveToPosition(FindComponent<Image>("ActionGrid").transform, new Vector3(0, -377, 0), back_anime_time, true, TweenType.Smooth);
        TweenController.Controller().AddEventTrigger(action, "BattleShowAnimeFinish");

        // action after panel show anime finish
        EventController.Controller().AddEventListener("BattleShowAnimeFinish", () => {
            AudioController.Controller().StartSound("BattleStart");
            ShowTurnText("Battle Start");
        });
    }

    public override void HideSelf()
    {
        pause = true;
        time_curr = -1;
        TweenController.Controller().MoveToPosition(FindComponent<Image>("TurnTime").transform, new Vector3(0, 600, 0), back_anime_time, true, TweenType.Smooth);
        TweenController.Controller().MoveToPosition(FindComponent<Image>("ActionGrid").transform, new Vector3(0, -719, 0), back_anime_time, true, TweenType.Smooth);
        GUIController.Controller().RemovePanel("BattlePanel");
    }

    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("Equip");
        // in planning action
        if(battle_control.player_action != null)
        {   // cancel action
            battle_control.player_action = null;
            ActiveButtons(true);

            battle_control.DisplayRange("Reset");

            return;
        }
        // skip turn
        if(button_name == "TurnTime")
        {
            AudioController.Controller().StopSound();
            count_sound = true;
            time_curr = 0.5f;
        }
        // click once to plan action
            // disable other buttons except this
            // reclick this button to cancel action
        // select a skill, assign this skill to player action
        else if(button_name.Contains("SkillSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            battle_control.player_action = "Skill:"+battle_control.player_build.skills[slot];

            battle_control.DisplayRange("Select");

            ActiveButtons(false);
            FindComponent<Button>(button_name).interactable = true;
        }
        // select a item, assign this item to player action
        else if(button_name.Contains("ItemSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            battle_control.player_action = "Potion:"+battle_control.player_build.potions[slot];

            battle_control.DisplayRange("Select");

            ActiveButtons(false);
            FindComponent<Button>(button_name).interactable = true;
        }
    }

    // panel slot reset
    // reset slot icon include player skills and potions
    public void ResetActionIcon()
    {
        PlayerBuild player_build = battle_control.player_build;
        
        // reset skills
        battle_control.skill_cost = new List<int>();
        for(int i = 0; i < 8; i ++)
        {
            FindComponent<Image>("SkillSlot ("+i+")").transform.GetChild(0).gameObject.SetActive(false);
            if( i < player_build.skills.Count)
            {
                KeyValuePair<SkillData, int> skill_info = SkillController.Controller().GetSkillInfo(player_build.skills[i]);
                FindComponent<Image>("SkillSlot ("+i+")").sprite = SkillController.Controller().GetImage(skill_info.Key.skill_id);   // icon
                FindComponent<Image>("SkillSlot ("+i+")").transform.GetChild(2).GetComponent<Text>().text = skill_info.Key.skill_cost[skill_info.Value].ToString();    // cost
                battle_control.skill_cold[i] = skill_info.Key.skill_cold[skill_info.Value];
                battle_control.skill_cost.Add(skill_info.Key.skill_cost[skill_info.Value]);
            }
            else
            {
                FindComponent<Image>("SkillSlot ("+i+")").sprite = null;
                FindComponent<Image>("SkillSlot ("+i+")").transform.GetChild(2).GetComponent<Text>().text = "";
            }
        }
        // potions
        battle_control.potion_cost = new List<int>();
        for(int i = 0; i < 8; i ++)
        {
            Transform slot = FindComponent<Button>("ItemSlot ("+i+")").transform;

            if( i < player_build.potions.Count)
            {
                slot.GetChild(0).gameObject.SetActive(true);
                slot.GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(player_build.potions[i]);     // icon
                slot.GetChild(1).GetComponent<Text>().text = ItemController.Controller().DictPotionInfo(player_build.potions[i]).potion_cost.ToString();    // cost
                battle_control.potion_cost.Add(ItemController.Controller().DictPotionInfo(player_build.potions[i]).potion_cost);
            }
            else
            {
                slot.GetChild(0).gameObject.SetActive(false);
                slot.GetChild(1).GetComponent<Text>().text = "";
                slot.GetChild(2).GetComponent<Text>().text = "";
            }
        }
    }
    // reset slot relate info skill colddown
    public void ResetSkillInfo()
    {
        PlayerBuild player_build = battle_control.player_build;

        // reset skills
        for(int i = 0; i < player_build.skills.Count; i ++)
        {
            Transform slot = FindComponent<Button>("SkillSlot ("+i+")").transform;
            KeyValuePair<SkillData, int> skill_info = SkillController.Controller().GetSkillInfo(player_build.skills[i]);

            // set cold time
            slot.GetChild(0).gameObject.SetActive(battle_control.cold_remain[i] > 0);
            slot.GetChild(0).GetComponent<Text>().text = battle_control.cold_remain[i].ToString();                                          // set cold text

            // set skill interactable
            FindComponent<Button>("SkillSlot ("+i+")").interactable = (battle_control.cold_remain[i] <= 0);
        }
    }

    // reset slot relate info potion limit
    public void ResetPotionInfo()
    {
        PlayerBuild player_build = battle_control.player_build;
        List<int> item_remain = battle_control.item_remain;

        for(int i = 0; i < player_build.potions.Count; i ++)
        {
            Transform slot = FindComponent<Button>("ItemSlot ("+i+")").transform;
            slot.GetChild(2).GetComponent<Text>().text = item_remain[i].ToString();
        }
    }

    public void ActiveButtons(bool act)
    {
        // Set Turn Time Btn
        FindComponent<Button>("TurnTime").interactable = act;

        // Set Skill Btn
        for(int i = 0; i < battle_control.player_build.skills.Count; i ++)
        {
            FindComponent<Button>("SkillSlot ("+i+")").interactable = act;
            if(curr_action_point < battle_control.skill_cost[i])
                FindComponent<Button>("SkillSlot ("+i+")").interactable = false;
            if(battle_control.cold_remain[i] > 0)
                FindComponent<Button>("SkillSlot ("+i+")").interactable = false;
        }
            
        // Set Potion Btn
        for(int i = 0; i < battle_control.player_build.potions.Count; i ++)
        {
            FindComponent<Button>("ItemSlot ("+i+")").interactable = act;
            if(curr_action_point < battle_control.potion_cost[i])
                FindComponent<Button>("ItemSlot ("+i+")").interactable = false;
            if(battle_control.item_remain[i] <= 0)
                FindComponent<Button>("SkillSlot ("+i+")").interactable = false;
        }
    }
    
    // Time Setting
    public void SetTime(float time_max)
    {
        time_curr = time_max;
    }
    public void SetTimeText()
    {
        FindComponent<Text>("TimeText").text = ((int)time_curr).ToString();
    }


    // Turn Text Setting
    public void ShowTurnText(string text)
    {
        // initial text
        FindComponent<Text>("TurnText").text = text;
        Transform target = FindComponent<Text>("TurnText").transform;

        TweenAction action;

        // fade in
        action = TweenController.Controller().ChangeAlpha(target, 1, text_anime_time);
        TweenController.Controller().AddEventTrigger(action, "TurnFadeIn:"+text);

        // fade out after fade in
        EventController.Controller().AddEventListener("TurnFadeIn:"+text, async () => {
            await Task.Delay(500);
            action = TweenController.Controller().ChangeAlpha(target, 0, text_anime_time);
            TweenController.Controller().AddEventTrigger(action, "TurnFadeOut:"+text);
        });
    }


    // Action Point Setting
    public void SetActionPoint(int num)
    {
        curr_action_point = num;
        ActionPointAnime();
    }
    public void ActionPointAnime()
    {
        Transform target = null;
        TweenAction action = null;

        // get point
        if(curr_action_point > action_point_index)
        {
            action_point_index ++;
            target = FindComponent<Image>("Point ("+action_point_index+")").transform;

            // point animation
            action = TweenController.Controller().ChangeSizeTo(target, new Vector3(1f, 1f, 0), point_anime_time, TweenType.Smooth);
        }
        // loss point
        else if(curr_action_point < action_point_index)
        {
            target = FindComponent<Image>("Point ("+action_point_index+")").transform;

            // point animation
            action = TweenController.Controller().ChangeSizeTo(target, new Vector3(0f, 0f, 0), point_anime_time, TweenType.Smooth);

            action_point_index --;
        }
        // if need to perform animation
        if(target != null)
        {
            TweenController.Controller().AddEventTrigger(action, "PointAnimeFinish:"+action_point_index);

            EventController.Controller().AddEventListener("PointAnimeFinish:"+action_point_index, () => {
                ActionPointAnime();
            });
        }
    }
    
    // turn switch
    public void PlayerTurn( float time, int action_points, int turn_count)
    {
        // reset time and action point
        SetTime(time);
        SetActionPoint(action_points);
        FindComponent<Text>("TurnCount").text = "Round "+turn_count.ToString();

        player_turn = true;
        count_sound = false;

        // set action cover
        Transform target = FindComponent<Image>("ActionCover").transform;
        TweenController.Controller().MoveToPosition(target, new Vector3(0, -350, 0), back_anime_time, true, TweenType.Smooth);
    }
    public void EnemyTurn()
    {
        player_turn = false;
        // stop timing
        pause = true;
        // set turn cover
        FindComponent<Text>("TimeText").text = "Enemy";
        
        // disable button
        FindComponent<Button>("TurnTime").interactable = false;

        Transform target = FindComponent<Image>("ActionCover").transform;
        TweenAction action = TweenController.Controller().MoveToPosition(target, new Vector3(0, 0, 0), back_anime_time, true, TweenType.Smooth);
        // trigger event after 
        TweenController.Controller().AddEventTrigger(action, "EnemyTurnAnimeFinish");
    }

    public void PauseTime(bool is_pause)
    {
        pause = is_pause;

        ActiveButtons(!pause);
    }
}
