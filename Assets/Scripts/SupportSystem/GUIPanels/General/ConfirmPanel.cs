using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmPanel : PanelBase
{
    public float panel_time;
    public string panel_text;

    private void FixedUpdate() {
        
        if(panel_time > 0)
        {
            panel_time -= Time.deltaTime;
            // change time cound text
            FindComponent<Text>("AcceptText").text = "Accept ("+((int)panel_time).ToString()+")";
            // count finish
            if(panel_time <= 0)
            {
                OnButtonClick("BackBtn");
            }     
        }
    }

    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ButtonClick");

        if(button_name == "AcceptButton")
        {
            EventController.Controller().EventTrigger("ConfirmPanelEvent");
        }
        else
        {
            EventController.Controller().EventTrigger("ConfirmPanelBack");
        }
        
        EventController.Controller().RemoveEventKey("ConfirmPanelEvent");
        EventController.Controller().RemoveEventKey("ConfirmPanelBack");
        GUIController.Controller().RemovePanel("ConfirmPanel");
    }

    public override void ShowSelf()
    {
        FindComponent<Text>("ConfirmText").text = panel_text;
    }

    /// <summary>
    /// Set the panel text and trigger event
    /// </summary>
    /// <param name="text"></param>
    /// <param name="action"></param>
    public void SetPanel(string text, float time = 0)
    {
        Debug.Log(panel_text + ": " + panel_time);
        Debug.Log(FindComponent<Text>("ConfirmText"));
        this.panel_text = text;
        this.panel_time = time;
    }
}
