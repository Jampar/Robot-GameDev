using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniRobot : MonoBehaviour
{

    NavMeshAgent agent;

    public float hoverDistance;
    public float hoverSpeed;

    GameObject player;

    Vector3 targetPos;

    public float moveSpeed;
    public float stoppingDist;
    public float yOffset;

    public float lookSpeed;
    Vector3 lookPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
        HoverSway();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(LookDirection()) * Quaternion.Euler(-90,0,0), lookSpeed * Time.deltaTime);
    }

    void FollowPlayer()
    {
        Vector3 playerPos = player.transform.position;

        if (Vector3.Distance(transform.position, playerPos) > stoppingDist)
        {
            targetPos = playerPos;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPos.x, targetPos.y + yOffset, targetPos.z), moveSpeed * Time.deltaTime);

    }
    Vector3 LookDirection()
    {
        return targetPos - transform.position;
    }
    void HoverSway()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (Mathf.Sin(Time.time * hoverSpeed) * hoverDistance * Time.deltaTime), transform.position.z);
    }

}
