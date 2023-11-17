using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "Dialogue", menuName = "ProjectDungeon/Dialogue", order = 0)]
public class Dialogue : ScriptableObject {
    
    public List<string> character_order;

    [TextArea]
    public List<string> character_lines;
    
}
