using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Resource Loading Module
/// </summary>
public class ResourceController : BaseController<ResourceController>
{
    /// <summary>
    /// Load Resource
    /// </summary>
    /// <param name="name">name of resource</param>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);

        // if it's a GameObject, instantiate and return it
        if( res is GameObject )
            return GameObject.Instantiate(res);
        else
            return res;
    }

    /// <summary>
    /// Load Resource Async
    /// </summary>
    /// <param name="name">name of resource</param>
    /// <param name="callback"> action after loading </param>
    /// <typeparam name="T"></typeparam>
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        MonoController.Controller().StartCoroutine(ILoadAsync(name, callback));
    }

    /// <summary>
    /// The Coroutine Function
    /// </summary>
    /// <param name="name">name of resource</param>
    /// <param name="callback"> action after loading </param>
    /// <returns></returns>
    private IEnumerator ILoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if( r.asset is GameObject )
            callback( GameObject.Instantiate(r.asset) as T );
        else
            callback( r.asset as T );
    } 
}
