using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionCraftPanel : PanelBase
{
    private string curr_recipe;
    private int craft_num;
    private int craft_limit;
    public Quest quest;

    public override void ShowSelf()
    {
        ResetComponent();
        RefreshRecipe();
        craft_num = 0;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// player click buttons
    /// </summary>
    /// <param name="button_name">the name of button</param>
    protected override void OnButtonClick(string button_name)
    {
        if(button_name.Contains("RecipeBtn"))
        {
            AudioController.Controller().StartSound("Equip");

            SetCurrentRecipe(FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Image>().sprite.name);
        }
        // craft time edit buttons
        else if(button_name == "AddBtn")
        {
            AudioController.Controller().StartSound("Equip");

            craft_num ++;
            SetCraftNumText(craft_num);
        }
        else if(button_name == "Add10Btn")
        {
            AudioController.Controller().StartSound("Equip");

            craft_num += 10;
            SetCraftNumText(craft_num);
        }
        else if(button_name == "CutBtn")
        {
            AudioController.Controller().StartSound("Equip");

            craft_num --;
            SetCraftNumText(craft_num);
        }
        else if(button_name == "Cut10Btn")
        {
            AudioController.Controller().StartSound("Equip");

            craft_num -= 10;
            SetCraftNumText(craft_num);
        }
        // craft potion
        else if(button_name == "CraftBtn")
        {
            AudioController.Controller().StartSound("ButtonClick");

            ItemController.Controller().CraftPotion(curr_recipe, craft_num);
            SetCurrentRecipe(curr_recipe);
        }
        // exit panel
        else if(button_name == "CloseBtn")
        {
            AudioController.Controller().StartSound("ShopRing");
            
            GUIController.Controller().RemovePanel("PotionCraftPanel");
        }
        else if(button_name == "QuestTip")
        {
            AudioController.Controller().StartSound("AcceptQuest");

            EventController.Controller().EventTrigger<Quest>("AcceptQuest", quest);
            quest = null;
            ResetComponent();
        }
    }

    private void RefreshRecipe()
    {
        int slot_index = 0;

        foreach( var pair in ItemController.Controller().dict_recipe)
        {
            PotionRecipe recipe = pair.Value;
            // set button image
            FindComponent<Button>("RecipeBtn ("+slot_index+")").transform.GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(recipe.recipe_id);
            // set button interactable
            FindComponent<Button>("RecipeBtn ("+slot_index+")").interactable = recipe.recipe_unlock;

            slot_index ++;
        }
    }

    private void SetCraftNumText(int num)
    {
        FindComponent<Text>("DisplayText").text = num.ToString();
        FindComponent<Button>("AddBtn").gameObject.SetActive(craft_num < craft_limit);
        FindComponent<Button>("Add10Btn").gameObject.SetActive(craft_num+10 <= craft_limit);
        FindComponent<Button>("CutBtn").gameObject.SetActive(craft_num > 0);
        FindComponent<Button>("Cut10Btn").gameObject.SetActive(craft_num >= 10);
        FindComponent<Button>("CraftBtn").interactable = craft_num != 0;
        
        ResetConsumeNum();
    }

    private void SetCurrentRecipe(string id)
    {
        Debug.Log(id);
        // set varibles
        curr_recipe = id;
        craft_limit = ItemController.Controller().RecipeProductLimit(id);
        craft_num = 0;
        SetCraftNumText(0);

        // set gui panel
        Transform item_slot = FindComponent<Image>("PotionInfoGrid").transform;
        PotionBase potion = ItemController.Controller().DictPotionInfo(curr_recipe);
        if(potion == null)
            return;

        // potion info set
        item_slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(curr_recipe);
        item_slot.GetChild(1).GetComponent<Text>().text = potion.item_name;
        item_slot.GetChild(2).GetComponent<Text>().text = potion.item_describe;
        FindComponent<Image>("RecipeImage").color = new Color(255,255,225,255);
        FindComponent<Text>("ItemName").gameObject.SetActive(true);
        FindComponent<Text>("ItemDescribe").gameObject.SetActive(true);
        // potion require set

        ResetConsumeNum();

        SetCraftNumText(0);
    }

    public void ResetConsumeNum()
    {
        PotionRecipe recipe = ItemController.Controller().RecipeInfo(curr_recipe);
        for(int i = 0; i < 4; i ++)
        {
            Transform item_slot = FindComponent<Image>("ItemRequire ("+i+")").transform;

            if(i < recipe.recipe_consume.Length)
            {
                item_slot.gameObject.SetActive(true);
                item_slot = FindComponent<Image>("ItemRequire ("+i+")").transform;
                item_slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ItemController.Controller().GetImage(recipe.recipe_consume[i]);
                item_slot.GetChild(1).GetComponent<Text>().text = ItemController.Controller().DictItemInfo(recipe.recipe_consume[i]).item_name;
                item_slot.GetChild(2).GetComponent<Text>().text = "x " + (recipe.recipe_consume_num[i] * craft_num).ToString();

                if(ItemController.Controller().InventItemInfo(recipe.recipe_consume[i]) == null)
                    item_slot.GetChild(2).GetComponent<Text>().text += "(0)";   
                else
                    item_slot.GetChild(2).GetComponent<Text>().text += "("+ItemController.Controller().InventItemInfo(recipe.recipe_consume[i]).item_num+")";
            }
            else
            {
                item_slot.gameObject.SetActive(false);
            }   
        }
    }

    private void ResetComponent()
    {
        FindComponent<Image>("RecipeImage").color = new Color32(255,255,225,0);
        FindComponent<Text>("ItemName").gameObject.SetActive(false);
        FindComponent<Text>("ItemDescribe").gameObject.SetActive(false);
        for(int i = 0; i < 4; i ++)
        {
            FindComponent<Image>("ItemRequire ("+i+")").gameObject.SetActive(false);
        }
        FindComponent<Button>("AddBtn").gameObject.SetActive(false);
        FindComponent<Button>("Add10Btn").gameObject.SetActive(false);
        FindComponent<Button>("CutBtn").gameObject.SetActive(false);
        FindComponent<Button>("Cut10Btn").gameObject.SetActive(false);
        FindComponent<Button>("CraftBtn").interactable = false;
    }

}
