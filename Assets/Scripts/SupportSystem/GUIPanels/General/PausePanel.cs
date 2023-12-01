using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ButtonClick");
        
        if(StageController.Controller().stage == Stage.Battle)
        {
            GUIController.Controller().GetPanel<BattlePanel>("BattlePanel").PauseTime(true);
        }

        if(button_name == "SettingBtn")
        {
            if(GUIController.Controller().GetPanel<SettingPanel>("SettingPanel") == null)
                GUIController.Controller().ShowPanel<SettingPanel>("SettingPanel", 2);
        }
        else if(button_name == "QuestBtn")
        {
            if(GUIController.Controller().GetPanel<QuestPanel>("QuestPanel") == null)
                GUIController.Controller().ShowPanel<QuestPanel>("QuestPanel", 2);
        }
    }
}
