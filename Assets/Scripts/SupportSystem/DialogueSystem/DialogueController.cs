using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : BaseController<DialogueController>
{
    private Dictionary<string, Dialogue> dict_dialogue = new Dictionary<string, Dialogue>();

    public Dialogue GetDialogue(string id)
    {
        if(!dict_dialogue.ContainsKey(id))
            return null;
        return dict_dialogue[id];
    }

    public Sprite GetAvator(string id)
    {
        return ResourceController.Controller().Load<Sprite>("Image/Dialogue/Character/"+id);
    }

    public Sprite GetBackground(string id)
    {
        return ResourceController.Controller().Load<Sprite>("Image/Dialogue/Background/"+id);
    }

    public void EnterDialogue(string id)
    {
        GUIController.Controller().ShowPanel<DialoguePanel>("DialoguePanel", 2, (p) =>
        {
            p.dialogue = GetDialogue(id);
        });
    }

    public void InitialData()
    {
        // load dialogue
        Dialogue[] dialogues = Resources.LoadAll<Dialogue>("Object/Dialogue/");
        if(dialogues != null)
        {
            foreach(Dialogue dialogue in dialogues)
            {
                dict_dialogue.Add(dialogue.dialogue_id, dialogue);
            }     
        }
    }
}
