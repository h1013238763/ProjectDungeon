using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainPanel : PanelBase
{
    // panel variables
    bool time_limit = false;
    bool health_limit = false;
    bool point_limit = false;
    bool potion_limit = false;
    bool enemy_action = false;
    bool dev_mode = false;

    public override void ShowSelf()
    {
        
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name == "TimeLimitBtn")
        {
            time_limit = !time_limit;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "time limit ( "+time_limit+" )";
        }
        else if(button_name == "HealthLimitBtn")
        {
            health_limit = !health_limit;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "health limit ( "+time_limit+" )";
        }
        else if(button_name == "PointLimitBtn")
        {
            point_limit = !point_limit;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "action point limit ( "+time_limit+" )";
        }
        else if(button_name == "TPotionLimitBtn")
        {
            potion_limit = !potion_limit;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "potion limit ( "+time_limit+" )";
        }
        else if(button_name == "EnemyActionBtn")
        {
            enemy_action = !enemy_action;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "enemy action ( "+time_limit+" )";
        }
        else if(button_name == "DevModeBtn")
        {
            dev_mode = !dev_mode;
            FindComponent<Button>(button_name).transform.GetChild(0).GetComponent<Text>().text = "dev mode ( "+time_limit+" )";
        }
    }

    
}
