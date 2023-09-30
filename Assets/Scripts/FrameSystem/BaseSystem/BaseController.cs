using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton pattern base class module
/// </summary>
/// <typeparam name="T"> Child Class </typeparam>
public class BaseController<T> where T: new()
{
    private static T controller;

    /// <summary>
    /// Defines and returns a unique static controller
    /// </summary>
    /// <returns></returns>
    public static T Controller(){
        if(controller == null)
            controller = new T();
        return controller;
    }
}