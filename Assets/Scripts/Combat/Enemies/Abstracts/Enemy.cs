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

    [Space]
    [Header("Enemy Behaviour")]

    Vector3 target;

    public enum HostileBehaviour {ChasePlayer, StandGround, Retreat};
    public enum IdleBehaviour {Wander, Stand ,Patrol};

    public HostileBehaviour hostileBehaviour;
    public IdleBehaviour idleBehaviour;


    bool alerted;

    [Space]

    [Header("Enemy Patrol")]
    public Transform[] patrolRoute;

    public enum PatrolType {Cyclic, Linear}
    public PatrolType patrolType;

    public int patrolPointNumber = 0;
    public int patrolPointNumberInc = 1;

    [Space]

    [Header("Enemy Wander")]
    public float wanderDistance;
    public float wanderTime;
    float timer;
    bool startWandering = false;
    Vector3 wanderPoint;


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

    public void PerformHostileBehaviour()
    {
        switch(hostileBehaviour)
        {
            case HostileBehaviour.ChasePlayer:
                ChasePlayer();
                break;
            
            case HostileBehaviour.StandGround:
                SetTarget(transform.position);
                break;

        }
    }

    public void PerformIdleBehaviour()
    {
        switch(idleBehaviour)
        {
            case IdleBehaviour.Wander:
                Wander();
                break;

            case IdleBehaviour.Stand:
                break;

            case IdleBehaviour.Patrol:
                FollowPatrol();
                break;
        }
    }

    void SetAvoidanceDistance(float distance){
        GetComponent<NavMeshAgent>().stoppingDistance = distance;
    }

    void MoveToTarget()
    {
        GetComponent<NavMeshAgent>().SetDestination(target);
    }

    void SetTarget(Vector3 _target){
        target = _target;
    }

    void FollowPatrol()
    {
        SetTarget(patrolRoute[patrolPointNumber].position);
        MoveToTarget();

        if(isAtTarget()){

            patrolPointNumber += patrolPointNumberInc;

            if(patrolPointNumber >= patrolRoute.Length || patrolPointNumber < 0)
            {
                ResetPatrol();
            }
        }
    }

    void ChasePlayer(){
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform.position);
        MoveToTarget();
    }

    public void AlertEnemy(){
        alerted = true;
    }

    public bool isAlerted(){
        return alerted;
    }

    public bool isDamaged()
    {
        if(GetHealth() < GetMaxHealth()){
            return true;
        }
        return false;
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
                patrolPointNumber += patrolPointNumberInc;
                break;
        }
    }
    
    void Wander()
    {       
        if(!startWandering) timer = wanderTime;

         timer += Time.deltaTime;
   
        if(timer >= wanderTime)
        {
            Vector3 newPos  = RandomNavSphere(transform.position, wanderDistance, -1);
            SetTarget(newPos);
            timer = 0;
        }

        MoveToTarget();

        startWandering = true;

    }

    bool isAtTarget()
    {
        float distance = Vector3.Distance(transform.position,target);

        if(distance < 0.5 || GetComponent<NavMeshAgent>().isStopped)
        {
            return true;
        }
        return false;
    }
    
    public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
           
            randomDirection += origin;
           
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
           
            return navHit.position;
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
        if(idleBehaviour == IdleBehaviour.Patrol){
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
        }
        #endregion

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(target, Vector3.one * 0.25f);

    }
}
