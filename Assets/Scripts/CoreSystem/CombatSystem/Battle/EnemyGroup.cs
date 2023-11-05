using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyGroup", menuName = "ProjectDungeon/EnemyGroup", order = 0)]
public class EnemyGroup : ScriptableObject {
    public EnemyBase[] enemy = new EnemyBase[6];
}
