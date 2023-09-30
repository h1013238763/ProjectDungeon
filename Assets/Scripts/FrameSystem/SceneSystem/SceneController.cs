using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class SceneController : BaseController<SceneController>
{
    /// <summary>
    /// 同步场景切换
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="fun">加载函数</param>
    public void LoadScene(string name, UnityAction fun)
    {
        // 同步加载场景
        SceneManager.LoadScene(name);

        fun.Invoke();
    }

    /// <summary>
    /// 异步场景切换
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="fun">加载函数</param>
    public void LoadSceneAsync(string name, UnityAction fun)
    {
        MonoController.Controller().StartCoroutine(ILoadSceneAsync(name, fun));
    }

    /// <summary>
    /// 携程函数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    private IEnumerator ILoadSceneAsync(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        while(ao.isDone)
        {
            EventController.Controller().EventTrigger("Refresh Progress", ao.progress);
            yield return ao.progress;
        }

        fun();
    }

}
