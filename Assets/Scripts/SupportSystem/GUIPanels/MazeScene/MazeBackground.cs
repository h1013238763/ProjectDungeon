using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MazeBackground : PanelBase
{
    public override void ShowSelf()
    {
        FindComponent<Image>("MazeImage").sprite = MazeController.Controller().maze_base.background;
    }
}