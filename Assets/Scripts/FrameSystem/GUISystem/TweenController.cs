using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Tween Controller module
/// </summary>
public class TweenController : BaseController<TweenController>
{   
    private Dictionary<string, TweenAction>              info_list   = new Dictionary<string, TweenAction>();
    private Dictionary<string, UnityAction<TweenAction>> action_list = new Dictionary<string, UnityAction<TweenAction>>();
    private List<TweenAction> remove_action = new List<TweenAction>();

    private Dictionary<TweenAction, UnityAction<TweenAction>> wait_list = new Dictionary<TweenAction, UnityAction<TweenAction>>();
    private List<TweenAction> remove_wait = new List<TweenAction>();

    private List<TweenAction> tween_pool = new List<TweenAction>();

    public TweenController()
    {
        MonoController.Controller().AddUpdateListener(Animating);
    }

    /// <summary>
    /// Animating
    /// </summary>
    private void Animating()
    {
        // animating
        foreach(var pair in info_list)
        {
            // assign infomation
            TweenAction current_action = pair.Value; 
            // use infomation for animating
            action_list[current_action.id].Invoke(current_action);

            // check if animation finished and remove animation
            if(current_action.Finish())
                remove_action.Add(current_action);
        }

        // remove finished animation
        foreach(TweenAction action in remove_action)
        {
            action_list.Remove(action.id);
            info_list.Remove(action.id); 
            tween_pool.Add(action); 
        }
        remove_action.Clear();

        
        // add animation into action list
        foreach(var pair in wait_list)
        {
            if(!info_list.ContainsKey(pair.Key.id))
            {
                info_list.Add(pair.Key.id, pair.Key);
                action_list.Add(pair.Key.id, pair.Value);
                remove_wait.Add(pair.Key);
            }
        }

        foreach(TweenAction action in remove_wait)
        {
            wait_list.Remove(action);
        }
        remove_wait.Clear();
    }

    /// <summary>
    /// Move object to target position in time
    /// </summary>
    /// <param name="transform">target object</param>
    /// <param name="pos">target position</param>
    /// <param name="time">time limit</param>
    /// <param name="type">animate type</param>
    public TweenAction MoveToPosition(Transform transform, Vector3 pos, float time,  bool localPos = false, TweenType type = TweenType.Normal)
    {
        // assign infomation
        TweenAction action = GetTweenAction();
        action.id = transform.gameObject.name;
        action.target = transform;
        if(localPos)
            action.start_pos = transform.localPosition;
        else
            action.start_pos = transform.position;

        action.end_pos = pos;
        action.current_time = 0;
        action.total_time = time;
        action.type = type;
        action.localPos = localPos;

        // assign to dictionary
        wait_list.Add(action, IMoveToPosition);
        return action;
    }
    private void IMoveToPosition(TweenAction action)
    {
        try{
            float percent = 0;

            percent = GetPercentile(action.current_time, action.total_time, action.type);

            if(percent > 1)
                percent = 1;
            
            if(action.localPos)
            {
                action.target.localPosition = action.start_pos + (action.end_pos-action.start_pos)*percent;
            }    
            else
            {
                action.target.position = action.start_pos + (action.end_pos-action.start_pos)*percent;
            }
        }
        catch(System.Exception e)
        {Debug.Log("Exception: id="+action.id+", e="+e);}
    }

    /// <summary>
    /// Change the size of object to target size in time
    /// </summary>
    /// <param name="transform">target object</param>
    /// <param name="size">target size</param>
    /// <param name="time">time limit</param>
    /// <param name="type">animate type</param>
    public TweenAction ChangeSizeTo(Transform transform, Vector3 scale, float time, TweenType type = TweenType.Normal)
    {
        // assign infomation
        TweenAction action = GetTweenAction();
        action.id = transform.gameObject.name;
        action.target = transform;
        action.start_pos = transform.localScale;
        action.end_pos = scale;
        action.current_time = 0;
        action.total_time = time;
        action.type = type;

        // assign to dictionary
        wait_list.Add(action, IChangeSizeTo);
        return action;
    }
    private void IChangeSizeTo(TweenAction action)
    {
        try{
            float percent = 0;

            percent = GetPercentile(action.current_time, action.total_time, action.type);

            if(percent > 1)
                percent = 1;

            action.target.localScale = action.start_pos + (action.end_pos-action.start_pos)*percent;
        }
        catch(System.Exception e)
        {Debug.Log("Exception: id="+action.id+", e="+e);}
    }

    public TweenAction ChangeImageColor(Transform transform, Vector3 color, float time, TweenType type = TweenType.Normal)
    {
        TweenAction action = GetTweenAction();
        action.id = transform.gameObject.name;
        action.target = transform;
        Color start_color = transform.GetComponent<Image>().color;
        action.start_pos = new Vector3(start_color.r, start_color.g, start_color.b);
        action.end_pos = color;
        action.current_time = 0;
        action.total_time = time;
        action.type = type;

        wait_list.Add(action, IChangeImageColor);
        return action;
    }
    private void IChangeImageColor(TweenAction action)
    {
        try{
            float percent = 0;

            percent = GetPercentile(action.current_time, action.total_time, action.type);

            if(percent > 1)
                percent = 1;

            Color new_color = action.target.GetComponent<Image>().color;
            Vector3 color_rgb = action.start_pos + (action.end_pos - action.start_pos ) * percent;
            new_color = new Color(color_rgb.x, color_rgb.y, color_rgb.z, new_color.a);
            action.target.GetComponent<Image>().color = new_color;
        }
        catch(System.Exception e)
        {Debug.Log("Exception: id="+action.id+", e="+e);}
    }

    public TweenAction ChangeAlpha(Transform transform, float alpha, float time, TweenType type = TweenType.Normal)
    {
        TweenAction action = GetTweenAction();
        action.id = transform.gameObject.name;
        action.target = transform;

        action.start_pos.x = transform.GetComponent<CanvasGroup>().alpha;
        action.end_pos.x = alpha;

        action.current_time = 0;
        action.total_time = time;
        action.type = type;

        wait_list.Add(action, IChangeAlpha);
        return action;
    }
    private void IChangeAlpha(TweenAction action)
    {
        try{
            float percent = 0;

            percent = GetPercentile(action.current_time, action.total_time, action.type);

            if(percent > 1)
                percent = 1;
            
            action.target.GetComponent<CanvasGroup>().alpha = action.start_pos.x + (action.end_pos.x - action.start_pos.x )*percent;
        }
        catch(System.Exception e)
        {Debug.Log("Exception: id="+action.id+", e="+e);}
    }

    public void AddEventTrigger(TweenAction action, string trigger)
    {
        if(action == null)
            return;
        action.event_id = trigger;
    }

    private float GetPercentile(float curr_time, float total_time, TweenType type)
    {
        switch(type)
        {
            case TweenType.Normal:
                return (float)curr_time/total_time;
            case TweenType.Smooth:
                return (float)Math.Sin(curr_time/total_time*Math.PI/2);
            default:
                return 1;
        }
    }

    public TweenAction GetTweenAction()
    {  
        if(tween_pool.Count < 1)
            tween_pool.Add(new TweenAction());

        TweenAction temp = tween_pool[tween_pool.Count-1];
        tween_pool.RemoveAt(tween_pool.Count-1);
        return temp;
    }
    
}

public class TweenAction
{
    public string id;
    public Transform target;
    public Vector3 start_pos;
    public Vector3 end_pos;
    public float current_time;
    public float total_time;
    public bool localPos;
    public TweenType type;
    public string event_id;

    /// <summary>
    /// Constructor
    /// </summary>
    public TweenAction(){}

    /// <summary>
    /// check if this animation is finished
    /// </summary>
    /// <returns>true if finished</returns>
    public bool Finish()
    {
        current_time += Time.deltaTime;
            // trigger tween finish event
        if(current_time >= (total_time*1.1) && event_id != null)
        {
            EventController.Controller().EventTrigger(event_id);
            EventController.Controller().RemoveEventKey(event_id);
            event_id = null;
        }
        return current_time >= (total_time*1.1);
    }
    // the animation type of tween
}

public enum TweenType
{
    Normal,
    Smooth
}