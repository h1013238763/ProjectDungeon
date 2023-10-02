using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Panel Base Module
///     Quick Add and Search Components
/// </summary>
public class PanelBase : MonoBehaviour
{

    private Dictionary<string, List<UIBehaviour>> control_dic = new Dictionary<string, List<UIBehaviour>>();
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        FindChildComponent<Button>();
        FindChildComponent<Image>();
        FindChildComponent<Text>();
        FindChildComponent<Toggle>();
        FindChildComponent<Slider>();
        FindChildComponent<ScrollRect>();
        FindChildComponent<InputField>();
        FindChildComponent<Dropdown>();
    }

    public virtual void ShowSelf(){}
    public virtual void HideSelf(){}

    protected virtual void OnButtonClick(string button_name){}
    protected virtual void OnSliderValueChanged(string Slider_name, float value){}
    protected virtual void OnToggleValueChanged(string toggle_name, bool is_check){}
    protected virtual void OnDropdownValueChanged(string drop_name, int value){}
    

    /// <summary>
    /// Find Target Child Object
    /// </summary>
    /// <param name="control_name">Object name</param>
    /// <typeparam name="T">Object type</typeparam>
    /// <returns>target Object</returns>
    protected T FindComponent<T>(string control_name) where T : UIBehaviour
    {
        if(control_dic.ContainsKey(control_name))
        {
            for(int i = 0; i < control_dic[control_name].Count; i ++)
            {
                if(control_dic[control_name][i] is T)
                    return control_dic[control_name][i] as T;
            }
            return null;
        }
        else{
            return null;  
        }
    }

    /// <summary>
    /// Initial all target type child component
    /// </summary>
    /// <typeparam name="T">type of component</typeparam>
    private void FindChildComponent<T>() where T : UIBehaviour
    {
        T[] ctrls = this.GetComponentsInChildren<T>();
 
        for(int i = 0; i < ctrls.Length; i ++){
            string temp = ctrls[i].gameObject.name;
            if(!control_dic.ContainsKey(temp))
                control_dic.Add(temp, new List<UIBehaviour>());
            control_dic[temp].Add(ctrls[i]);
            
            // Add Button Listener
            if(ctrls[i] is Button)
            {
                (ctrls[i] as Button).onClick.AddListener(() => 
                {
                    OnButtonClick(temp);
                });
            }
            // Add Slider Listener
            else if(ctrls[i] is Slider)
            {
                (ctrls[i] as Slider).onValueChanged.AddListener((value) =>
                {
                    OnSliderValueChanged(temp, value);
                });
            }
            // Add Toggle Listener
            else if(ctrls[i] is Toggle)
            {
                (ctrls[i] as Toggle).onValueChanged.AddListener((check) =>
                {
                    OnToggleValueChanged(temp, check);
                });
            }
            // Add Dropdown Listener
            else if(ctrls[i] is Dropdown)
            {
                (ctrls[i] as Dropdown).onValueChanged.AddListener((value) =>
                {
                    OnDropdownValueChanged(temp, value);
                });
            }
        }
    }
}
