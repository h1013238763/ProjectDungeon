using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "Dialogue", menuName = "ProjectDungeon/Dialogue", order = 0)]
public class Dialogue : ScriptableObject {
    
    public string dialogue_id;
    public bool large_dialogue;
    public List<string> character_order;

    [TextArea]
    public List<string> character_lines;

    public List<string> background_id;
    public List<int> background_change_index;
    public List<string> music_id;
    public List<int> music_change_index;
}
