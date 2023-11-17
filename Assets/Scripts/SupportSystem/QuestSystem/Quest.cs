using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "ProjectDungeon/Quest", order = 0)]
public class Quest : ScriptableObject {
    
    public string quest_id;         // the id of quest for searching
    public string quest_name;       // the name of quest for display

    [TextArea]
    public string quest_describe;   // the describe of quest such as background

    public QuestGoal quest_goal;
    public string quest_target;
    public int quest_progress;
    public int quest_progress_curr;

    public QuestStatus quest_status;// the current status of quest

    public Dialogue[] quest_dialogue;

    public string quest_reward;

    public void Progress()
    {
                    
    }
}