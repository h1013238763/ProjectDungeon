using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerPanel : PanelBase
{
    PlayerController player_control;
    List<string> avail_skill;
    PlayerBuild player_build;
    InventPanel invent;

    public override void ShowSelf()
    {
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1,  (p) => {
            p.panel = "PlayerPanel";
            invent = p;
        });
        
        player_control = PlayerController.Controller();
        player_build = player_control.GetCurrBuild();
        player_control.panel = this;
        avail_skill = SkillController.Controller().GetAvailSkills(player_control.data.player_build_index);

        Button btn;
        // Add custom event listener on each slot, show item information
        for(int i = 0; i < 6; i ++)
        {
            btn = FindComponent<Button>("EquipSlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
        for(int i = 0; i < 8; i ++)
        {
            btn = FindComponent<Button>("PotionSlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
        for(int i = 0; i < 8; i ++)
        {
            btn = FindComponent<Button>("SkillSlot (" + i + ")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
        for(int i = 0; i < 30; i ++)
        {
            btn = FindComponent<Button>("AvailSkillSlot ("+i+")");
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(btn, EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
        
        ResetSkill();
        ResetPanel();
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name == "CloseBtn")
        {
            AudioController.Controller().StartSound("ShopRing");

            GUIController.Controller().RemovePanel("InventPanel");
            GUIController.Controller().RemovePanel("PlayerPanel");
        }
        // switch build
        else if(button_name.Contains("BuildBtn"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player_control.data.player_build_index = slot;
            player_build = player_control.GetCurrBuild();
            avail_skill = SkillController.Controller().GetAvailSkills(slot);
            invent.ResetInventPanel();
            ResetSkill();
            ResetPanel();
        }
        // equip skill
        else if(button_name.Contains("AvailSkillSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            if(player_control.Equip("Skill", null, avail_skill[slot]))
            {
                ResetPlayerSkill();
                ResetSkill();
            }  
        }
        // unequip equip
        else if(button_name.Contains("EquipSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player_build.equips[slot] = null;
            invent.ResetInventPanel();
            ResetPlayerEquip();
        }
        // unequip potion
        else if(button_name.Contains("PotionSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player_build.potions.RemoveAt(slot);
            invent.ResetInventPanel();
            ResetPlayerPotion();
        }
        // unequip skill
        else if(button_name.Contains("SkillSlot"))
        {
            AudioController.Controller().StartSound("Equip");

            int slot = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, button_name.IndexOf(")")-button_name.IndexOf("(")-1 ));
            player_build.skills.RemoveAt(slot);
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
        int max_exp = player_control.GetLevelExp(player_control.data.player_level);
        FindComponent<Text>("LevelText").text = player_control.data.player_level.ToString();
        FindComponent<Text>("ExpText").text = player_control.data.player_exp.ToString() + " / " + max_exp.ToString();
        FindComponent<Slider>("LevelBar").value = player_control.data.player_exp/max_exp;
        for(int i = 0; i < 5; i ++)
        {
            FindComponent<Button>("BuildBtn ("+i+")").interactable = ( i != player_control.data.player_build_index);
        }
    }

    public void ResetPlayerEquip()
    {
        Button slot;
        // equip
        for(int i = 0; i < 6; i ++)
        {
            slot = FindComponent<Button>("EquipSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive(player_build.equips[i] != null);
            if(player_build.equips[i] != null)
            {
                Debug.Log(ItemController.Controller().GetImage(player_build.equips[i].item_id));
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ItemController.Controller().GetImage(player_build.equips[i].item_id);
            }
        }
         // Attributes
        FindComponent<Text>("HealthText").text = "Health: " + player_control.GetHealth().ToString();
        FindComponent<Text>("AttackText").text = "Attack: " + player_control.GetAttack().ToString();
        FindComponent<Text>("DefenseText").text = "Defense: " + player_control.GetDefense().ToString();
    }

    public void ResetPlayerPotion()
    {
        Button slot;
        // potion
        for(int i = 0; i < player_control.potion_limit; i ++)
        {
            slot = FindComponent<Button>("PotionSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive( i < player_build.potions.Count );
            if( i < player_build.potions.Count)
            {
                Debug.Log("Potion "+i+"/"+player_build.potions.Count+": "+player_build.potions[i]);
                Debug.Log(ItemController.Controller().GetImage(player_build.potions[i]));
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ItemController.Controller().GetImage(player_build.potions[i]);
            }
                
        }
    }

    public void ResetPlayerSkill()
    {
        Button slot;
        // skill
        for(int i = 0; i < player_control.skill_limit; i ++)
        {
            slot = FindComponent<Button>("SkillSlot ("+i+")");
            slot.transform.GetChild(0).gameObject.SetActive( i < player_build.skills.Count );
            if( i < player_build.skills.Count )
                slot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = SkillController.Controller().GetImage(player_build.skills[i]);
        }
    }

    public void ResetSkill()
    {
        // set each slot
        Button slot = null;
        for(int i = 0; i < 30; i ++)
        {
            slot = FindComponent<Button>("AvailSkillSlot ("+i+")");
            slot.gameObject.SetActive(i < avail_skill.Count);

            if( i < avail_skill.Count)
            {
                slot.GetComponent<Image>().sprite = SkillController.Controller().GetImage(avail_skill[i]);
                slot.interactable = !player_build.skills.Contains(avail_skill[i]);
            }
        }
    }
    

    // show info panel on pointer enter
    private void OnPointerEnter(PointerEventData event_data)
    {
        string name = event_data.pointerEnter.name;
        UnityAction<InfoPanel> action = null;

        if(!name.Contains("Slot ("))
            return;
        
        int index = Int32.Parse(name.Substring(name.IndexOf("(")+1, name.IndexOf(")")-name.IndexOf("(")-1));
        // show skill info
        if(name.Contains("AvailSkillSlot"))
        {
            if(index < 0 || index >= avail_skill.Count)
                return;

            action = new UnityAction<InfoPanel>((p) =>
            {
                p.info_type = "Skill";
                p.info_skill = player_build.skills[index];
                p.mouse_pos = event_data.position;
            });
        }
        else if(name.Contains("SkillSlot"))
        {
            if(index < 0 || index >= player_build.skills.Count)
                return;
            if(player_build.skills[index] == null)
                return;

            action = new UnityAction<InfoPanel>((p) =>
            {
                p.info_type = "Skill";
                p.info_skill = player_build.skills[index];
                p.mouse_pos = event_data.position;
            });
        }
        else if(name.Contains("Slot ("))
        {
            if(name.Contains("Equip"))
            {
                if(index < 0 || index >= player_build.equips.Count)
                    return;
                if(player_build.equips[index] == null)
                    return;

                action = new UnityAction<InfoPanel>((p) =>
                {
                    p.info_type = "Equip";
                    p.info_item = player_build.equips[index];
                    p.mouse_pos = event_data.position;
                });
            }
            else
            {
                if(index < 0 || index >= player_build.potions.Count)
                    return;
                if(player_build.potions[index] == null)
                    return;

                action = new UnityAction<InfoPanel>((p) =>
                {
                    p.info_type = "Potion";
                    p.info_item = ItemController.Controller().InventPotionInfo(player_build.potions[index]);
                    p.mouse_pos = event_data.position;
                });
            }
        }

        GUIController.Controller().ShowPanel<InfoPanel>("InfoPanel", 3, action);
    }

    // hide info panel on pointer exit
    private void OnPointerExit(PointerEventData event_data)
    {
        GUIController.Controller().RemovePanel("InfoPanel");
    }
}
