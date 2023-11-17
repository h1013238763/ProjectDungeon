using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manage all game stages like scene switching, save loading etc
/// </summary>
public class StageController : BaseControllerMono<StageController>
{
    public Stage stage;
    void Start()
    {
        SceneController.Controller().AddDontDestroy(gameObject);

        // Windows Setting
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

        // game data initial
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3);
        EventController.Controller().AddEventListener("LoadingAnimeComplete", InitialGame);
    }



    /// <summary>
    /// Game initial events
    /// </summary>
    private void InitialGame()
    {
        // skill initial
        SkillController.Controller().InitialData();
        // Item initial
        ItemController.Controller().InitialData();    
        // Shop
        ShopController.Controller().InitialData(); 
        


        // Enemy initial
        // EnemyController.Controller();

        

        EnterStartScene();
    }

    public void NewGame()
    {
        PlayerController.Controller().NewData();
        // Skill
        SkillController.Controller().NewData();

        // Item
        ItemController.Controller().NewData();
        // Shop
        ShopController.Controller().NewData();
        // Recipe



        

        // TODO : switch to tutorial scene
        SwitchScene("TownScene");
    }

    public void LoadGame()
    {
        PlayerController.Controller().LoadData();
        // Item
        ItemController.Controller().LoadData();
        // Shop
        ShopController.Controller().LoadData();

        

        SwitchScene("TownScene");
    }

    public void SaveGame()
    {
        PlayerController.Controller().SaveData();
        ItemController.Controller().SaveData(); 
    }

    /// <summary>
    /// Handle the actual scene switching event include audio switch, gui change etc
    /// </summary>
    public void SwitchScene(string to_scene)
    {
        

        // start loading scene
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3);

        // trigger exit scene event
        EventController.Controller().AddEventListener("LoadingAnimeComplete", ExitScene);

        // trigger enter scene event after loading scene
        switch(to_scene)
        {
            case "StartScene":
                EventController.Controller().AddEventListener("ExitSceneComplete", EnterStartScene);
                break;
            case "TownScene":
                EventController.Controller().AddEventListener("ExitSceneComplete", EnterTownScene);
                break;
            case "MazeScene":
                EventController.Controller().AddEventListener("ExitSceneComplete", EnterMazeScene);
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
        // reset event trigger
        EventController.Controller().RemoveEventKey("ExitSceneComplete");
    }

    private void EnterTutorialScene()
    {
        // change stage
        stage = Stage.Tutorial;
        // Switch Scene
        SceneController.Controller().LoadScene("TutorialScene");

        // Enter Tutorial Maze
        

        // Start Dialogue
        DialogueController.Controller().EnterDialogue("Tutorial_1");

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

        /// Warning : Code Tester
        CodeTester.Controller().TestCode();
        // loading scene complete
        ShopController.Controller().RefreshShop();

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

        // reset event trigger
        EventController.Controller().RemoveEventKey("ExitSceneComplete");
    }

    /// <summary>
    /// Exit Start Scene Steps and Events
    /// </summary>
    private void ExitScene()
    {
        // Stop BGM
        AudioController.Controller().StopMusic();

        // remove previous gui
        GUIController.Controller().ClearPanel("LoadingPanel");

        EventController.Controller().EventTrigger("ExitSceneComplete");
    }
}

public enum Stage
{
    Start,
    Tutorial,
    Town,
    Maze,
    Battle
}