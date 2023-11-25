using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : PanelBase
{
    private Transform target;
    
    private string control_component = "Background";
    private string load_event_key = "LoadingAnimeComplete";

    public string anime_effect = "Fade";
    public float anime_time = 1f;

    public override void ShowSelf()
    {   
        gameObject.SetActive(true);
        // initial
        target = FindComponent<Image>(control_component).transform;

        // start performing
        AnimeEffect(true);
        // collect previous scene garbage
        MemoryController.Controller().ForceCollectGarbageAsync((ulong)(2000000000*anime_time));
        MonoController.Controller().StartCoroutine(ActAfterSeconds(anime_time*2, false));
    }
    
    public override void HideSelf()
    {
        AnimeEffect(false);
        MonoController.Controller().StartCoroutine(ActAfterSeconds(anime_time*2, true));
    }

    public void AnimeEffect(bool is_enter)
    {
        if(anime_effect == "ChangeSize")
        {
            if(is_enter)
                TweenController.Controller().ChangeSizeTo(target, new Vector3(0, 0, 0), anime_time);
            else
                TweenController.Controller().ChangeSizeTo(target, new Vector3(1, 1, 0), anime_time);
        }
        else if(anime_effect == "Fade")
        {
            if(is_enter)
                TweenController.Controller().GroupFade(target, true, anime_time, TweenType.Smooth);
            else
                TweenController.Controller().GroupFade(target, false, anime_time, TweenType.Smooth);
        }
    }

    private IEnumerator ActAfterSeconds(float seconds, bool remove)
    {
        yield return new WaitForSeconds(seconds);

        if(remove)
        {
            EventController.Controller().RemoveEventKey(load_event_key);
            gameObject.SetActive(false);
        }
        else
        {
            EventController.Controller().EventTrigger(load_event_key);
        }
            

        
    }
}
