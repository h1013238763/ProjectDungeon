using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : PanelBase
{
    private Transform mask;

    public override void ShowSelf()
    {   
        // initial
        mask = FindComponent<Image>("Mask").transform;

        // start performing
        TweenController.Controller().ChangeSizeTo(mask, new Vector3(0, 0, 0), 0.5f);
        // collect previous scene garbage
        MemoryController.Controller().ForceCollectGarbageAsync(1000000000);
        MonoController.Controller().StartCoroutine(ActAfterSeconds(0.5f, false));
    }
    
    public override void HideSelf()
    {
        TweenController.Controller().ChangeSizeTo(mask, new Vector3(1, 1, 0), 0.5f);
        MonoController.Controller().StartCoroutine(ActAfterSeconds(1, true));
    }

    private IEnumerator ActAfterSeconds(float seconds, bool remove)
    {
        yield return new WaitForSeconds(seconds);

        if(remove)
        {
            EventController.Controller().RemoveEventKey("LoadingAnimeFinish");
            GUIController.Controller().RemovePanel("LoadingPanel");
        }
        else
            EventController.Controller().EventTrigger("LoadingAnimeFinish");
    }
}
