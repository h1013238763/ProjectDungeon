using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class StartPanel : PanelBase
{    
    protected override void Awake()
    {
        base.Awake();
    }

    public override void ShowSelf()
    {
        // print game version
        FindComponent<Text>("VersionText").text = "Version: " + Application.version;
        // show buttons
        if(PlayerController.Controller().data == null)
            FindComponent<Button>("ContinueBtn").interactable = false;
        FindComponent<Image>("ComponentGrid").gameObject.SetActive(true);

    }

    /// <summary>
    /// trigger the button click event of start panel
    /// </summary> 
    /// <param name="button_name">the name of button element</param>
    protected override void OnButtonClick(string button_name)
    {
        // player sound effect
        AudioController.Controller().StartSound("ButtonClick");

        // call functions
        switch(button_name)
        {
            case "StartBtn": 
                if(PlayerController.Controller().data != null)
                {
                    EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                    {
                        StageController.Controller().NewGame();
                    });
                    GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 3, (p) => 
                    {
                        p.SetPanel("This will overwrite the previous game record. Sure to create a new game?");
                    });
                }
                else
                {
                    StageController.Controller().NewGame();
                }
                break;
            case "ContinueBtn":
                StageController.Controller().LoadGame();
                break;
            case "SettingBtn": // move to setting menu
                GUIController.Controller().ShowPanel<SettingPanel>("SettingPanel", 2);
                break;
            case "QuitBtn":    // quit game
                Application.Quit();
                break;
        }
    }
}