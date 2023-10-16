using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestCode()
    {
        ItemController.Controller();
        ItemController.Controller().GetItem("GreenHerb", 20);
        ItemController.Controller().GetItem("EnchantmentShard", 1);
        ItemController.Controller().GetItem("StrengthenShard", 20);
        ItemController.Controller().GetEquip("RustySword", 2, 3);
        ItemController.Controller().GetPotion("NormalHealingPotion", 2);
    }
}
