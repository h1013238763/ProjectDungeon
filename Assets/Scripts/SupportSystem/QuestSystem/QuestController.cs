using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : BaseController<QuestController>
{
    public List<Quest> quest_list = new List<Quest>();
    public int quest_limit = 10;

    public void OpenQuestGUI()
    {
        GUIController.Controller().ShowPanel<QuestPanel>("QuestPanel", 3, (p) => 
        {
            for(int i = 0; i < quest_list.Count; i ++)
            {
                p.quest_names.Add(quest_list[i].quest_name);
            }
        });
    }
    
    public Quest QuestInfo(string id)
    {
        return ResourceController.Controller().Load<Quest>("Objects/Quest/"+id);
    }

    public void AcceptQuest(Quest quest)
    {

    }

    public void InitialData()
    {
        EventController.Controller().AddEventListener<Quest>("AcceptQuest", AcceptQuest);
    }
}

public enum QuestGoal
{
    Kill,
    Protect,
    Survival,
    Use,
    Complete
}

public enum QuestStatus
{
    Lock,
    Unlock,
    Accept,
    Complete,
    Finish
}
