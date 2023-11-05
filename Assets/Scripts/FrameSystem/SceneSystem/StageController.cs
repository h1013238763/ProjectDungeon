using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manage all game stages like scene switching, save loading etc
/// </summary>
public class StageController : BaseControllerMono<StageController>
{
    public SaveData save_data;
    public Stage stage;
    void Start()
    {
        SceneController.Controller().AddDontDestroy(gameObject);
        GameInitial();
    }

    /// <summary>
    /// Game initial events
    /// </summary>
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

        if(config.resolution < 0 || config.resolution >= resolutions.Length)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, config.is_fullscreen);
        else
        {
            Debug.Log(config.resolution);
            Screen.SetResolution(resolutions[config.resolution].width, resolutions[config.resolution].height, config.is_fullscreen);
        }
        // start loading panel
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3);

        // read player saves
        save_data = XmlController.Controller().LoadData(typeof(SaveData), "save01") as SaveData;

        // regist item/equip/potion dictionary

        // enter start scene
        EventController.Controller().AddEventListener("LoadingAnimeComplete", EnterStartScene);

        // Enemy initial
        // EnemyController.Controller();
    }

    /// <summary>
    /// Handle the actual scene switching event include audio switch, gui change etc
    /// </summary>
    public void SwitchScene(string from_scene, string to_scene)
    {
        // start loading scene
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3);

        AudioController.Controller().StopMusic();

        // trigger exit scene event
        switch(from_scene)
        {
            case "StartScene":
                EventController.Controller().AddEventListener("LoadingAnimeComplete", ExitStartScene);
                break;
            case "TutorialScene":
                break;
            default:
                break;
        }

        // trigger enter scene event after loading scene
        switch(to_scene)
        {
            case "StartScene":
                EventController.Controller().AddEventListener("ExitSceneComplete", EnterStartScene);
                break;
            case "TownScene":
                EventController.Controller().AddEventListener("ExitSceneComplete", EnterTownScene);
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// Enter Start Scene Steps and Events
    /// </summary>
    private void EnterStartScene()
    {
        // change stage
        stage = Stage.Start;  
        // Switch scene
        SceneController.Controller().LoadScene("StartScene");

        // GUI loading
        GUIController.Controller().ShowPanel<StartPanel>("StartPanel", 1);

        // loading scene complete
        GUIController.Controller().HidePanel("LoadingPanel");
        // start BGM
        AudioController.Controller().StartMusic("StartSceneMusic");
    }

    /// <summary>
    /// Exit Start Scene Steps and Events
    /// </summary>
    private void ExitStartScene()
    {
        // Stop BGM
        AudioController.Controller().StopMusic();

        // remove previous gui
        GUIController.Controller().RemovePanel("StartPanel");

        EventController.Controller().EventTrigger("ExitSceneComplete");
    }

    private void EnterTutorialScene()
    {
        
    }

    private void EnterTownScene()
    {
        // change stage
        stage = Stage.Town;
        // Switch scene
        SceneController.Controller().LoadScene("TownScene");

        // GUI loading
        GUIController.Controller().ShowPanel<TownPanel>("TownPanel", 1);
        GUIController.Controller().ShowPanel<MapPanel>("MapPanel", 1);

        // loading scene complete
        
        // start BGM
        AudioController.Controller().StartMusic("StartSceneMusic");

        GUIController.Controller().HidePanel("LoadingPanel");

        // reset event trigger
        EventController.Controller().RemoveEventKey("ExitSceneComplete");
    }

    private void EnterMazeScene()
    {

        // change stage
        stage = Stage.Maze;
        // Switch scene
        SceneController.Controller().LoadScene("MazeScene");

        // Generate Maze
        MazeController.Controller().MazeGenerator();

        // GUI loading
        GUIController.Controller().ShowPanel<MazePanel>("MazePanel", 1, (p) => 
        {
            p.SetMaze(MazeController.Controller().maze);
            p.start_pos = MazeController.Controller().start_pos;
        });

        // loading scene complete
        GUIController.Controller().HidePanel("LoadingPanel");
        // start BGM
        AudioController.Controller().StartMusic("StartSceneMusic");
    }
}

public class SaveData
{
    public int level;                                   // Player level
    public int experience;                              // Player current experience

    // Item related data
    public int money;                                   // player money owned
    public List<Equip> invent_equip;                    // player equipment inventory
    public XmlDictionary<Item, int> invent_item;        // player item inventory
    public XmlDictionary<Potion, int> invent_potion;    // player potion inventory

    // new player data
    public SaveData()
    {
        level = 1;
        experience = 0;

        money = 0;
        invent_equip = new List<Equip>();
        invent_item = new XmlDictionary<Item, int>();
        invent_potion = new XmlDictionary<Potion, int>();
    }
}

public enum Stage
{
    Start,
    Town,
    Maze,
    Battle
}