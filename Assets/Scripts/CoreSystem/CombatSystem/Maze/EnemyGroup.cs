using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyGroup", menuName = "ProjectDungeon/EnemyGroup", order = 0)]
public class EnemyGroup : ScriptableObject {
    public List<string> enemies = new List<string>(new string[6]);
}