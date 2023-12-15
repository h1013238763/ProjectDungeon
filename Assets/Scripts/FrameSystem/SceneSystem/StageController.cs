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
        // write xml files
        SkillData skill = new SkillData();
        skill.SkillXmlWriter();

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
            Screen.SetResolution(resolutions[config.resolution].width, resolutions[config.resolution].height, config.is_fullscreen);
        }

        // game data initial
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3, (p) =>
        {
            p.loading_action = InitialGame;
        });
    }



    /// <summary>
    /// Game initial events
    /// </summary>
    private void InitialGame()
    {
        // Enemy
        EnemyController.Controller().InitialData();
        // Buff
        BuffController.Controller().InitialData();
        // Player
        PlayerController.Controller().InitialData();
        // Shop
        ShopController.Controller().InitialData(); 
        // Item initial
        ItemController.Controller().InitialData();
        // skill initial
        SkillController.Controller().InitialData();
        // Enchant
        EnchantController.Controller().InitialData();
        // dialogue
        DialogueController.Controller().InitialData();
        // Maze
        MazeController.Controller().InitialData();
         
        // Start Game
        EnterStartScene();
    }

    public void NewGame()
    {
        // Player
        PlayerController.Controller().NewData();
        // Shop
        ShopController.Controller().NewData();
        // Item
        ItemController.Controller().NewData();
        // Skill
        SkillController.Controller().NewData();
        // Enchant
        EnchantController.Controller().NewData();
        
        // Recipe

        // Register Tutorial 
        SwitchScene("TutorialScene");
    }

    public void LoadGame()
    {
        // Player
        PlayerController.Controller().LoadData();
        // Item
        ItemController.Controller().LoadData();
        // Shop
        ShopController.Controller().LoadData();
        // Enchant
        EnchantController.Controller().LoadData();
        // Skill
        SkillController.Controller().LoadData();

        SwitchScene("TownScene");
    }

    public void SaveGame()
    {
        Debug.Log("Save Game");
        // Enchant
        EnchantController.Controller().SaveData();

        ItemController.Controller().SaveData();
        // Player
        PlayerController.Controller().SaveData();
        // Shop
        ShopController.Controller().SaveData();
        // Skill
        SkillController.Controller().SaveData();
    }

    /// <summary>
    /// Handle the actual scene switching event include audio switch, gui change etc
    /// </summary>
    public void SwitchScene(string to_scene)
    {
        // start loading scene
        GUIController.Controller().ShowPanel<LoadingPanel>("LoadingPanel", 3, (p) =>
        {
            p.loading_action = ExitScene;

            if(to_scene == "StartScene")
                    p.loading_action += EnterStartScene;
            else if(to_scene == "TownScene")
                    p.loading_action += EnterTownScene;
            else if(to_scene == "MazeScene")
                    p.loading_action += EnterMazeScene;
            else if(to_scene == "TutorialScene")
                    p.loading_action += EnterTutorialScene;
        });        
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

        // start BGM
        AudioController.Controller().StartMusic("Title");
    }

    private void EnterTutorialScene()
    {
        // change stage
        stage = Stage.Tutorial;

        // Start Dialogue
        DialogueController.Controller().EnterDialogue("Tutorial_0");

        // register dialogue flow and events
        // dialogue 0 : ask player input name
        EventController.Controller().AddEventListener("DialogueFinish:Tutorial_0", () => {
            GUIController.Controller().ShowPanel<NamePanel>("NamePanel", 1);

            EventController.Controller().RemoveEventKey("DialogueFinish:Tutorial_0");
        });

        EventController.Controller().AddEventListener("DialogueFinish:Tutorial_1", () => {
            MazeController.Controller().SetMaze("TutorialMaze");
            MazeController.Controller().TutorialMaze();
            StageController.Controller().SwitchScene("MazeScene");

            EventController.Controller().RemoveEventKey("DialogueFinish:Tutorial_1");
        });

        EventController.Controller().AddEventListener("EnterRoom:TutorialMaze, Complete", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_2");

            EventController.Controller().RemoveEventKey("EnterRoom:TutorialMaze, Complete");
        });

        EventController.Controller().AddEventListener("CompleteRoom:TutorialMaze, Enemy", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_3");

            EventController.Controller().RemoveEventKey("CompleteRoom:TutorialMaze, Enemy");
        });

        EventController.Controller().AddEventListener("EnterRoom:TutorialMaze, Rest", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_4");

            EventController.Controller().RemoveEventKey("EnterRoom:TutorialMaze, Rest");
        });

        EventController.Controller().AddEventListener("EnterRoom:TutorialMaze, Treasure", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_5");

            EventController.Controller().RemoveEventKey("EnterRoom:TutorialMaze, Treasure");
        });

        EventController.Controller().AddEventListener("CompleteRoom:TutorialMaze, Treasure", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_6");

            EventController.Controller().RemoveEventKey("CompleteRoom:TutorialMaze, Treasure");
        });

        EventController.Controller().AddEventListener("DialogueFinish:Tutorial_6", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_7");

            EventController.Controller().RemoveEventKey("DialogueFinish:Tutorial_6");
        });

        EventController.Controller().AddEventListener("EnterTownScene", () => {
            DialogueController.Controller().EnterDialogue("Tutorial_8");

            EventController.Controller().RemoveEventKey("EnterTownScene");
        });

        GUIController.Controller().ShowPanel<PausePanel>("PausePanel", 3);
    }

    private void EnterTownScene()
    {
        if(stage != Stage.Start)
            SaveGame();

        GUIController.Controller().ClearPanel("LoadingPanel");
        // change stage
        stage = Stage.Town;
        // Switch scene
        SceneController.Controller().LoadScene("TownScene");

        // GUI loading
        GUIController.Controller().ShowPanel<TownPanel>("TownPanel", 1);
        GUIController.Controller().ShowPanel<MapPanel>("MapPanel", 1);
        GUIController.Controller().ShowPanel<PausePanel>("PausePanel", 1);

        /// Warning : Code Tester
        CodeTester.Controller().TestCode();

        // loading scene complete
        ShopController.Controller().RefreshShop();
        // start BGM
        AudioController.Controller().StartMusic("Town");

        EventController.Controller().EventTrigger("EnterTownScene");
    }

    private void EnterMazeScene()
    {

        // change stage
        if(stage != Stage.Tutorial)
            stage = Stage.Maze;
        // Switch scene
        SceneController.Controller().LoadScene("MazeScene");

        BattleUnit player_unit = PlayerController.Controller().CreateUnit();
        GUIController.Controller().ShowPanel<UnitPanel>("UnitPanel (Player)", 1, (p) => {
            p.unit = player_unit;
            player_unit.unit_panel = p;
        });
        MazeController.Controller().player_unit = player_unit;
        MazeController.Controller().reward_item = new List<Item>();
        MazeController.Controller().reward_money = 0;
        MazeController.Controller().reward_exp = 0;  

        // GUI loading
        GUIController.Controller().ShowPanel<MazeBackground>("MazeBackground", 0);
        GUIController.Controller().ShowPanel<MazePanel>("MazePanel", 2);
        GUIController.Controller().ShowPanel<PausePanel>("PausePanel", 2);

        // start BGM
        string bgm = MazeController.Controller().maze_base.route_music;
        AudioController.Controller().StartMusic(bgm);
    }

    /// <summary>
    /// Exit Start Scene Steps and Events
    /// </summary>
    private void ExitScene()
    {
        // remove previous gui
        GUIController.Controller().ClearPanel("LoadingPanel");

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