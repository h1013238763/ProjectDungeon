using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : PanelBase
{
    private Transform mask;

    public override void ShowSelf()
    {   
        gameObject.SetActive(true);
        // initial
        mask = FindComponent<Image>("MaskImage").transform;

        // audio setting
        AudioController.Controller().StopSound();
        AudioController.Controller().StartSound("EnterLoadingPanel");

        // start performing
        TweenController.Controller().ChangeSizeTo(mask, new Vector3(0, 0, 0), 0.5f);
        // collect previous scene garbage
        MemoryController.Controller().ForceCollectGarbageAsync(1000000000);
        MonoController.Controller().StartCoroutine(ActAfterSeconds(1f, false));
    }
    
    public override void HideSelf()
    {
        AudioController.Controller().StartSound("ExitLoadingPanel");
        TweenController.Controller().ChangeSizeTo(mask, new Vector3(1, 1, 0), 0.5f);
        MonoController.Controller().StartCoroutine(ActAfterSeconds(1, true));
    }

    private IEnumerator ActAfterSeconds(float seconds, bool remove)
    {
        yield return new WaitForSeconds(seconds);

        if(remove)
        {
            EventController.Controller().RemoveEventKey("LoadingAnimeComplete");
            gameObject.SetActive(false);
        }
        else
        {
            EventController.Controller().EventTrigger("LoadingAnimeComplete");
        }
            

        
    }
}
