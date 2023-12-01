using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuestPanel : PanelBase
{
    /*
    public List<string> quest_names = new List<string>();

    public override void ShowSelf()
    {
        for(int i = 0; i < 10; i ++)
        {
            if(i >= quest_names.Count)
                FindComponent<Button>("QuestSlot ("+i+")").gameObject.SetActive(false);
            else
            {
                FindComponent<Button>("QuestSlot ("+i+")").gameObject.SetActive(true);
                FindComponent<Button>("QuestSlot ("+i+")").transform.GetChild(0).GetComponent<Text>().text = quest_names[i];
            }
        }

        FindComponent<Text>("QuestTitle").gameObject.SetActive(false);
        FindComponent<Text>("QuestDescribe").gameObject.SetActive(false);
        FindComponent<Text>("QuestRequire").gameObject.SetActive(false);
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name == "CloseBtn")
        {
            GUIController.Controller().RemovePanel("QuestPanel");
        }

        else if(button_name.Contains("QuestSlot"))
        {
            int index = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
            Quest quest = QuestController.Controller().QuestInfo(quest_names[index]);

            if(quest == null)
                return;

            FindComponent<Text>("QuestTitle").gameObject.SetActive(true);
            FindComponent<Text>("QuestDescribe").gameObject.SetActive(true);
            FindComponent<Text>("QuestRequire").gameObject.SetActive(true);
            FindComponent<Text>("QuestTitle").text = quest.quest_name;
            FindComponent<Text>("QuestDescribe").text = quest.quest_describe;
            FindComponent<Text>("QuestRequire").text = quest.quest_goal+" "+quest.quest_target+" ("+quest.quest_progress_curr+"/"+quest.quest_progress+")";
        }
    }

    */
}
