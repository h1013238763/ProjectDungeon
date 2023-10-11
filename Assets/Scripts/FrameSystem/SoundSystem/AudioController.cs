using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sound controller module
/// </summary>
public class AudioController : BaseController<AudioController>
{
    private GameObject audio_obj;
    private float master_volume = 1;
    
    private AudioSource music_player = null;
    private float music_volume = 1;
    
    private GameObject sound_player;
    private List<AudioSource> sound_list = new List<AudioSource>();
    private List<AudioSource> inactive_list = new List<AudioSource>();
    private float sound_volume = 1;

    public AudioController()
    {
        MonoController.Controller().AddUpdateListener(Update);
    }

    private void Update()
    {
        for(int i = 0; i < sound_list.Count; i ++)
        {
            if(!sound_list[i].isPlaying)
                StopSound(sound_list[i]);
        }
    }

    /// <summary>
    /// change the master volume
    /// </summary>
    /// <param name="v">volume value</param>
    public void ChangeMasterVolume(float v)
    {
        master_volume = v;
        ChangeMusicVolume(music_volume);
        ChangeSoundVolume(sound_volume);
    }

    /// <summary>
    /// play bgm
    /// </summary>
    /// <param name="name">name of bgm</param>
    public void StartMusic(string name)
    {
        // BGM Handler Check
        initialAudioObject();

        // load bgm and play
        ResourceController.Controller().LoadAsync<AudioClip>("Audio/Music/" + name, (m) =>
        {
            music_player.clip = m;
            music_player.loop = true;
            music_player.volume = master_volume * music_volume;
            music_player.Play();
        });
    }

    /// <summary>
    /// change volume of music
    /// </summary>
    /// <param name="v">volume value</param>
    public void ChangeMusicVolume(float v)
    {
        music_volume = v;
        if(music_player != null)
            music_player.volume = master_volume * music_volume;
    }

    /// <summary>
    /// pause bgm
    /// </summary>
    public void PauseMusic()
    {
        if(music_player != null)
            music_player.Pause();
    }

    /// <summary>
    /// stop bgm
    /// </summary>
    public void StopMusic()
    {
        if(music_player != null)
            music_player.Stop();
    }

    /// <summary>
    /// play sound effect
    /// </summary>
    /// <param name="name">name of sound</param>
    /// <param name="isLoop">loop sound?</param>
    /// <param name="callback">possible actions</param>
    public void StartSound(string name, bool isLoop = false, UnityAction<AudioSource> callback = null)
    {
        // sound handler check
        initialAudioObject();

        // get a inactive sound
        AudioSource source;
        if(inactive_list.Count > 0)
            source = inactive_list[0]; 
        else
            source = sound_player.AddComponent<AudioSource>();

        // load audioclip and add it into object
        ResourceController.Controller().LoadAsync<AudioClip>("Audio/Sound/" + name, (m) =>
        {   
            source.enabled = true;
            inactive_list.Remove(source);
            source.clip = m;
            source.volume = master_volume * sound_volume;
            source.loop = isLoop;
            source.Play();
            sound_list.Add(source);
            if(callback != null)
                callback(source);
        });
    }

    /// <summary>
    /// change sound volume
    /// </summary>
    /// <param name="v">volume</param>
    public void ChangeSoundVolume(float v)
    {
        sound_volume = v;
        for(int i = 0; i < sound_list.Count; i ++)
            sound_list[i].volume = master_volume * sound_volume;
    }

    /// <summary>
    /// stop target sound effect
    /// </summary>
    /// <param name="sound">sound handler object</param>
    public void StopSound(AudioSource source)
    {
        if( sound_list.Contains(source))
        {
            sound_list.Remove(source);
            inactive_list.Add(source);
            source.Stop();
            source.enabled = false;
        }
    }

    /// <summary>
    /// stop all sound effects
    /// </summary>
    public void StopSound()
    {
        foreach(AudioSource source in sound_list)
        {
            if( sound_list.Contains(source))
            {
                inactive_list.Add(source);
                source.Stop();
                source.enabled = false;
            }           
        }
        sound_list.Clear();
    }

    /// <summary>
    /// Initial the audio player object
    /// </summary>
    private void initialAudioObject()
    {
        if(audio_obj == null)
        {
            audio_obj = new GameObject("Audio Player");
            GameObject.DontDestroyOnLoad(audio_obj);
        }

        // create bgm handler object
        if(music_player == null)
        {   
            GameObject obj = new GameObject("MusicPlayer");
            obj.transform.SetParent(audio_obj.transform);
            music_player = obj.AddComponent<AudioSource>();
        }

        if(sound_player == null)
        {
            sound_player = new GameObject("SoundPlayer");
            sound_player.transform.SetParent(audio_obj.transform);
        }
    }
}
