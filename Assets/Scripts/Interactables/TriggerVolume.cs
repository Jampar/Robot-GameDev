using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class TriggerVolume : Interactable
{
    public enum TriggerType{Tutorial,SceneChange, AnimationTrigger,AnimationBoolean};
    public TriggerType VolumeType;

    public Color triggerColor;

    [Header("Scene Change")]
    public int transitionSceneIndex;
    public GameObject fadeObject;
    public Transform returnPoint;

    [Header("Animation")]
    public GameObject animationActivation;

    [Header("Animation Trigger")]
    public string animationTrigger;

    [Header("Animation Boolean")]
    public string animationBool;

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            switch(VolumeType){
                case TriggerType.Tutorial:
                    CreateTooltip();
                    break;

                case TriggerType.SceneChange:
                    fadeObject.GetComponent<Animator>().SetTrigger("Fade Out");
                    fadeObject.GetComponent<FadeScript>().fadeToSceneIndex = transitionSceneIndex;
                    break;
                case TriggerType.AnimationTrigger:
                    animationActivation.GetComponent<Animator>().SetTrigger(animationTrigger);
                    gameObject.SetActive(false);
                    break;
                case TriggerType.AnimationBoolean:
                    animationActivation.GetComponent<Animator>().SetBool(animationBool,true);
                    break;
                
            }
        }
    }

    void OnTriggerExit(Collider other) 
    {
        DestroyToolTip();
        if(VolumeType == TriggerType.AnimationBoolean){
            animationActivation.GetComponent<Animator>().SetBool(animationBool,false);
        }
    }

   


    void OnDrawGizmos() {
        Gizmos.color = triggerColor;
        Gizmos.DrawCube(transform.position + GetComponent<BoxCollider>().center,GetComponent<BoxCollider>().size);
    }
}
