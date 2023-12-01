using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : PanelBase
{
    public ItemController item_control;

    public string info_type = "";
    public Item info_item = null;
    public string info_skill = null;
    public Vector2 mouse_pos = new Vector2(0, 0);
    public Vector2 screen_size = new Vector2(0, 0);
    public Vector2 self_size = new Vector2(0, 0);

    RectTransform self_transform;

    // Item tier color
    private List<Color> tier_color = new List<Color>{
        new Color(0.937f, 1f, 0.948f, 1f ),
        new Color(0.937f, 1f, 0.956f, 1f ),
        new Color(0.937f, 1f, 0.994f, 1f ),
        new Color(0.971f, 0.937f, 1f, 1f ),
        new Color(1f, 0.937f, 0.956f, 1f )
    };
    private List<Color> tier_line_color = new List<Color>{
        new Color(0.311f, 0.311f, 0.311f, 0.13f ),
        new Color(0.311f, 0.632f, 0.238f, 0.13f ),
        new Color(0.125f, 0.372f, 0.5f, 0.13f ),
        new Color(0.425f, 0.125f, 0.502f, 0.13f ),
        new Color(0.502f, 0.125f, 0.211f, 0.13f )
    };
    private List<Color> tier_light_color = new List<Color>{
        new Color(0.955f, 1f, 0.740f, 0.325f ),
        new Color(0.442f, 0.868f, 0.788f, 0.325f ),
        new Color(0.192f, 0.5f, 0.867f, 0.325f ),
        new Color(0.620f, 0.192f, 0.867f, 0.325f ),
        new Color(0.867f, 0.192f, 0.222f, 0.325f )
    };

    public override void ShowSelf()
    {
        gameObject.SetActive(true);

        self_transform = FindComponent<Image>("Background").GetComponent<RectTransform>();

        if(item_control == null)
            item_control = ItemController.Controller();
        // assign each color

        DisplayInfo(info_type, info_item, info_skill);
    }

    public override void HideSelf()
    {
        gameObject.SetActive(false);
    }

    public void DisplayInfo( string type, Item item = null, string skill = null, BattleUnit unit = null)
    {
        FindComponent<Image>("SkillInfo").gameObject.SetActive(type == "Skill");
        FindComponent<Text>("ItemLevel").gameObject.SetActive(type == "Equip");
        FindComponent<Image>("EquipInfoGrid").gameObject.SetActive(type == "Equip");

        if(item != null)
        {
            ItemBase item_info = null;

            if(type == "Equip")
            {
                item_info = item_control.DictEquipInfo(item.item_id) as ItemBase;

                Equip equip = item as Equip;

                // set extra info
                FindComponent<Text>("ItemLevel").text = "  [ lv : "+equip.equip_level+" ]";
                // attributes
                FindComponent<Text>("EquipAttack").text = "attack  : "+equip.GetAttributes("Attack").ToString();
                FindComponent<Text>("EquipDefense").text = "defense : "+equip.GetAttributes("Defense").ToString();
                FindComponent<Text>("EquipHealth").text = "health  : "+equip.GetAttributes("Health").ToString();
                // enchants
                for(int i = 0; i < 5; i ++)
                {
                    FindComponent<Text>("EnchantText ("+i+")").gameObject.SetActive(i < equip.enchant_limit);
                    if(i < equip.enchant_limit)
                        FindComponent<Text>("EnchantText ("+i+")").text = equip.equip_enchants[i].enchant_describe;
                }
            }
            else if(type == "Potion")
            {
                item_info = item_control.DictPotionInfo(item.item_id) as ItemBase;
            }
            else if(type == "Item")
            {
                item_info = item_control.DictItemInfo(item.item_id);
            }

            // set name
            FindComponent<Text>("ItemName").text = item_info.item_name;
            FindComponent<Text>("ItemName").color = tier_color[item_info.item_tier-1];
            Outline[] outlines = FindComponent<Text>("ItemName").gameObject.GetComponents<Outline>();
            outlines[0].effectColor = tier_line_color[item_info.item_tier-1];
            outlines[1].effectColor = tier_light_color[item_info.item_tier-1];

            // set describe
            FindComponent<Text>("ItemDescribe").text = item_info.item_describe;
        }
        
        else if(skill != null)
        {
            // set name
            SkillData skill_data = SkillController.Controller().GetSkill(skill);
            int level = SkillController.Controller().GetSkillLevel(skill);

            FindComponent<Text>("ItemName").text = skill_data.skill_name;
            FindComponent<Text>("ItemName").color = tier_color[0];
            Outline[] outlines = FindComponent<Text>("ItemName").gameObject.GetComponents<Outline>();
            outlines[0].effectColor = tier_line_color[0];
            outlines[1].effectColor = tier_light_color[0];
            // level
            FindComponent<Text>("ItemLevel").gameObject.SetActive(skill_data.skill_career != SkillCareer.Mob);
            FindComponent<Text>("ItemLevel").text = "  [ lv : "+ level+" ]";

            // cost & colddown
            FindComponent<Text>("SkillCost").gameObject.SetActive(skill_data.skill_career != SkillCareer.Mob);
            FindComponent<Text>("SkillCold").gameObject.SetActive(skill_data.skill_career != SkillCareer.Mob);
            if(skill_data.skill_career != SkillCareer.Mob)
            {
                if(level > 0)
                {
                    FindComponent<Text>("SkillCost").text = "cost : "+skill_data.skill_cost[level-1];
                    FindComponent<Text>("SkillCold").text = "colddown : "+skill_data.skill_cold[level-1];
                }
                else
                {
                    FindComponent<Text>("SkillCost").text = "cost : "+skill_data.skill_cost[0];
                    FindComponent<Text>("SkillCold").text = "colddown : "+skill_data.skill_cold[0];
                }
            }
            // get formatted describe from skill controller
            FindComponent<Text>("ItemDescribe").text = SkillController.Controller().GetDescribe(skill, unit);
        }

        // change position

        type = "";
        info_item = null;
        info_skill = null;
    }

    void LateUpdate()
    {
        try{
            if(self_size != self_transform.sizeDelta)
            {
                self_size = self_transform.sizeDelta;
                // calculate the panel position

                float radian = Mathf.Atan2(mouse_pos.y - Screen.height/2, mouse_pos.x - Screen.width/2);
                float hypo = Mathf.Sqrt(self_size.x*self_size.x/4 + self_size.y*self_size.y/4) + 50;
                Vector2 pos = new Vector2( mouse_pos.x-hypo*Mathf.Cos(radian), mouse_pos.y-hypo*Mathf.Sin(radian));

                self_transform.position = pos;
            }
        }
        catch(System.Exception)
        {
            GUIController.Controller().RemovePanel("InfoPanel");
        }
    }
}
