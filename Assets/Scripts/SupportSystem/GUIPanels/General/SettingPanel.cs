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

    /// <summary>
    /// player click buttons
    /// </summary>
    /// <param name="button_name">the name of button</param>
    protected override void OnButtonClick(string button_name)
    {
        if(!is_inital)
            AudioController.Controller().StartSound("ButtonClick");

        switch(button_name)
        {
            // add confirm event
            case "ApplyBtn": 
                SaveSettingConfig();
                GUIController.Controller().HidePanel("SettingPanel");
                break;
            // close setting panel
            case "BackBtn":
                ResetSetting();
                GUIController.Controller().HidePanel("SettingPanel");
                break;
            // back to start scene
            case "QuitBtn": 
                GUIController.Controller().ShowPanel<ConfirmPanel>("ConfirmPanel", 3, (p) =>
                {
                    p.SetPanel("Unsaved game content will be lost,\nsure to return to the start menu?");
                });
                break;
            default:
                break;
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
        if(!is_inital)
            AudioController.Controller().StartSound("SliderDrag");
        // set volume text
        // FindComponent<Text>(slider_name+"Value").text = (slider_value).ToString();
        
        // handle component events
        switch(slider_name)
        {
            // master volume
            case "MasterVolume":
                AudioController.Controller().ChangeMasterVolume(slider_value/100);
                config.master_volume = slider_value;
                break;
            // music volume
            case "MusicVolume":
                AudioController.Controller().ChangeMusicVolume(slider_value/100);
                config.music_volume = slider_value;
                break;
            // sound volume
            case "SoundVolume":
                AudioController.Controller().ChangeSoundVolume(slider_value/100);
                config.sound_volume = slider_value;
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
        if(toggle_name == "FullscreenToggle")
        {
            Screen.fullScreen = is_check;
            config.is_fullscreen = is_check;
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
            config.resolution = drop_value;
            Screen.SetResolution(resolutions[drop_value].width, resolutions[drop_value].height, config.is_fullscreen);
        }
    }

    /// <summary>
    /// initial the dropdown menu
    /// </summary>
    private void DropdownInitial()
    {
        if(FindComponent<Dropdown>("ResolutionDropdown").options.Count < 1)
        {
            resolutions = Screen.resolutions;

            List<string> options = new List<string>();
    
            for(int i = 0; i < resolutions.Length; i ++)
            {
                options.Add( resolutions[i].width + " x " + resolutions[i].height );
                if(resolutions[i].ToString() == Screen.currentResolution.ToString())
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
        switch(StageController.Controller().stage)
        {
            case Stage.Start :
                FindComponent<Button>("QuitBtn").gameObject.SetActive(false);
                break;
            default:
                break;
        }           
        // volume setting
        OnSliderValueChanged("MasterVolume", config.master_volume);
        OnSliderValueChanged("MusicVolume", config.music_volume);
        OnSliderValueChanged("SoundVolume", config.sound_volume);
        // resolution setting
        OnDropdownValueChanged("ResolutionDropdown", config.resolution);
        // Toggle setting
        OnToggleValueChanged("FullScreenToggle", config.is_fullscreen);

        // finish initial
        is_inital = false;
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

    public SettingConfig()
    {
        master_volume = 100;
        music_volume = 100;
        sound_volume = 100;
        is_fullscreen = true;
        resolution = -1;
    }
}