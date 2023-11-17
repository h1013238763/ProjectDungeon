using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTester : BaseControllerMono<CodeTester>
{

    public void TestCode()
    {
        ItemController.Controller();
        ItemController.Controller().GetItem("GreenHerb", 20);
        ItemController.Controller().GetItem("EnchantmentShard", 20);
        ItemController.Controller().GetItem("StrengthenShard", 20);
        ItemController.Controller().GetEquip(ItemController.Controller().RandomEquip("RustySword", 3, 3));
        ItemController.Controller().GetPotion("NormalHealingPotion", 2);
    }
}
