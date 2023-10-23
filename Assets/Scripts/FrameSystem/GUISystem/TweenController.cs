using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public void MoveToPosition(Transform transform, Vector3 pos, float time, bool localPos = false, TweenType type = TweenType.Normal)
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
    }
    private void IMoveToPosition(TweenAction action)
    {
        float percent = 0;

        switch(action.type)
        {
            case TweenType.Normal:
                percent = action.current_time/action.total_time;
                break;
            default:
                break;
        }

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

    /// <summary>
    /// Change the size of object to target size in time
    /// </summary>
    /// <param name="transform">target object</param>
    /// <param name="size">target size</param>
    /// <param name="time">time limit</param>
    /// <param name="type">animate type</param>
    public void ChangeSizeTo(Transform transform, Vector3 scale, float time, TweenType type = TweenType.Normal)
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
    }
    private void IChangeSizeTo(TweenAction action)
    {
        float percent = 0;

        switch(action.type)
        {
            case TweenType.Normal:
                percent = action.current_time/action.total_time;
                break;
            default:
                break;
        }

        if(percent > 1)
            percent = 1;

        action.target.localScale = action.start_pos + (action.end_pos-action.start_pos)*percent;
    }

    public TweenAction GetTweenAction()
    {  
        if(tween_pool.Count < 1)
            tween_pool.Add(new TweenAction());

        TweenAction temp = tween_pool[tween_pool.Count-1];
        tween_pool.RemoveAt(tween_pool.Count-1);
        return temp;
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
            return current_time >= (total_time*1.1);
        }

        // the animation type of tween
        
    }
}

public enum TweenType
{
    Normal,
    Smooth
}