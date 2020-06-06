using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{
    public GameObject activation;
    public Color gizmoColor;

    public override void PerformInteraction(){
        activation.GetComponent<Animation>().Play();
        Destroy(this);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position,activation.transform.position);

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position,2);
        Gizmos.DrawCube(activation.transform.position,Vector3.one);
    }
}
