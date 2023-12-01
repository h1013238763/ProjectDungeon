using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnchantController : BaseController<EnchantController>
{
    public Dictionary<int, List<string>> slot_enchant = new Dictionary<int, List<string>>();    // enchant for each equip slot
    public Dictionary<string, EquipEnchant> dict_enchant = new Dictionary<string, EquipEnchant>();
    public Dictionary<string, EquipEnchant> avail_enchant = new Dictionary<string, EquipEnchant>(); // unlocked enchants
    
    public EnchantData data;


    // return a random enchant of given equip type
    public EquipEnchant GetRandomEnchant( EquipType equip_type)
    {
        int enchant_index = UnityEngine.Random.Range(0, slot_enchant[(int)equip_type].Count-1);
        return dict_enchant[slot_enchant[(int)equip_type][enchant_index]];
    }

    // get enchant info
    public EquipEnchant EnchantInfo(string id)
    {
        if(!dict_enchant.ContainsKey(id))
            return null;
        return dict_enchant[id];
    }

    public void InitialData()
    {
        EquipEnchant[] enchants = Resources.LoadAll<EquipEnchant>("Object/Enchant/");

        for(int i = 0; i < 6; i ++)
        {
            slot_enchant.Add(i, new List<string>());
        }

        if(enchants != null)
        {
            dict_enchant.Clear();
            foreach(EquipEnchant enchant in enchants)
            {
                dict_enchant.Add(enchant.enchant_id, enchant);
                foreach(EquipType type in enchant.avail_type)
                {
                    slot_enchant[(int)type].Add(enchant.enchant_id);
                }
            }
        }
    }
    public void NewData()
    {
        data = new EnchantData();

        avail_enchant.Clear();
        foreach(string id in data.unlock_enchants)
        {
            avail_enchant.Add(id, dict_enchant[id]);
        }
    }
    public void LoadData()
    {
        data = XmlController.Controller().LoadData(typeof(EnchantData), "EnchantData") as EnchantData;
        if(data == null)
            data = new EnchantData();

        avail_enchant.Clear();
        foreach(string id in data.unlock_enchants)
        {
            if(!avail_enchant.ContainsKey(id))
                avail_enchant.Add(id, dict_enchant[id]);
        }
    }
    public void SaveData()
    {
        if(data != null)
            XmlController.Controller().SaveData(data, "EnchantData");
    }
}

public class EnchantData
{
    public List<string> unlock_enchants = new List<string>();

    public EnchantData()
    {
        unlock_enchants.Add("FortifyAttack");
        unlock_enchants.Add("FortifyDefense");
        unlock_enchants.Add("FortifyHealth");
    }
}