using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : PanelBase
{
    public Dialogue dialogue;
    private int index;

    public override void ShowSelf()
    {
        index = 0;
        ShowText();
    }

    public void ShowText()
    {
        if(dialogue.character_order[index] == "Player")
        {
            FindComponent<Image>("PlayerIcon").gameObject.SetActive(true);
            FindComponent<Image>("CharacterIcon").gameObject.SetActive(false);  
        }
        else
        {
            FindComponent<Image>("PlayerIcon").gameObject.SetActive(false); 
            FindComponent<Image>("CharacterIcon").gameObject.SetActive(true);
            FindComponent<Image>("CharacterImage").sprite = ResourceController.Controller().Load<Sprite>("Image/Objects/"+dialogue.character_order[index]);
        }
        
        FindComponent<Text>("DialogueText").text = dialogue.character_lines[index];
    }

    protected override void OnButtonClick(string button_name)
    {
        index ++;

        if(index >= dialogue.character_order.Count)
            GUIController.Controller().RemovePanel("DialoguePanel");
        else
            ShowText();
    }
}
