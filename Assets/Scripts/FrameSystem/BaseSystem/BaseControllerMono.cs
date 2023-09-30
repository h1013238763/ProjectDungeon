using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  singleton pattern base class module with MonoBehaviour
/// </summary>
/// <typeparam name="T"> Child Class </typeparam>
public class BaseControllerMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T controller;

    /// <summary>
    /// Create an game object to which contains the script, 
    /// define and return a unique static controller
    /// </summary>
    /// <returns></returns>
    public static T Controller()
    {
        if(controller == null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).ToString();
            controller = obj.AddComponent<T>();

            GameObject.DontDestroyOnLoad(obj);
        }
        return controller;
    }


}
