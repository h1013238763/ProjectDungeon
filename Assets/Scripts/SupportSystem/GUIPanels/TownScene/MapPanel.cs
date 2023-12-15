using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapPanel : PanelBase
{
    public override void ShowSelf()
    {
        for(int i = 0; i < 5; i ++)
        {
            FindComponent<Button>("MazeBtn ("+i+")").gameObject.SetActive( i <= PlayerController.Controller().data.maze_progress);
        }

        SetCostumeEvent();
    }
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("EnterMaze");

        if(button_name == "TestMaze")
        {
            MazeController.Controller().TestMaze();
            StageController.Controller().SwitchScene("MazeScene");
        }
        else
        {
            MazeController.Controller().SetMaze(button_name);
            MazeController.Controller().NormalMaze();
            StageController.Controller().SwitchScene("MazeScene");
        }
    }

    public void SetCostumeEvent()
    {
        for(int i = 0; i < 5; i ++)
        {
            GUIController.AddCustomEventListener(FindComponent<Button>("MazeBtn ("+i+")"), EventTriggerType.PointerEnter, (data) => {OnPointerEnter((PointerEventData)data); });
            GUIController.AddCustomEventListener(FindComponent<Button>("MazeBtn ("+i+")"), EventTriggerType.PointerExit,  (data) => {OnPointerExit ((PointerEventData)data); });
        }
    }

    // visual effect
    private void OnPointerEnter(PointerEventData event_data)
    {
        FindComponent<Button>(event_data.pointerEnter.name).gameObject.GetComponent<Outline>().enabled = true;
    }
    private void OnPointerExit(PointerEventData event_data)
    {
        FindComponent<Button>(event_data.pointerEnter.name).gameObject.GetComponent<Outline>().enabled = false;
    }
}
