using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BuffController : BaseController<BuffController>
{

    private Dictionary<string, Sprite> image_buff = new Dictionary<string, Sprite>();

    public Sprite GetImage(string id)
    {
        if(!image_buff.ContainsKey(id))
            return null;
        return image_buff[id];
    }

    public void InitialData()
    {
        // Tutorial Effect
        
    }
}