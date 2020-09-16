using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FlyingObject : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    public float GetGroundDist()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit))
        {
            return Vector3.Distance(transform.position, hit.point);
        }
        else
        {
            return 0;
        }
    }
}
