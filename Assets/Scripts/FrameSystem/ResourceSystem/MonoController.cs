using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// MonoBehavior controller module
/// </summary>
public class MonoController : BaseControllerMono<MonoController>
{
    private event UnityAction update_event;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(update_event != null)
            update_event.Invoke();
    }

    /// <summary>
    /// Add Update invoke event
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        update_event += fun;
    }

    /// <summary>
    /// Remove Update invoke event
    /// </summary>
    public void RemoveUpdateListener(UnityAction fun)
    {
        update_event -= fun;
    }
}
