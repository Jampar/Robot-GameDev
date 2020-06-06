using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] ambientMusic;
    public AudioClip[] hostileMusic;
    public AudioClip[] transistionStings;

    static GameObject _instance;

    int playerState = 0;

    AudioSource source;

    
    void Awake()
    {
         //if we don't have an [_instance] set yet
        if(!_instance)
            _instance = this.gameObject;
         //otherwise, if we do, kill this thing
        else
            Destroy(this.gameObject);
        
        DontDestroyOnLoad(this.gameObject);
        source = GetComponent<AudioSource>();
    }

    private void Update() 
    {
        if(!source.isPlaying){
            if(playerState == 0)
            {
                PlayRandomAmbientMusic();
            }
        }
    }

    void PlayRandomAmbientMusic()
    {
        int randInt = Random.Range(0,ambientMusic.Length);

        if(source.clip != null)
        {
            while(source.clip == ambientMusic[randInt])
                randInt = Random.Range(0,ambientMusic.Length);
        }
        
        source.clip = ambientMusic[randInt];
        source.Play();
    }
}
