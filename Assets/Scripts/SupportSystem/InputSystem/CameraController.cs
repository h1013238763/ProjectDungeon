using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!SceneController.Controller().ContainsObject("StageController"))
            StageController.Controller();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
