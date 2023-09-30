using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 面板基类模块
///     快速查找和添加控件
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
    }

    public virtual void ShowSelf(){}
    public virtual void HideSelf(){}

    protected virtual void OnClick(string button_name){}
    protected virtual void OnValueChanged(string toggle_name, bool is_check){}

    /// <summary>
    /// 获得指定控件
    /// </summary>
    /// <param name="control_name">物体名称</param>
    /// <typeparam name="T">控件类型</typeparam>
    /// <returns>对应控件</returns>
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
    /// 找到所有对应类型的子物体控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    private void FindChildComponent<T>() where T : UIBehaviour
    {
        T[] ctrls = this.GetComponentsInChildren<T>();
 
        for(int i = 0; i < ctrls.Length; i ++){
            string temp = ctrls[i].gameObject.name;
            if(!control_dic.ContainsKey(temp))
                control_dic.Add(temp, new List<UIBehaviour>());
            control_dic[temp].Add(ctrls[i]);
            
            // 添加按钮监听
            if(ctrls[i] is Button)
            {
                (ctrls[i] as Button).onClick.AddListener(() => 
                {
                    OnClick(temp);
                });
            }
            else if( ctrls[i] is Toggle)
            {
                (ctrls[i] as Toggle).onValueChanged.AddListener((is_check) => 
                {
                    OnValueChanged(temp, is_check);
                });
            }

        }
    }
}
