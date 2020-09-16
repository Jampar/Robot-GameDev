using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniRobot : FlyingObject
{
    public GameObject backpack;
    Transform robotSlot;

    float storeDist = 20.0f;
    public static bool returnToSlot;
    bool inSlot;

    public float hoverDistance;
    public float hoverSpeed;
    public float heightLerpSpeed;

    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        robotSlot = backpack.transform.Find("Robot Slot");
    }

    // Update is called once per frame
    void Update()
    {
        if(returnToSlot)
            ReturnToSlot();

    }

    void ReleaseFromSlot()
    {
        transform.SetParent(null);
        agent.enabled = true;
        inSlot = false;
    }

    void ReturnToSlot()
    {
        agent.SetDestination(robotSlot.position);
        float distToSlot = Vector3.Distance(transform.position, agent.destination);
        
        if (distToSlot < storeDist*transform.localScale.y)
        {
            agent.enabled = false;
            transform.SetParent(robotSlot);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            agent.SetDestination(transform.position);
            returnToSlot = false;
            inSlot = true;
        }
    }

}
