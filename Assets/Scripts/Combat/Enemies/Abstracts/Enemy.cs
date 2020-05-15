using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : DamageableObject
{

    [Space]
    [Header("Enemy")]

    [Range(0,90)]
    public float viewDetectionAngle;

    [Range(1,50)]
    public float viewDetectionRange;

    [Range(1,10)]
    public float audioDetectionRange;

    [Range(-5,5)]
    public int hostileLevel;

    bool alerted;

    public enum PatrolType {Cyclic, Linear}
    public PatrolType patrolType;

    public Transform[] patrolRoute;
    int patrolPointNumber = 0;
    int patrolPointNumberInc = 1;

    Vector3 target;

    public GameObject[] GetViewedObjects()  
    {
        List<GameObject> viewed = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position,viewDetectionRange);

        foreach (Collider collider in colliders)
        {
            if(Vector3.Angle(transform.forward,collider.transform.position - transform.position) < viewDetectionAngle) 
                viewed.Add(collider.gameObject);
        }

        return viewed.ToArray();
    }
    
    public GameObject[] GetHeardObjects()
    {
        List<GameObject> heard = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position,audioDetectionRange);

        foreach (Collider collider in colliders)
        {
            heard.Add(collider.gameObject);
        }

        return heard.ToArray();
    }

    public bool IsPlayerViewed(){
        GameObject[] viewedObjects = GetViewedObjects();
        foreach(GameObject viewedObject in viewedObjects)
        {
            if(viewedObject.tag == "Player")    return true;
        }
        return false;
    }
    public bool IsPlayerHeard(){
        GameObject[] heardObjects = GetHeardObjects();
        foreach(GameObject heardObject in heardObjects)
        {
            if(heardObject.tag == "Player")    return true;
        }
        return false;
    }

    public void SetAvoidanceDistance(float distance){
        GetComponent<NavMeshAgent>().stoppingDistance = distance;
    }

    public void MoveToTarget()
    {
        GetComponent<NavMeshAgent>().SetDestination(target);
    }

    public void SetTarget(Vector3 _target){
        target = _target;
    }

    public void FollowPatrol()
    {
        SetTarget(patrolRoute[patrolPointNumber].position);
        MoveToTarget();

        if(Vector3.Distance(transform.position,patrolRoute[patrolPointNumber].position) < 0.1)
        {
            if(patrolPointNumberInc > 0)
            {
                if(patrolPointNumber + patrolPointNumberInc < patrolRoute.Length)
                {
                    patrolPointNumber += patrolPointNumberInc;
                }
                else
                {
                    ResetPatrol();
                }
            }
            else if(patrolPointNumberInc < 0)
            {
                if(patrolPointNumber + patrolPointNumberInc >=  0)
                {
                    patrolPointNumber += patrolPointNumberInc;
                }
                else
                {
                    ResetPatrol();
                }
            }
            
        }
    }

    public void ChasePlayer(){
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform.position);
        MoveToTarget();
    }

    public void AlertEnemy(){
        alerted = true;
    }

    public bool isAlerted(){
        return alerted;
    }

    void ResetPatrol()
    {
        switch(patrolType)
        {
            case PatrolType.Cyclic:
                patrolPointNumber = 0;
                patrolPointNumberInc = 1;
                break;

            case PatrolType.Linear:
                patrolPointNumberInc *= -1;
                break;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,transform.position + transform.forward * viewDetectionRange);

        Vector3 arcPoint01 = new Vector3(viewDetectionRange * Mathf.Cos(Mathf.PI/2 - Mathf.Deg2Rad * viewDetectionAngle/2),0,viewDetectionRange * Mathf.Sin(Mathf.PI/2 -Mathf.Deg2Rad * viewDetectionAngle/2));
        Vector3 arcPoint02 = new Vector3(-viewDetectionRange * Mathf.Cos(Mathf.PI/2 - Mathf.Deg2Rad * viewDetectionAngle/2),0,viewDetectionRange * Mathf.Sin(Mathf.PI/2 -Mathf.Deg2Rad * viewDetectionAngle/2));
        Vector3 arcPoint03 = new Vector3(0,viewDetectionRange * Mathf.Cos(Mathf.PI/2 - Mathf.Deg2Rad * viewDetectionAngle/2),viewDetectionRange * Mathf.Sin(Mathf.PI/2 -Mathf.Deg2Rad * viewDetectionAngle/2));
        Vector3 arcPoint04 = new Vector3(0,-viewDetectionRange * Mathf.Cos(Mathf.PI/2 - Mathf.Deg2Rad * viewDetectionAngle/2),viewDetectionRange * Mathf.Sin(Mathf.PI/2 -Mathf.Deg2Rad * viewDetectionAngle/2));

        Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(transform.eulerAngles.y,Vector3.up) * arcPoint01 + transform.position);
        Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(transform.eulerAngles.y,Vector3.up) * arcPoint02 + transform.position);
        Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(transform.eulerAngles.y,Vector3.up) * arcPoint03 + transform.position);
        Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(transform.eulerAngles.y,Vector3.up) * arcPoint04 + transform.position);

        Gizmos.DrawWireSphere(transform.position,audioDetectionRange);

        Gizmos.color = Color.red;
        #region Patrol Gizmos
        if(patrolRoute.Length > 0){
            Transform lastPoint = null;
            foreach(Transform point in patrolRoute)
            {
                Gizmos.DrawWireSphere(point.position,0.25f);
                if(lastPoint != null){
                    Gizmos.DrawLine(lastPoint.position,point.position);
                }
                lastPoint = point;
            }

            if(patrolType == PatrolType.Cyclic)
                Gizmos.DrawLine(patrolRoute[patrolRoute.Length-1].position,patrolRoute[0].position);
            #endregion
        
        }
    }
}
