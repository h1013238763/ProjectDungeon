using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : PanelBase
{
    public float time_curr;
    public bool pause;
    public bool player_turn;
    private int action_point_index;
    private int curr_action_point;

    private float back_anime_time = 0.3f;
    private float point_anime_time = 0.4f;
    private float text_anime_time = 0.6f;

    private int skill_slot_num;
    public int potion_slot_num;

    public PlayerData player;

    public int click_stage;

    private void FixedUpdate()
    {
        if(!pause)
        {
            if(time_curr > -0.2f)
                time_curr -= Time.deltaTime;
            if(time_curr <= 0)
            {
                time_curr = -0.5f;
                BattleController.Controller().ChangeTurn();
            }
            else
            {
                SetTimeText();
            }
        }
    }

    public override void ShowSelf()
    {
        ResetPanel();
        EventController.Controller().AddEventListener("BattleAnimationEnd", EndAction);
        player = PlayerController.Controller().data;

        SetSkill();
        SetItem();
    }

    public override void HideSelf()
    {
        // TweenController.Controller().ChangeSizeTo(mask, new Vector3(1, 1, 0), 0.5f);
        TweenController.Controller().MoveToPosition(FindComponent<Image>("TurnTime").transform, new Vector3(0, 600, 0), back_anime_time, true, TweenType.Smooth);
        TweenController.Controller().MoveToPosition(FindComponent<Image>("ActionGrid").transform, new Vector3(0, -719, 0), back_anime_time, true, TweenType.Smooth);
    }

    public void ResetPanel()
    {
        TweenController.Controller().MoveToPosition(FindComponent<Image>("TurnTime").transform, new Vector3(0, 488, 0), back_anime_time, true, TweenType.Smooth);
        TweenController.Controller().MoveToPosition(FindComponent<Image>("ActionGrid").transform, new Vector3(0, -377, 0), back_anime_time, true, TweenType.Smooth);

        EnemyTurn();

        // Print Battle Start Text
        FindComponent<Text>("TurnCount").text = "Battle Start";
        Transform target = FindComponent<Text>("TurnCount").transform;
        TweenController.Controller().ChangeTextAlpha(target, 0, 0, text_anime_time);
        TweenController.Controller().ChangeTextAlpha(target, 0, 1, text_anime_time);
        TweenController.Controller().ChangeTextAlpha(target, 1, 1, text_anime_time);
        TweenController.Controller().ChangeTextAlpha(target, 1, 0, text_anime_time);
        BattleController.Controller().ChangeTurn();
    }

    protected override void OnButtonClick(string button_name)
    {
        /*
        // select action
        if(click_stage == 0)
            SelectAction(button_name);
        // select target
        else if(click_stage == 1)
            SelectTarget(button_name);

        */
    }

    public void SelectAction(string button_name)
    {
        // int action_index;

        // requires select enemy
        if(button_name.Contains("SkillSlot"))
        {
            // int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            // BattleController.Controller().PlayerAction(index, true);
            // StartAction();
        }
        else if(button_name.Contains("ItemSlot"))
        {
            // int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            // BattleController.Controller().PlayerAction(index, false);
            // StartAction();
        }
        else if(button_name == "TurnTime")
        {
            // BattleController.Controller().ChangeTurn();
        }
    }



    public void SelectTarget()
    {
        // int target_index;
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
    public void Pause(bool pause)
    {
        this.pause = pause;
    }

    // Turn Text Setting
    public void SetTurnText(bool is_player)
    {
        FindComponent<Text>("TurnSideName").text = (is_player) ? "Player's Turn" : "Enemy's Turn" ;
    }
    public void SetTurnNum(int turn_num)
    {
        FindComponent<Text>("TurnCount").text = turn_num.ToString();
        Transform target = FindComponent<Text>("TurnCount").transform;
        TweenController.Controller().ChangeTextAlpha(target, 0, 1, text_anime_time);
        TweenController.Controller().ChangeTextAlpha(target, 1, 1, text_anime_time);
        TweenController.Controller().ChangeTextAlpha(target, 1, 0, text_anime_time);
    }

    // Action Point Setting
    public void SetActionPoint(int curr_num)
    {
        curr_action_point = curr_num;
        if(curr_action_point > action_point_index)
            InvokeRepeating( "GetActionPoint", 0, point_anime_time );
        else if(curr_action_point < action_point_index)
            InvokeRepeating( "UseActionPoint", 0, point_anime_time );

        Transform target = FindComponent<Image>("TimeCover").transform;
    }
    public void GetActionPoint()
    {
        action_point_index ++;

        Transform target = FindComponent<Image>("Point ("+action_point_index+")").transform;
        TweenController.Controller().ChangeSizeTo(target, new Vector3(1, 1, 0), point_anime_time, TweenType.Smooth);
        
        if(action_point_index >= curr_action_point)
            CancelInvoke("GetActionPoint");
    }
    public void UseActionPoint()
    {
        Transform target = FindComponent<Image>("Point ("+action_point_index+")").transform;
        TweenController.Controller().ChangeSizeTo(target, new Vector3(0, 0, 0), point_anime_time, TweenType.Smooth);

        action_point_index --;
        if(action_point_index <= curr_action_point)
            CancelInvoke("UseActionPoint");
    }
    
    // turn switch
    public void PlayerTurn( float time, int action_points, int turn_count)
    {
        // reset time and action point
        SetTime(time);
        SetActionPoint(action_points);

        // set turn cover
        Transform target = FindComponent<Image>("TimeCover").transform;
        TweenController.Controller().MoveToPosition(target, new Vector3(0, 150, 0), back_anime_time, true, TweenType.Smooth);
        // enable button
        FindComponent<Button>("TurnTime").interactable = true;
        // set action cover
        target = FindComponent<Image>("ActionCover").transform;
        TweenController.Controller().MoveToPosition(target, new Vector3(0, -350, 0), back_anime_time, true, TweenType.Smooth);
        // 
        SetTurnText(true);
        SetTurnNum(turn_count);
    }
    public void EnemyTurn()
    {
        // stop timing
        pause = true;
        // set turn cover
        Transform target = FindComponent<Image>("TimeCover").transform;
        TweenController.Controller().MoveToPosition(target, new Vector3(0, 2.75f, 0), back_anime_time, true, TweenType.Smooth);
        // disable button
        FindComponent<Button>("TurnTime").interactable = false;

        target = FindComponent<Image>("ActionCover").transform;
        TweenController.Controller().MoveToPosition(target, new Vector3(0, 0, 0), back_anime_time, true, TweenType.Smooth);
        SetTurnText(false);
    }

    // Set Skill Icons
    public void SetSkill()
    {
        for(int i = 0; i < player.player_build[player.player_build_index].skills.Count; i ++)
        {
            Transform slot = FindComponent<Button>("SkillSlot ("+i+")").transform;

            Skill skill = player.player_build[player.player_build_index].skills[i];

            if(skill != null)
            {
                FindComponent<Button>("SkillSlot ("+i+")").interactable = (skill.cold_left == 0);
                slot.GetChild(0).gameObject.SetActive(true);
                slot.GetChild(1).gameObject.SetActive(skill.cold_left == 0);
                slot.GetChild(2).gameObject.SetActive(skill.cold_left > 0);
                slot.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+skill.skill_id);
                slot.GetChild(1).GetComponent<Text>().text = skill.skill_cost.ToString();
                slot.GetChild(2).GetComponent<Text>().text = skill.cold_left.ToString();
            }
            else
            {
                FindComponent<Button>("SkillSlot ("+i+")").interactable = false;
                slot.GetChild(0).gameObject.SetActive(false);
                slot.GetChild(1).gameObject.SetActive(false);
                slot.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
    // Set Potion Icons
    public void SetItem()
    {
        for(int i = 0; i < player.player_build[player.player_build_index].potions.Count; i ++)
        {
            Transform slot = FindComponent<Button>("ItemSlot ("+i+")").transform;
            if(player.player_build[player.player_build_index].potions[i] != null)
            {
                int num = player.player_build[player.player_build_index].potions[i].item_num;
                string potion_id = player.player_build[player.player_build_index].potions[i].item_id;
                PotionBase potion = ItemController.Controller().DictPotionInfo(potion_id);

                FindComponent<Button>("ItemSlot ("+i+")").interactable = true;
                slot.GetChild(0).gameObject.SetActive(true);
                slot.GetChild(1).gameObject.SetActive(true);
                slot.GetChild(2).gameObject.SetActive(true);
                slot.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+potion.item_id);
                slot.GetChild(1).GetComponent<Text>().text = potion.potion_cost.ToString();
                slot.GetChild(2).GetComponent<Text>().text = num.ToString();
            }
            else
            {
                FindComponent<Button>("ItemSlot ("+i+")").interactable = false;
                slot.GetChild(0).gameObject.SetActive(false);
                slot.GetChild(1).gameObject.SetActive(false);
                slot.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    // The action animation start
    public void StartAction()
    {
        FindComponent<Button>("TurnTime").interactable = false;
        for(int i = 0; i < skill_slot_num; i ++)
        {
            FindComponent<Button>("SkillSlot ("+i+")").interactable = false;
        }
        for(int i = 0; i < potion_slot_num; i ++)
        {
            FindComponent<Button>("ItemSlot ("+i+")").interactable = false;
        }
    }
    // The action animation end
    public void EndAction()
    {
        FindComponent<Button>("TurnTime").interactable = true;
        for(int i = 0; i < skill_slot_num; i ++)
        {
            FindComponent<Button>("SkillSlot ("+i+")").interactable = true;
        }
        for(int i = 0; i < potion_slot_num; i ++)
        {
            FindComponent<Button>("ItemSlot ("+i+")").interactable = true;
        }
    }

    private void ShowUnitInfo(bool is_show)
    {

    }

    private void ShowEffectInfo(bool is_show)
    {

    }

    private void ShowSkillInfo(bool is_show)
    {

    }
}
