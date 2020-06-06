using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponsCase : Interactable
{

    Animator animator;
    public GameObject containedGameObject;
    public Transform objectPoint;
    public GameObject openingParticle;
    public AudioClip openingSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void PerformInteraction()
    {
        animator.SetTrigger("Open");
        PlayOpeningSound();
        
        GameObject instance = Instantiate(containedGameObject);
        GameObject particle_instance = Instantiate(openingParticle);

        particle_instance.transform.position = objectPoint.transform.position;
        instance.transform.position = objectPoint.position;

        instance.name = containedGameObject.name;

        TurnOffLights();
        Destroy(GetComponent<BoxCollider>());
        Destroy(particle_instance,5);
    }

    void TurnOffLights(){
        transform.GetChild(0).GetChild(0).GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
        transform.GetChild(0).GetChild(1).GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
        transform.GetChild(0).GetComponent<Renderer>().materials[2].DisableKeyword("_EMISSION");

        transform.GetChild(1).GetChild(0).GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
        transform.GetChild(1).GetChild(1).GetComponent<Renderer>().materials[1].DisableKeyword("_EMISSION");
        transform.GetChild(1).GetComponent<Renderer>().materials[2].DisableKeyword("_EMISSION");
    }

    void PlayOpeningSound()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = openingSound;
        source.Play();
    }


}
