using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// control module
/// </summary>
public class GUIController : BaseController<GUIController>
{
    private Dictionary<string, PanelBase> panel_dic = new Dictionary<string, PanelBase>();

    public Transform canvas;
    private int canvas_layer_count = 0;

    public GUIController()
    {
        // Create Canvas Object
        GameObject temp_canvas = GameObject.Find("Canvas");
        if(temp_canvas == null)
            temp_canvas = ResourceController.Controller().Load<GameObject>("GUI/Core/Canvas");
        temp_canvas.name = "Canvas";
        canvas = temp_canvas.transform;
        GameObject.DontDestroyOnLoad(temp_canvas);
        
        // Create Unity Event System
        GameObject temp_event = GameObject.Find("EventSystem");
        if(temp_event == null)
            temp_event = ResourceController.Controller().Load<GameObject>("GUI/Core/EventSystem");
        temp_event.name = "EventSystem";
        GameObject.DontDestroyOnLoad(temp_event);

        foreach( Transform child in canvas)
            canvas_layer_count ++;
    }

    /// <summary>
    /// Show Panel
    /// </summary>
    /// <param name="panel_name">name of panel</param>
    /// <param name="layer">to which layer</param>
    /// <param name="callback">calling function after panel shows</param>
    /// <typeparam name="T">type of panel</typeparam>
    public void ShowPanel<T>(string panel_name, int layer, UnityAction<T> callback = null) where T : PanelBase
    {
        // Avoid Panel repeat create
        if(panel_dic.ContainsKey(panel_name))
        {
            panel_dic[panel_name].ShowSelf();
            if(callback != null)
                callback(panel_dic[panel_name] as T);
            return;
        }

        // Avoid layer out of bound
        if(layer >= canvas_layer_count){
            return;
        }

        // LoadAsync panel, set layer and position
        ResourceController.Controller().LoadAsync<GameObject>("GUI/Panels/" + panel_name, (obj) =>
        {
            obj.transform.SetParent(canvas.GetChild(layer));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.name = panel_name;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            T panel = obj.GetComponent<T>();
            // invoke he call back event
            if(callback != null)
                callback(panel);
            panel.ShowSelf();
            panel_dic.Add(panel_name, panel);
        });
    }

    /// <summary>
    /// Hide Panel
    /// </summary>
    /// <param name="panel_name">the name of panel</param>
    public void HidePanel(string panel_name)
    {
        if(panel_dic.ContainsKey(panel_name))
            panel_dic[panel_name].HideSelf();
    }

    /// <summary>
    /// Remove Panel from scene
    /// </summary>
    /// <param name="panel_name">the name of panel</param>
    public void RemovePanel(string panel_name)
    {
        if(panel_dic.ContainsKey(panel_name))
        {
            GameObject.Destroy(panel_dic[panel_name].gameObject);
            panel_dic.Remove(panel_name);
        }
    }

    /// <summary>
    ///  Get a exist panel
    /// </summary>
    /// <param name="panel_name">the name of panel</param>
    /// <returns>target panel</returns>
    public T GetPanel<T>(string panel_name) where T : PanelBase
    {
        if(panel_dic.ContainsKey(panel_name))
            return panel_dic[panel_name] as T;
        return null;
    }

    /// <summary>
    /// Add a custom event trigger to gui object from scripts
    /// </summary>
    /// <param name="control">the gui object</param>
    /// <param name="type">the type of trigger event</param>
    /// <param name="callback">the function to call</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if(trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);

        trigger.triggers.Add(entry);
    }
}
