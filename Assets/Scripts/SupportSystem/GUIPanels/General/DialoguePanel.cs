using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DialoguePanel : PanelBase
{
    public Dialogue dialogue;
    private int index;

    private int background_progress;
    private int music_progress;

    public override void ShowSelf()
    {
        index = 0;
        background_progress = 0;
        music_progress = 0;

        ShowText();
    }

    public void ShowText()
    {
        if(dialogue.large_dialogue)
        {
            // change background
            if(background_progress < dialogue.background_change_index.Count)
            {
                FindComponent<Image>("Background").gameObject.SetActive(true);
                if(index == dialogue.background_change_index[background_progress])
                {
                    FindComponent<Image>("Background").sprite = DialogueController.Controller().GetBackground(dialogue.background_id[background_progress]);
                    background_progress ++;
                }
            }
            // change music
            if(music_progress < dialogue.music_change_index.Count)
            {
                if(index == dialogue.music_change_index[music_progress])
                {
                    AudioController.Controller().StartMusic("Dialogue/"+dialogue.music_id[music_progress]);
                    music_progress ++;
                }
            }
        }
        else
        {
            FindComponent<Image>("Background").gameObject.SetActive(false);
        }
        
        // change speaker name
        if(dialogue.character_order[index] != "Player")
            FindComponent<Text>("Speaker").text = dialogue.character_order[index];
        else
            FindComponent<Text>("Speaker").text = PlayerController.Controller().data.player_name;
        // change avator
        if(dialogue.character_order[index] == "Player")
        {
            FindComponent<Image>("PlayerIcon").gameObject.SetActive(true);
            FindComponent<Image>("CharacterIcon").gameObject.SetActive(false);  
        }
        else if(dialogue.character_order[index] == "Narrator")
        {
            FindComponent<Image>("PlayerIcon").gameObject.SetActive(false);
            FindComponent<Image>("CharacterIcon").gameObject.SetActive(false);
            FindComponent<Image>("CharacterIcon").sprite = DialogueController.Controller().GetAvator("Player");
        }
        else
        {
            FindComponent<Image>("PlayerIcon").gameObject.SetActive(false); 
            FindComponent<Image>("CharacterIcon").gameObject.SetActive(true);
            FindComponent<Image>("CharacterIcon").sprite = DialogueController.Controller().GetAvator(dialogue.character_order[index]);
        }
        // change line
        string line = dialogue.character_lines[index];
        if(line.Contains("{Player}"))
        {
            string new_line = line.Substring(0, line.IndexOf("{"));
            new_line += PlayerController.Controller().data.player_name;
            new_line += line.Substring(line.IndexOf("}")+1);
            FindComponent<Text>("DialogueText").text = new_line;
        }
        else
        {
            FindComponent<Text>("DialogueText").text = line;
        }
    }

    protected override void OnButtonClick(string button_name)
    {
        AudioController.Controller().StartSound("DialogueNext");

        index ++;

        if(index >= dialogue.character_order.Count)
        {
            GUIController.Controller().RemovePanel("DialoguePanel");
            EventController.Controller().EventTrigger("DialogueFinish:"+dialogue.dialogue_id);
        }
        else
        {
            ShowText();
        }
            
    }
}
