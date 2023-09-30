using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Object Pool Module
/// </summary>
public class PoolController : BaseController<PoolController>
{
    private GameObject dictionary_obj;
    private GameObject world_obj;
    private GameObject pool_obj;
    // Dictionary of object pool
    public Dictionary<string, PoolData> dictionary_pool = new Dictionary<string, PoolData>();

    /// <summary>
    /// Initial pool controller objects
    /// </summary>
    private void Initial()
    {
        if(dictionary_obj == null)
        {
            dictionary_obj = new GameObject("Pool Controller");
            world_obj = new GameObject("World Objects");
            world_obj.transform.SetParent(dictionary_obj.transform);
            pool_obj = new GameObject("Pool Objects");
            pool_obj.transform.SetParent(dictionary_obj.transform);
        }
    }

    /// <summary>
    /// get a object async from object pool
    /// </summary>
    /// <param name="name">name of object</param>
    public void GetObject(string name, UnityAction<GameObject> callback)
    {
        Initial();

        // Create and search object pool
        if(!dictionary_pool.ContainsKey(name))
            dictionary_pool.Add(name, new PoolData(pool_obj, name));

        // active and callback object
        if(dictionary_pool[name].pool_list.Count > 0)
        {
            GameObject temp = dictionary_pool[name].GetObj();
            temp.transform.SetParent(world_obj.transform);
            callback(temp);
        }
        else
        {
            ResourceController.Controller().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                o.transform.SetParent(world_obj.transform);
                callback(o);
            });
        } 
    }

    /// <summary>
    /// get a object from object pool
    /// </summary>
    /// <param name="name">name of object</param>
    /// <returns>target object</returns>
    public GameObject GetObject(string name)
    {
        Initial();

        // Create and search object pool
        if(!dictionary_pool.ContainsKey(name))
            dictionary_pool.Add(name, new PoolData(pool_obj, name));

        GameObject temp;
        if( dictionary_pool[name].pool_list.Count > 0 )
        {
            temp = dictionary_pool[name].GetObj();
        }
        else
        {
            temp = new GameObject(name);
        }
        
        temp.transform.SetParent(world_obj.transform);
        return temp;

    }

    /// <summary>
    /// push a object back to object pool
    /// </summary>
    /// <param name="name">name of object</param>
    /// <param name="obj">target object</param>
    public void PushObject(string name, GameObject obj)
    {
        // deactive object
        obj.SetActive(false);

        // push it back to pool
        dictionary_pool[name].PushObj(obj);
    }

    /// <summary>
    /// clear object pool
    /// </summary>
    public void Clear(){
        dictionary_pool.Clear();
        GameObject.Destroy(dictionary_obj);
    }

}

/// <summary>
/// Custom Object Pool Class
/// </summary>
public class PoolData
{
    
    public GameObject pool_obj;         // object
    public List<GameObject> pool_list;  // pool of objects

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="obj"> the object to store </param>
    /// <param name="dic_obj"> parent object for dictionary </param>
    public PoolData(GameObject dic_obj, string name)
    {
        pool_obj = new GameObject(name);
        pool_obj.transform.SetParent(dic_obj.transform);

        pool_list = new List<GameObject>();
    }

    /// <summary>
    /// push a object back to object pool
    /// </summary>
    /// <param name="name">name of object</param>
    /// <param name="obj">target object</param>
    public void PushObj(GameObject obj)
    {
        pool_list.Add(obj);
        obj.transform.SetParent(pool_obj.transform);
    }

    /// <summary>
    /// get a object from object pool
    /// </summary>
    /// <returns>target object</returns>
    public GameObject GetObj()
    {
        GameObject obj = obj = pool_list[0];
        pool_list.RemoveAt(0);  

        return obj;  
    }
}