using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerVolume : Interactable
{
    public enum TriggerType{Tutorial};
    public TriggerType VolumeType;

    public Color triggerColor;

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            switch(VolumeType){
                case TriggerType.Tutorial:
                    CreateTooltip();
                    break;
            }
        }
    }

    void OnTriggerExit(Collider other) 
    {
        DestroyToolTip();
    }

    void OnDrawGizmos() {
        Gizmos.color = triggerColor;
        Gizmos.DrawCube(transform.position + GetComponent<BoxCollider>().center,GetComponent<BoxCollider>().size);
    }
}
