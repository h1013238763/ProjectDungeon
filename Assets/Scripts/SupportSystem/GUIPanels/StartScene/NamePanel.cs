using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
        {
            PlayerController.Controller().data.player_name = FindComponent<Text>("NameText").text;
            GUIController.Controller().RemovePanel("NamePanel");
            DialogueController.Controller().EnterDialogue("Tutorial_1");
        });

        GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 2, (p) =>
        {
            p.SetPanel("It is your name?");
        });
    }
}
