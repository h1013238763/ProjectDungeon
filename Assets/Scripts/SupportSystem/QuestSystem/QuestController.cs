using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : BaseController<QuestController>
{
    /*

    public Dictionary<string, Quest> dict_quest = new Dictionary<string, Quest>();
    public int quest_limit = 10;
    
    public QuestData data;

    public void OpenQuestGUI()
    {
        GUIController.Controller().ShowPanel<QuestPanel>("QuestPanel", 3, (p) => 
        {
            for(int i = 0; i < data.quest_list.Count; i ++)
            {
                p.quest_names.Add(data.quest_list[i]);
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
    */
}

public class QuestData
{
    public List<string> quest_list = new List<string>();        // accepted quest
    public List<int> quest_progress = new List<int>();          // the progress of each quest
    public List<string> complete_quest = new List<string>();    // complete quest
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
