using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene Changing module
/// </summary>
public class SceneController : BaseController<SceneController>
{
    private Dictionary<string, GameObject> object_ddol = new Dictionary<string, GameObject>();

    /// <summary>
    /// Change Scene
    /// </summary>
    /// <param name="name">name of scene</param>
    /// <param name="action">The action after finish loading</param>
    public void LoadScene(string name, UnityAction action = null)
    {
        SceneManager.LoadScene(name);

        if(action != null)
            action.Invoke();
    }

    /// <summary>
    /// Change Scene Async
    /// </summary>
    /// <param name="name">name of scene</param>
    /// <param name="action">The action after finish loading</param>
    public void LoadSceneAsync(string name, UnityAction action = null)
    {
        MonoController.Controller().StartCoroutine(ILoadSceneAsync(name, action));
    }

    /// <summary>
    /// The Coroutine function
    /// </summary>
    /// <param name="name">name of scene</param>
    /// <param name="action">The action after finish loading</param>
    /// <returns></returns>
    private IEnumerator ILoadSceneAsync(string name, UnityAction action = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        while(operation.isDone)
        {
            EventController.Controller().EventTrigger("Refresh Progress", operation.progress);
            yield return operation.progress;
        }

        if(action != null)
            action();
    }

    public void AddDontDestroy(GameObject obj)
    {
        if(!object_ddol.ContainsKey(obj.name))
        {
            GameObject.DontDestroyOnLoad(obj);
            object_ddol.Add(obj.name, obj);
        }
    }

    public GameObject GetDontDestroy(string name)
    {
        if(object_ddol.ContainsKey(name))
            return object_ddol[name];
        return null;
    }

    public bool ContainsObject(string name)
    {
        return object_ddol.ContainsKey(name);
    }
}
