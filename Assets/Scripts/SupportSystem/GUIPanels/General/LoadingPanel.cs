using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadingPanel : PanelBase
{
    private Transform target;
    
    private string control_component = "Background";

    public UnityAction loading_action = null;

    public string anime_effect = "Fade";
    public float anime_time = 1f;

    public override void ShowSelf()
    {   
        gameObject.SetActive(true);
        // initial
        target = FindComponent<Image>(control_component).transform;

        EventController.Controller().AddEventListener("LoadingEnterComplete", () => {
            // collect previous scene garbage
            MemoryController.Controller().ForceCollectGarbageAsync((ulong)(2000000000*anime_time));
            loading_action.Invoke();
            AnimeEffect(false);
        });

        EventController.Controller().AddEventListener("LoadingExitComplete", () => {
            gameObject.SetActive(false);
        });

        // start performing
        AnimeEffect(true);        
    }

    public void AnimeEffect(bool is_enter)
    {
        if(is_enter)
        {
            TweenAction action = null;
            if(anime_effect == "ChangeSize")
                action = TweenController.Controller().ChangeSizeTo(target, new Vector3(0, 0, 0), anime_time);
            else if(anime_effect == "Fade")
                action = TweenController.Controller().ChangeAlpha(target, 1, anime_time, TweenType.Smooth);

            TweenController.Controller().AddEventTrigger(action, "LoadingEnterComplete");
        }
        else
        {
            TweenAction action = null;
            if(anime_effect == "ChangeSize")
                action = TweenController.Controller().ChangeSizeTo(target, new Vector3(1, 1, 0), anime_time);
            else if(anime_effect == "Fade")
                action = TweenController.Controller().ChangeAlpha(target, 0, anime_time, TweenType.Smooth);

            TweenController.Controller().AddEventTrigger(action, "LoadingExitComplete");
        }
    }
}
