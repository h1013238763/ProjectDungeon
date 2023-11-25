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
            AudioController.Controller().StartSound("ButtonClick");

            GUIController.Controller().RemovePanel("InventPanel");
            GUIController.Controller().RemovePanel("PlayerPanel");
        }
        // switch build
        else if(button_name.Contains("BuildBtn"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player.player_build_index = slot;
            invent.ResetInventPanel();
            ResetPanel();
        }
        // equip skill
        else if(button_name.Contains("AvailSkillSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            for(int i = 0; i < build.skills.Count; i ++)
            {
                if(build.skills[i] == null)
                {
                    build.skills[i] = avail_skill[slot];
                    ResetPlayerSkill();
                    ResetSkill();
                    return;
                } 
            }   
        }
        // unequip equip
        else if(button_name.Contains("EquipSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            build.equips[slot] = null;
            invent.ResetInventPanel();
            ResetPlayerEquip();
        }
        // unequip potion
        else if(button_name.Contains("PotionSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            build.potions[slot] = null;
            invent.ResetInventPanel();
            ResetPlayerPotion();
        }
        // unequip skill
        else if(button_name.Contains("SkillSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            Debug.Log("skill:"+slot);
            build.skills[slot] = null;

            ResetPlayerSkill();
            ResetSkill();
        }
        
    }

    public void ResetPanel()
    {
        ResetPlayerEquip();
        ResetPlayerPotion();
        ResetPlayerSkill();
        // exp
        int max_exp = controller.GetLevelExp(player.player_level);
        FindComponent<Text>("LevelText").text = player.player_level.ToString();
        FindComponent<Text>("ExpText").text = player.player_exp.ToString() + " / " + max_exp.ToString();
        FindComponent<Slider>("LevelBar").value = player.player_exp/max_exp;
    }

    public void ResetPlayerEquip()
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

    public void ResetPlayerPotion()
    {
        PlayerBuild build = player.player_build[player.player_build_index];
        Button slot;
        // potion
        for(int i = 0; i < build.potions.Count; i ++)
        {
            slot = FindComponent<Button>("PotionSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(build.potions[i] != null);
            if(build.potions[i] != null)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+build.potions[i].item_id);
        }
    }

    public void ResetPlayerSkill()
    {
        PlayerBuild build = player.player_build[player.player_build_index];
        Button slot;
        // skill
        for(int i = 0; i < build.skills.Count; i ++)
        {
            slot = FindComponent<Button>("SkillSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(build.skills[i] != null);
            if(build.skills[i] != null)
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+build.skills[i].skill_id);
        }
    }

    public void ResetSkill()
    {
        // reset skills
        avail_skill = SkillController.Controller().data.avail_skill;

        for(int i = 0; i < 30; i ++)
        {
            Button slot = FindComponent<Button>("AvailSkillSlot ("+i+")");
            
            slot.gameObject.SetActive(i < avail_skill.Count);
            if(i < avail_skill.Count)
            {
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Skills/"+avail_skill[i].skill_id);
                slot.interactable = !player.player_build[player.player_build_index].skills.Contains(avail_skill[i]);
            }      
        }
    }

}
