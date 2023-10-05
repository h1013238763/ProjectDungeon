using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ItemController.Controller();
        ItemController.Controller().GetPotion("NormalHealingPotion", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestButton()
    {
        GUIController.Controller().ShowPanel<InventPanel>("InventPanel", 1);
    }
}
