using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : PanelBase
{
    public int health_max;
    public int health_curr;

    public List<Effect> effects;

    public Vector2 bar_size = new Vector2(175f, 8.5f);

    public float health_bar_anime_time = 0.5f;

    public void ChangeHealth(int num)
    {
        Image bar_back = FindComponent<Image>("HealthBackground");
        Image bar_curr = FindComponent<Image>("Health");
        Image bar_anime = FindComponent<Image>("HealthAnime");

        // heal
        if(num > health_curr)
        {
            TweenController.Controller().ChangeSizeTo(bar_curr.transform, new Vector2(num / health_max, 1), health_bar_anime_time, TweenType.Smooth);
        }
        // take damage
        else
        {
            bar_curr.transform.localScale = new Vector2(num / health_max, 1);
            TweenController.Controller().ChangeSizeTo(bar_anime.transform, bar_curr.transform.localScale, health_bar_anime_time, TweenType.Smooth);
        }

        health_curr = num;
    }

    public void PredictHealth(int num)
    {
        Image bar_predict = FindComponent<Image>("HealthPredict");
        Image bar_curr = FindComponent<Image>("Health");

        bar_predict.transform.localScale = new Vector2( num / health_max, 1);

        float curr_x = bar_size.x * bar_curr.transform.localScale.x;            // the length of health bar
        float predict_x = bar_size.x * bar_predict.transform.localScale.x / 2;  // the center point length of predict bar
        float pos_x = -(bar_size.x/2) + curr_x + predict_x;

        // heall
        if(num > health_curr)
        {
            bar_predict.transform.localPosition = new Vector3(-(bar_size.x/2) + curr_x + predict_x, 0, 0);
        }
        // damage
        else
        {
            bar_predict.transform.localPosition = new Vector3(-(bar_size.x/2) + curr_x - predict_x, 0, 0);
        }
    }

    public void SetBuff()
    {

    }

    public void AddBuff()
    {
        
    }

    public void RemoveBuff()
    {

    }
}
