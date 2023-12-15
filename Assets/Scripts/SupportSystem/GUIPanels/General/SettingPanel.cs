using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class SettingPanel : PanelBase
{
    private SettingConfig config;
    private Resolution[] resolutions;
    private int resolution_index = 0;
    private bool is_inital;

    private float slider_cold = 0;
    public override void ShowSelf()
    {
        ResetSetting();
        DropdownInitial();
        gameObject.SetActive(true);
    }

    public override void HideSelf()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(slider_cold > -1)
        {
            slider_cold -= Time.deltaTime;
            if(slider_cold <= 0)
            {
                slider_cold = -1;
            }
        }
    }

    /// <summary>
    /// player click buttons
    /// </summary>
    /// <param name="button_name">the name of button</param>
    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("ButtonClick");            

        // add confirm event
        if(button_name == "ApplyBtn")
        {
            SaveSettingConfig();
            GUIController.Controller().HidePanel("SettingPanel");
        }
        // close setting panel
        if(button_name == "BackBtn")
        {
            if(StageController.Controller().stage == Stage.Battle)
            {
                GUIController.Controller().GetPanel<BattlePanel>("BattlePanel").PauseTime(false);
            }
            ResetSetting();
            GUIController.Controller().HidePanel("SettingPanel");
        }
        // back to start scene
        if(button_name == "QuitBtn")
        {
            Debug.Log(StageController.Controller().stage);
            
            // in tutorial scene
            if(StageController.Controller().stage == Stage.Tutorial)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    MazeController.Controller().ExitMaze("Fail");
                    StageController.Controller().stage = Stage.Town;
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 2, (p) =>
                {
                    p.SetPanel("Sure to skip the tutorial maze?");
                });
            }
            // in Town Scene
            else if(StageController.Controller().stage == Stage.Town)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    StageController.Controller().SaveGame();
                    StageController.Controller().SwitchScene("StartScene");
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 2, (p) =>
                {
                    p.SetPanel("Sure to exit to title?");
                });
            }
            // in Maze
            else if(StageController.Controller().stage == Stage.Maze)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    MazeController.Controller().ExitMaze("Fail");
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 2, (p) =>
                {
                    p.SetPanel("Return to town and settle current rewards,\nsure to exit the maze?");
                });
            }
            // in Battle
            else if(StageController.Controller().stage == Stage.Battle)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    BattleController.Controller().BattleEnd("Exit");
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 2, (p) =>
                {
                    p.SetPanel("Will lose 1 hope and return to the previous room,\nsure to exit the battle?");
                });
            }
        }
    }

    /// <summary>
    /// volume slider value change
    /// </summary>
    /// <param name="slider_name">the name of slider</param>
    /// <param name="slider_value">the new slider value</param>
    protected override void OnSliderValueChanged(string slider_name, float slider_value)
    {
        // play sound effect
        if(!is_inital && slider_cold <= 0)
        {
            AudioController.Controller().StartSound("SliderDrag");
            slider_cold = 0.1f;
        }
            

        // handle component events
        switch(slider_name)
        {
            // master volume
            case "MasterSlider":
                AudioController.Controller().ChangeMasterVolume(0.01f*slider_value);
                config.master_volume = slider_value;
                FindComponent<Text>("MasterSliderValue").text = slider_value.ToString();
                break;
            // music volume
            case "MusicSlider":
                AudioController.Controller().ChangeMusicVolume(0.01f*slider_value);
                config.music_volume = slider_value;
                FindComponent<Text>("MusicSliderValue").text = slider_value.ToString();
                break;
            // sound volume
            case "SoundSlider":
                AudioController.Controller().ChangeSoundVolume(0.01f*slider_value);
                config.sound_volume = slider_value;
                FindComponent<Text>("SoundSliderValue").text = slider_value.ToString();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// change full screen setting
    /// </summary>
    /// <param name="toggle_name">the name of toggle</param>
    /// <param name="is_check">new toggle value</param>
    protected override void OnToggleValueChanged(string toggle_name, bool is_check)
    {
        if(!is_inital)
            AudioController.Controller().StartSound("ButtonClick");
        // set full screen
        if(toggle_name == "FullScreenToggle")
        {
            if(!is_inital)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    Screen.fullScreen = is_check;
                    config.is_fullscreen = is_check;
                    SaveSettingConfig();
                });
                EventController.Controller().AddEventListener("ConfirmPanelBack", () => 
                {
                    FindComponent<Toggle>("FullScreenToggle").isOn = config.is_fullscreen;
                    FindComponent<Dropdown>("ResolutionDropdown").value = config.resolution;
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 3, (p) => 
                {
                    p.SetPanel("Sure to apply this setting?", 10);
                });
            }
            else
            {
                Screen.fullScreen = is_check;
                config.is_fullscreen = is_check;
            }
            // is_inital = false;
        }
    }

    /// <summary>
    /// change resolution setting
    /// </summary>
    /// <param name="drop_name">dropdown menu name</param>
    /// <param name="drop_value">new choice index</param>
    protected override void OnDropdownValueChanged(string drop_name, int drop_value)
    {
        // play audio
        if(!is_inital)
            AudioController.Controller().StartSound("ButtonClick");

        // dropdown initial
        DropdownInitial();

        if(drop_value < 0)
            drop_value = config.resolution;

        // change resolution
        if(drop_name == "ResolutionDropdown")
        {
            if(!is_inital)
            {
                EventController.Controller().AddEventListener("ConfirmPanelEvent", () => 
                {
                    config.resolution = drop_value;
                    Screen.SetResolution(config.resolution_options[drop_value].width, config.resolution_options[drop_value].height, config.is_fullscreen);
                    FindComponent<Dropdown>("ResolutionDropdown").RefreshShownValue();
                    SaveSettingConfig();
                });
                EventController.Controller().AddEventListener("ConfirmPanelBack", () => 
                {
                    FindComponent<Dropdown>("ResolutionDropdown").value = config.resolution;
                });
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 3, (p) => 
                {
                    p.SetPanel("Sure to apply this setting?", 10);
                });
            }
            else
            {
                config.resolution = drop_value;
                Screen.SetResolution(config.resolution_options[drop_value].width, config.resolution_options[drop_value].height, config.is_fullscreen);
            } 
        }
        is_inital = false;
    }

    /// <summary>
    /// initial the dropdown menu
    /// </summary>
    private void DropdownInitial()
    {
        if(FindComponent<Dropdown>("ResolutionDropdown").options.Count < 1)
        {

            List<string> options = new List<string>();
    
            for(int i = 0; i < config.resolution_options.Length; i ++)
            {
                options.Add( config.resolution_options[i].width + " x " + config.resolution_options[i].height );
                if(config.resolution_options[i].ToString() == Screen.currentResolution.ToString())
                    resolution_index = i;
            }

            Dropdown resolution_drop = FindComponent<Dropdown>("ResolutionDropdown");
            resolution_drop.ClearOptions();
            resolution_drop.AddOptions(options);
            resolution_drop.value = resolution_index;
            resolution_drop.RefreshShownValue();  
        }
        if(config.resolution == -1)
            config.resolution = resolution_index;
    }

    /// <summary>
    /// reset all setting by file
    /// </summary>
    private void ResetSetting()
    {
        // start initial
        is_inital = true;
        // get config file
        LoadSettingConfig();
        // Button setting
        Button quit_btn = FindComponent<Button>("QuitBtn");
        Text quit_text = FindComponent<Button>("QuitBtn").transform.GetChild(0).gameObject.GetComponent<Text>();

        switch(StageController.Controller().stage)
        {
            case Stage.Start :
                quit_btn.gameObject.SetActive(false);
                break;
            case Stage.Tutorial:
                quit_btn.gameObject.SetActive(true);
                quit_text.text = "Skip";
                break;
            
            case Stage.Town:
                quit_btn.gameObject.SetActive(true);
                quit_text.text = "Exit to title";
                break;
            // TODO : quit maze
            case Stage.Maze:
                quit_btn.gameObject.SetActive(true);
                quit_text.text = "Exit Maze";
                break;
            // TODO : quit battle
            case Stage.Battle:
                quit_btn.gameObject.SetActive(true);
                quit_text.text = "Exit Battle";
                break;
            default:
                break;
        }           
        // volume setting
        FindComponent<Slider>("MasterSlider").value = config.master_volume;
        FindComponent<Slider>("MusicSlider").value = config.music_volume;
        FindComponent<Slider>("SoundSlider").value = config.sound_volume;
        // Toggle setting
        FindComponent<Toggle>("FullScreenToggle").isOn = config.is_fullscreen;
        // resolution setting
        FindComponent<Dropdown>("ResolutionDropdown").value = config.resolution;  
    }

    /// <summary>
    /// try load exist setting config, create new if not exist and return
    /// </summary>
    private void LoadSettingConfig()
    {
        // try load exist setting config file
        config = XmlController.Controller().LoadData(typeof(SettingConfig), "Config") as SettingConfig;
        // if file not exist, create new setting
        if( config == null )
        {
            config = new SettingConfig();
        }
    }

    /// <summary>
    /// save current setting config as file
    /// </summary>
    private void SaveSettingConfig()
    {
        XmlController.Controller().SaveData(config, "Config");
    }
}

/// <summary>
/// Inner Class to store setting config info
/// </summary>
public class SettingConfig
{
    public float master_volume;
    public float music_volume;
    public float sound_volume;
    public bool is_fullscreen;
    public int resolution;
    public Resolution[] resolution_options;

    public SettingConfig()
    {
        master_volume = 100;
        music_volume = 100;
        sound_volume = 100;
        is_fullscreen = true;
        resolution = -1;
        resolution_options = Screen.resolutions;
    }

    public override string ToString()
    {
        return "config [v1: "+master_volume+", v2: "+music_volume+", v3: "+sound_volume+", if: "+is_fullscreen+", re: "+resolution+"]";
    }
}