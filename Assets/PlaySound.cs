using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour
{
    public AudioClip[] sounds;
    void Play(int index){
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = sounds[index];
        audioSource.Play();
    }
}
