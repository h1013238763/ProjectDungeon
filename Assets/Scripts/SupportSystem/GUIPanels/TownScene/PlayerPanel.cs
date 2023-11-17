using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : PanelBase
{
    List<Skill> avail_skill;
    PlayerController controller;

    PlayerData player;
    InventPanel invent;

    public override void ShowSelf()
    {
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1,  (p) => {
            p.panel = "PlayerPanel";
            invent = p;
        });
        controller = PlayerController.Controller();
        player = controller.data;
        controller.panel = this;
        ResetSkill();
        ResetPanel();
    }

    protected override void OnButtonClick(string button_name)
    {
        PlayerBuild build = player.player_build[player.player_build_index];

        if(button_name == "CloseBtn")
        {
            GUIController.Controller().RemovePanel("InventPanel");
            GUIController.Controller().RemovePanel("PlayerPanel");
        }
        // switch build
        else if(button_name.Contains("BuildBtn"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player.player_build_index = slot;
            ResetPanel();
        }
        // unequip
        else if(button_name.Contains("BuildSkillSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            build.skills[slot] = null;
            
            ResetPanel();
        }
        else if(button_name.Contains("PotionSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            build.potions[slot] = null;
            invent.ResetInventPanel();
            ResetPanel();
        }
        else if(button_name.Contains("EquipSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            build.equips[slot] = null;
            invent.ResetInventPanel();
            ResetPanel();
        }
        else if(button_name.Contains("SkillSlot"))
        {
            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            for(int i = 0; i < 10; i ++)
            {
                if(build.skills[i] == null)
                {
                    build.skills[i] = avail_skill[slot];
                    return;
                } 
                ResetPanel();
            }   
        }
    }

    public void ResetPanel()
    {
        PlayerBuild build = player.player_build[player.player_build_index];
        Button slot;
        // equip
        for(int i = 0; i < 6; i ++)
        {
            slot = FindComponent<Button>("EquipSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(build.equips[i] != null);
            if(build.equips[i] != null)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+build.equips[i].item_id);
        }
        // skill
        for(int i = 0; i < 10; i ++)
        {
            slot = FindComponent<Button>("BuildSkillSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(build.skills[i] != null);
            if(build.skills[i] != null)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+build.skills[i].skill_id);
        }
        // potion
        for(int i = 0; i < 10; i ++)
        {
            slot = FindComponent<Button>("PotionSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(build.potions[i] != null);
            if(build.potions[i] != null)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+build.potions[i].item_id);
        }
        // exp
        int max_exp = controller.GetLevelExp(player.player_level);
        FindComponent<Text>("LevelText").text = player.player_level.ToString();
        FindComponent<Text>("ExpText").text = player.player_exp.ToString() + " / " + max_exp.ToString();
        FindComponent<Slider>("LevelBar").value = player.player_exp/max_exp;

        // Attributes
        int attack = player.base_attack;
        int defense = player.base_defense;
        int health = player.base_health;
        for(int i = 0; i < 6; i ++)
        {
            if(build.equips[i] == null)
                continue;
            
            attack += build.equips[i].GetAttributes("Attack");
            defense += build.equips[i].GetAttributes("Defense");
            health += build.equips[i].GetAttributes("Health");
        }
        FindComponent<Text>("HealthText").text = "Health: " + health.ToString();
        FindComponent<Text>("AttackText").text = "Attack: " + attack.ToString();
        FindComponent<Text>("DefenseText").text = "Defense: " + defense.ToString();
    }

    public void ResetSkill()
    {
        // reset skills
        avail_skill = SkillController.Controller().data.avail_skill;

        for(int i = 0; i < 30; i ++)
        {
            Button slot = FindComponent<Button>("SkillSlot ("+i+")");
            slot.gameObject.SetActive(i < avail_skill.Count);
            if(i < avail_skill.Count)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+avail_skill[i].skill_id);
        }
    }

}
