using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController<EnemyController>
{
    private Dictionary<string, EnemyBase> enemy_dict = new Dictionary<string, EnemyBase>();
    private Dictionary<string, Sprite> image_enemy = new Dictionary<string, Sprite>();

    public EnemyBase EnemyInfo(string id)
    {
        if(!enemy_dict.ContainsKey(id))
            return null;
        return enemy_dict[id];
    }
    public Sprite GetImage(string id)
    {
        if(!image_enemy.ContainsKey(id))
            return null;
        return image_enemy[id];
    }

    public void InitialData()
    {
        EnemyBase[] enemies = Resources.LoadAll<EnemyBase>("Object/Enemy/");
        if(enemies != null)
        {
            enemy_dict.Clear();
            foreach(EnemyBase enemy in enemies)
            {
                enemy_dict.Add(enemy.enemy_id, enemy);
            }
        }

        Sprite[] images = Resources.LoadAll<Sprite>("Image/Enemy/");
        if(images != null)
        {
            foreach(Sprite image in images)
            {
                if(!image_enemy.ContainsKey(image.name))
                    image_enemy.Add(image.name, image);
            }
        }
    }
}

public enum EnemyEffect
{
    Normal,     // no effect or effect have been triggered and lost
    Fly,        // Normally only takes 70% damage, but takes 200% damage when stunned.
    Undead,     // recover 30% when first time die
    Element     // Immunity to certain types of damage
}

public enum Weakness
{
    Null,
    Fire
}
