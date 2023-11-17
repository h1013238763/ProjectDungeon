using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : BaseController<DialogueController>
{
    public Dialogue DialogueInfo(string id)
    {
        return ResourceController.Controller().Load<Dialogue>("Objects/Dialogue/"+id);
    }

    public void EnterDialogue(string id)
    {
        GUIController.Controller().ShowPanel<DialoguePanel>("DialoguePanel", 3, (p) =>
        {
            p.dialogue = DialogueInfo(id);
        });
    }
}
