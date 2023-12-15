using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillLearnPanel : PanelBase
{
    public SkillCareer curr_career;
    public List<string> skill_list;
    public Quest quest;

    public SkillController controller;
    public int build_index;

    public override void ShowSelf()
    {
        build_index = PlayerController.Controller().data.player_build_index;
        curr_career = SkillCareer.Fighter;
        controller = SkillController.Controller();
        ResetPanel();

        // assign mouse enter and exit event ( show skill info )
        for(int i = 0; i < 32; i ++)
        {
            Button btn = FindComponent<Button>("SkillSlot ("+i+")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {
    
                PointerEventData event_data = data as PointerEventData;
                string name = event_data.pointerEnter.name;

                // get index
                int index = Int32.Parse(name.Substring(name.IndexOf("(")+1, name.IndexOf(")")-name.IndexOf("(")-1));

                if(index < 0 || index >= skill_list.Count)
                    return;
                // show panel
                GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, (p) =>
                {
                    p.info_type = "Skill";
                    p.info_skill = skill_list[index];
                    p.mouse_pos = event_data.position;
                });
            });
            
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {
                GUIController.Controller().RemovePanel("InfoPanel");
            });
        }
    }

    protected override void OnButtonClick(string button_name)
    {
        // close panel
        if(button_name == "CloseBtn")
        {
            AudioController.Controller().StartSound("ShopRing");

            GUIController.Controller().RemovePanel("SkillLearnPanel");
        }
        // reset all
        else if(button_name == "ResetAllButton")
        {
            AudioController.Controller().StartSound("ButtonClick");

            controller.ResetSkill(true, build_index);
            ResetPanel();
        }
        // reset one
        else if(button_name == "ResetThisButton")
        {
            AudioController.Controller().StartSound("ButtonClick");

            controller.ResetSkill(false, build_index, curr_career);
            ResetPanel();
        }
        // learn skill
        else if(button_name.Contains("SkillSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            if(skill_list[index] == null)
                return;
            controller.AddSkillLevel(skill_list[index], build_index);
            ResetPanel();
        }
        else if(button_name.Contains("CareerBtn"))
        {
            AudioController.Controller().StartSound("Equip");

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
        else if(button_name == "QuestTip")
        {
            AudioController.Controller().StartSound("AcceptQuest");

            EventController.Controller().EventTrigger<Quest>("AcceptQuest", quest);
            quest = null;
            ResetPanel();
        }
        
    }

    private void ResetPanel()
    {
        skill_list = controller.GetCareerSkills(curr_career);

        // skill slot
        Transform slot;
        for(int i = 0; i < 32; i ++)
        {
            slot = FindComponent<Button>("SkillSlot ("+ i +")").transform;

            if(i >= skill_list.Count)
            {
                slot.gameObject.SetActive(false);
                continue;
            }
            else if(skill_list[i] == null)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            // find target slot by skill pos
            slot.gameObject.SetActive(true);
            slot.GetChild(0).gameObject.GetComponent<Image>().sprite = SkillController.Controller().GetImage(skill_list[i]);
            if( controller.GetSkill(skill_list[i]).skill_active )
            {
                if( controller.data.avail_skills[build_index].ContainsKey(skill_list[i]) ) 
                    slot.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = controller.data.avail_skills[build_index][skill_list[i]].ToString();
                else
                    slot.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "0";
            }
            else
            {
                if( controller.data.passive_skills[build_index].ContainsKey(skill_list[i]) ) 
                    slot.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = controller.data.passive_skills[build_index][skill_list[i]].ToString();
                else
                    slot.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "0";
            }
        }

        // skill career point bar
        FindComponent<Slider>("SkillPointBar").value = controller.data.career_point[build_index][curr_career];
        if(FindComponent<Slider>("SkillPointBar").value > 25)
            FindComponent<Slider>("SkillPointBar").value = 25;
        
        // skill cover
        for(int i = 0; i < 6; i ++)
            FindComponent<Image>("SkillCover ("+i+")").gameObject.SetActive(i*5 > FindComponent<Slider>("SkillPointBar").value);
        
        // total skill point
        Debug.Log( FindComponent<Text>("TotalPointValue"));
        FindComponent<Text>("TotalPointValue").text = controller.GetRemainPoint(build_index).ToString();
    }
}
