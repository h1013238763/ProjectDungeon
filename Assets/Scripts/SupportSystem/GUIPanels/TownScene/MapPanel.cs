using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapPanel : PanelBase
{
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("EnterMaze");

        if(button_name == "TestMaze")
        {
            MazeController.Controller().TestMaze();
            BattleController.Controller().train_mode = true;
            StageController.Controller().SwitchScene("MazeScene");
        }
        else
        {
            BattleController.Controller().train_mode = false;
            MazeController.Controller().maze_base = ResourceController.Controller().Load<Maze>("Objects/Maze/"+button_name);
            MazeController.Controller().NormalMaze();
            StageController.Controller().SwitchScene("MazeScene");
        }
        
    }
}
