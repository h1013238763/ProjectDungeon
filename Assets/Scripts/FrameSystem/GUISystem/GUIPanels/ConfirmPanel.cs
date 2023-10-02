using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmPanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ButtonClick");

        if(button_name == "AcceptButton")
            EventController.Controller().EventTrigger("ConfirmPanelEvent");
        else
            EventController.Controller().RemoveEventKey("ConfirmPanelEvent");

        GUIController.Controller().RemovePanel("ConfirmPanel");
    }

    public override void ShowSelf()
    {
        
    }

    /// <summary>
    /// Set the panel text and trigger event
    /// </summary>
    /// <param name="text"></param>
    /// <param name="action"></param>
    public void SetPanel(string text)
    {
        FindComponent<Text>("ConfirmText").text = text;
    }
}
