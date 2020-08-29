using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using cakeslice;
using System;

public class PlayerInteraction : MonoBehaviour
{
    //Array of the current and last interactable in range
    Interactable[] interactableHistory = new Interactable[2];

    float heightCalculationOffset = 4;

    [Space]

    //Interaction radius range
    [Range(0,10)]
    public float reachRadius;

    //Interaction height range
    [Range(0,5)]
    public float reachHeight;


    // Update is called once per frame
    void Update()
    {
        //Get the closest interactable
        interactableHistory[0] = GetClosestInteractable();

        //Handle the highlighting of interactables
        HighlightingHandling();

        //Handle the interaction with current interactable
        InteractionHandling();
    }

    void InteractionHandling()
    {
        //If there is a current interactable
        if (interactableHistory[0])
        {
            //And the interaction button is pressed
            if (Input.GetButtonDown("Interact"))
            {
                //Perform interaction
                interactableHistory[0].PerformInteraction();
            }
        }

    }

    void HighlightingHandling()
    {
        //If a current interactable is not null
        if (interactableHistory[0])
        {
            //Highlight current interactable
            interactableHistory[0].Highlight();

            //If the previous interactable is empty fill with current
            if (interactableHistory[1] == null)
                interactableHistory[1] = interactableHistory[0];

            //If the current and previous interactable differ remove the highlight from the previous and set it to null
            if (interactableHistory[0] != interactableHistory[1])
            {
                interactableHistory[1].RemoveInteractableHighlight();
                interactableHistory[1] = null;
            }
        }
        //If there is no current interactable but there is a previous
        else if(interactableHistory[1])
        {
            //Remove the highlight from the previous and set it to null
            interactableHistory[1].RemoveInteractableHighlight();
            interactableHistory[1] = null;
        }
    }

    Interactable GetClosestInteractable()
    {
        //Minimum distance to be compared
        float minDist = reachRadius + 1;
        //Return variable
        Interactable closest = null;

        //Collect all colliders within a radius
        Collider[] colls = OverlapCylinder(reachRadius, reachHeight);

        //Loop through the colliders
        foreach (Collider coll in colls)
        {
            //If a collider has the interactable componeent and is interactable
            if (coll.GetComponent<Interactable>())
            {
                if (coll.GetComponent<Interactable>().interactable)
                {
                    Debug.Log("Nearby Interactable " + coll.name);

                    //Horizontal distance to interactable
                    float horizontalDistance = HorizontalDistance(transform.position, coll.transform.position);

                    //If the distance is lest than the current minimum distance
                    if (horizontalDistance < minDist)
                    {
                        //It becomes the new closest
                        closest = coll.GetComponent<Interactable>();
                        //Update distance variable
                        minDist = horizontalDistance;
                    }
                }
            }
        }

        return closest;
    }

    bool isGameObjectInRange(GameObject target)
    {
        //Calculate individual planar distances
        float horizontalDistance = HorizontalDistance(transform.position, target.transform.position);
        float verticalDistance = VerticalDistance(transform.position, target.transform.position);

        //If inside cylinder area
        if (horizontalDistance < reachRadius && verticalDistance < reachHeight)
        {
            return true;
        }
       
        return false;
    }

    Collider[] OverlapCylinder(float radius,float height)
    {
        //List to convert later in returned array
        List<Collider> overlapResult = new List<Collider>();
        //Cast a wide sphere net of objects of twice the reach radius
        Collider[] initalOverlap = Physics.OverlapSphere(transform.position, radius * 2);
        
        //Loop through the initial colliders
        foreach(Collider collider in initalOverlap)
        {
            //If theyre inside the range
            if (isGameObjectInRange(collider.gameObject))
            {
                //Add them to result list
                overlapResult.Add(collider);
            }
        }

        //Return the converted list
        return overlapResult.ToArray();
    }
   
    float HorizontalDistance(Vector3 pos1,Vector3 pos2)
    {
        //Pythagoras Formula
        return Mathf.Sqrt(Mathf.Pow((pos2.x - pos1.x), 2) + Mathf.Pow((pos2.z - pos1.z), 2));
    }
    float VerticalDistance(Vector3 pos1, Vector3 pos2)
    {
        //Absolute difference
        return Mathf.Abs(pos2.y - pos1.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawWireDisk(transform.position, reachRadius, Gizmos.color);
        DrawWireDisk(transform.position + Vector3.up * reachHeight, reachRadius, Gizmos.color);

        Gizmos.DrawWireSphere(transform.position, reachRadius * 2);

        try
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(interactableHistory[0].transform.position,0.5f);
            Gizmos.DrawSphere(interactableHistory[1].transform.position,0.5f);

        }
        catch (Exception e)
        {

        }

    }
    public static void DrawWireDisk(Vector3 position, float radius, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }
}
