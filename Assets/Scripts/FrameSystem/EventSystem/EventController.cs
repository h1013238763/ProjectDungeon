using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event Center Module
/// </summary>
public class EventController : BaseController<EventController>
{
    // Create the listener list
    private Dictionary<string, IEventInfo> event_dic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// Add event listener
    /// </summary>
    /// <param name="name"> Event Name </param>
    /// <param name="action"> Trigger function </param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        // Search target listener
        if(event_dic.ContainsKey(name))
            (event_dic[name] as EventInfo<T>).actions += action;
        else
            event_dic.Add(name, new EventInfo<T>( action ));
    }

    /// <summary>
    /// Add no-parameter event listener
    /// </summary>
    /// <param name="name"> Event Name </param>
    /// <param name="action"> Trigger function </param>
    public void AddEventListener(string name, UnityAction action)
    {
        // Search target listener
        if(event_dic.ContainsKey(name))
            (event_dic[name] as EventInfo).actions += action;
        else
            event_dic.Add(name, new EventInfo( action ));
    }

    /// <summary>
    /// Event trigger
    /// </summary>
    /// <param name="name">Event Name</param>
    public void EventTrigger<T>(string name, T info)
    {
        // Search target listener
        if(event_dic.ContainsKey(name))
        {
            if((event_dic[name] as EventInfo<T>).actions != null)
                (event_dic[name] as EventInfo<T>).actions.Invoke(info);
        }
    }

    /// <summary>
    /// No-parameter event triggered
    /// </summary>
    /// <param name="name">Event Name</param>
    public void EventTrigger(string name)
    {
        // Search target listener
        if(event_dic.ContainsKey(name))
        {
            if((event_dic[name] as EventInfo).actions != null)
                (event_dic[name] as EventInfo).actions.Invoke();
        }
    }

    /// <summary>
    /// Remove event listener
    /// </summary>
    /// <param name="name"> Event Name </param>
    /// <param name="action"> Trigger function </param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if(event_dic.ContainsKey(name))
            (event_dic[name] as EventInfo<T>).actions-= action;
    }

    /// <summary>
    /// Remove no-parameter event listener
    /// </summary>
    /// <param name="name"> Event Name </param>
    /// <param name="action"> Trigger function </param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if(event_dic.ContainsKey(name))
            (event_dic[name] as EventInfo).actions-= action;
    }

    /// <summary>
    /// Clear all event listeners
    /// </summary>
    public void Clear()
    {
        event_dic.Clear();
    }
}

/// <summary>
/// Interfaces and classes for packaging
/// </summary>
public interface IEventInfo{}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo( UnityAction<T> action )
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo( UnityAction action )
    {
        actions += action;
    }
}