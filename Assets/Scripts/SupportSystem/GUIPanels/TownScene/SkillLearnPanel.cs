using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillLearnPanel : PanelBase
{

    /* TODO:
        Show skill info when mouse move in
        set all skill slot when show
        learn skill when mouse click
        reset one career connect
        reser all career connect
    */

    public SkillCareer curr_career;
    public List<Skill> skill_list;

    public SkillController controller;

    public override void ShowSelf()
    {
        curr_career = SkillCareer.Fighter;
        controller = SkillController.Controller();
        ResetPanel();
    }

    protected override void OnButtonClick(string button_name)
    {
        // close panel
        if(button_name == "CloseBtn")
        {
            GUIController.Controller().RemovePanel("SkillLearnPanel");
        }
        // reset all
        else if(button_name == "ResetAllButton")
        {
            controller.ResetSkill(true);
            ResetPanel();
        }
        // reset one
        else if(button_name == "ResetThisButton")
        {
            controller.ResetSkill(false, curr_career);
            ResetPanel();
        }
        // learn skill
        else if(button_name.Contains("SkillSlot"))
        {
            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            controller.AddSkillLevel(skill_list[index].skill_id);
            ResetPanel();
        }
        else if(button_name.Contains("CareerBtn"))
        {
            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            if(index == 0)
                curr_career = SkillCareer.Fighter;
            else if(index == 1)
                curr_career = SkillCareer.Archer;
            else if(index == 2)
                curr_career = SkillCareer.Mage;
            else if(index == 3)
                curr_career = SkillCareer.Priest;
            
            ResetPanel();
        }
        
    }

    private void ResetPanel()
    {
        skill_list = controller.GetCareerSkills(curr_career);
        
        // skill slot
        Transform slot;
        for(int i = 0; i < skill_list.Count; i ++)
        {
            Skill skill = skill_list[i];
            // find target slot by skill pos
            slot = FindComponent<Button>("SkillSlot ("+ (skill.skill_pos.x+skill.skill_pos.y*6).ToString() +")").transform;

            slot.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+skill.skill_id);
            slot.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = skill.skill_level.ToString();
        }

        // skill career point bar
        if(curr_career == SkillCareer.Fighter)
            FindComponent<Slider>("SkillPointBar").value = controller.data.point_fighter;
        else if(curr_career == SkillCareer.Archer)
            FindComponent<Slider>("SkillPointBar").value = controller.data.point_archer;
        else if(curr_career == SkillCareer.Mage)
            FindComponent<Slider>("SkillPointBar").value = controller.data.point_mage;
        else if(curr_career == SkillCareer.Priest)
            FindComponent<Slider>("SkillPointBar").value = controller.data.point_priest;
        if(FindComponent<Slider>("SkillPointBar").value > 25)
            FindComponent<Slider>("SkillPointBar").value = 25;
        
        // skill cover
        for(int i = 0; i < 6; i ++)
        {
            FindComponent<Image>("SkillCover ("+i+")").gameObject.SetActive(i*5 <= FindComponent<Slider>("SkillPointBar").value);
        }
        
        // total skill point
        FindComponent<Text>("TotalPointValue").text = controller.data.skill_points.ToString();

        // Skill info
        FindComponent<Image>("SkillInfo").gameObject.SetActive(false);
    }

    /// <summary>
    /// Add custom event listener on each slot, show item information
    /// </summary>
    private void AddCustomeEvent()
    {
        
    }

    // mouse hover to check item information
    /// <summary>
    /// show info panel on pointer enter
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerEnter(PointerEventData event_data)
    {
        // get index
        int index = GetPointerObjectIndex(event_data);

        if(index < 0)
            return;
        if(FindComponent<Button>("TagBtn ("+index+")").interactable == false )
            return;

    }
    /// <summary>
    /// hide info panel on pointer exit
    /// </summary>
    /// <param name="event_data"></param>
    private void OnPointerExit(PointerEventData event_data)
    {
        FindComponent<Text>("TagDescribeText").text = "";
    }
    // break PointerEvent into useful info token
    private int GetPointerObjectIndex(PointerEventData event_data)
    {
        string name = event_data.pointerEnter.name;
        return int.Parse( name.Substring(name.IndexOf("(")+1, 1) );
    }
}
