using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapPanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        int maze_level = Int32.Parse(button_name.Substring( button_name.IndexOf("(")+1, 1 ));
        MazeController.Controller().maze_level = maze_level;
        StageController.Controller().SwitchScene("MazeScene");
    }
}
