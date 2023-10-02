using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manage all game stages like scene switching, save loading etc
/// </summary>
public class StageController : BaseControllerMono<StageController>
{
    public SaveDate save_data;
    public Stage stage;
    void Start()
    {
        SceneController.Controller().AddDontDestroy(gameObject);
        GameInitial();
    }

    private void GameInitial()
    {
        // read setting config
        SettingConfig config = XmlController.Controller().LoadData(typeof(SettingConfig), "Config") as SettingConfig;
        if( config == null )
            config = new SettingConfig();
        // set volume
        AudioController.Controller().ChangeMasterVolume(config.master_volume/100);
        AudioController.Controller().ChangeMusicVolume(config.music_volume/100);
        AudioController.Controller().ChangeSoundVolume(config.sound_volume/100);
        // set resolution
        Resolution[] resolutions = Screen.resolutions;
        if(config.resolution == -1)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, config.is_fullscreen);
        else
            Screen.SetResolution(resolutions[config.resolution].width, resolutions[config.resolution].height, config.is_fullscreen);

        // start loading panel

        // read player saves
        save_data = XmlController.Controller().LoadData(typeof(SaveDate), "save01") as SaveDate;

        // regist item/equip/potion dictionary

        // regist enemy dictionary

        // regist skill dictionary

        // enter start scene
        EnterStartScene();
    }

    private void EnterStartScene()
    {
        // change stage
        stage = Stage.Start;
        
        AudioController.Controller().StartMusic("StartSceneMusic");
        GUIController.Controller().ShowPanel<StartPanel>("StartPanel", 1);
    }
}

public class SaveDate
{
    public int level;
    public int experience;
    
    // new player data
    public SaveDate()
    {
        level = 1;
        experience = 0;
    }
}

public enum Stage
{
    Start,
    Town,
    Maze,
    Combat
}