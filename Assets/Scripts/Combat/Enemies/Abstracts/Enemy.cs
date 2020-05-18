using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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

    [Range(1,50)]
    public float audioDetectionRange;

    [Space]
    [Header("Enemy Behaviour")]

    Vector3 target;

    public enum HostileBehaviour {Chase, StandGround, Retreat};
    public enum IdleBehaviour {Wander, Stand ,Patrol};

    public HostileBehaviour hostileBehaviour;
    public IdleBehaviour idleBehaviour;

    public float idleSpeed;
    public float hostileSpeed;

    [Space]
    public string[] FoeComponents;


    float alertLevel; // 1 == Alert, 0 == Idle
    [Space]
    public float alertLevelRate;
    public float idleLevelRate;
    public float searchAlertThreshold;
    public GameObject alertLevelPrefab;
    GameObject alertMeterInstance;
    bool alerted;
    GameObject alertedBy;
    bool foeFound;

    public float stopBorder;
    [Space]

    public float soundThreshold;
    float searchTimer;
    public float searchTime;
    bool searching;
    Vector3 searchPosition;

    [Space]

    public float meleeReach;
    public float meleeDamage;
    float meleeTimer;
    public float meleeCooldown;
    bool startmeleeCooldown;
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
    public float wanderPauseTime;
    float timer;
    bool startWandering = false;
    Vector3 wanderPoint;
    public float wanderHeightRestriction;
    public float wanderBorderAvoidanceDistance;

    public GameObject[] GetViewedObjects()  
    {
        List<GameObject> viewed = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position,viewDetectionRange);

        foreach (Collider collider in colliders)
        {
            if(Vector3.Angle(transform.forward,collider.transform.position - transform.position) < viewDetectionAngle/2) 
                viewed.Add(collider.gameObject);
        }

        return viewed.ToArray();
    }
    public GameObject GetLoudestHeardObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,audioDetectionRange);
        
        GameObject loudest = null;
        float loudestDeliveredSound = 0.0f;

        foreach (Collider collider in colliders)
        {
            if(collider.GetComponent<AudioSource>())
            {
                AudioSource heardSource = collider.GetComponent<AudioSource>();
                if(heardSource.isPlaying){
                    float deliveredSound = heardSource.volume / Mathf.Pow(Vector3.Distance(heardSource.transform.position,transform.position),2) * heardSource.priority;
                    //print(heardSource.name + ' ' + deliveredSound.ToString());
                    if(deliveredSound > loudestDeliveredSound && deliveredSound > soundThreshold){
                        loudest = heardSource.gameObject;
                    }
                }
            }
        }
        return loudest;
    }
    
    GameObject currentFoe;
    
    public bool IsFoeViewed(){
        GameObject[] viewedObjects = GetViewedObjects();

        foreach(GameObject viewedObject in viewedObjects)
        {
            foreach (string component in FoeComponents){
                
                if(viewedObject.GetComponent(component) != null)
                {
                    currentFoe = viewedObject;
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsFoeHeard(){
        GameObject heardObject = GetLoudestHeardObject();
        if(heardObject != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isSearching(){
        return searching;
    }
    void SearchTimer()
    { 
        timer -= Time.deltaTime;
        if(timer <= 0.0f)
        {
            searching = false;
        }
    }

    public void PerformHostileBehaviour()
    {
        GetComponent<NavMeshAgent>().speed = hostileSpeed;
        switch(hostileBehaviour)
        {
            case HostileBehaviour.Chase:
                ChaseFoe();
                break;
            
            case HostileBehaviour.StandGround:
                SetTarget(transform.position);
                break;

        }
    }
    void PerformSearch()
    {
        SearchTimer();
        SetTarget(searchPosition);
        MoveToTarget();
        
    }
    public void PerformIdleBehaviour()
    {
        GetComponent<NavMeshAgent>().speed = idleSpeed;
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

    public void AlertEnemy()
    {
        alertedBy = currentFoe;
        alertLevel = 1;
        UpdateAlertMeter();

        alerted = true;
    }
    public bool isAlerted(){
        return alerted;
    }
    public float GetAlertLevel(){
        return alertLevel;
    }
    public void IncreaseAlertLevel()
    {
        if(alertLevel < 1)
        {
            alertLevel += alertLevelRate * Time.deltaTime;
        }
        if(alertLevel >= 1) alertLevel = 1;
    }
    public void DecreaseAlertLevel()
    {
        if(alertLevel > 0)
        {
            alertLevel -= idleLevelRate * Time.deltaTime;
        }
        if(alertLevel < 0) alertLevel = 0;
    }
    public void SetAlertLevel(float level){
        alertLevel = level;
    }

    public void SetFoe(GameObject foe){
        currentFoe = foe;
    }

    public void Search()
    {
        if(!isSearching()){
            timer = searchTime;
            searching = true;

            GameObject loudest = GetLoudestHeardObject();
            if(loudest != null)
                searchPosition = loudest.transform.position;
        }
        PerformSearch();

    }

    void CreateAlertMeter()
    {
        if(alertMeterInstance == null){
            alertMeterInstance = Instantiate(alertLevelPrefab);
            alertMeterInstance.transform.position = healthBarPoint.position;
            alertMeterInstance.transform.SetParent(transform);
        }
    }
    void UpdateAlertMeter()
    {
        if(alertMeterInstance != null)
        {
            alertMeterInstance.transform.Find("Alert").localScale = Vector3.one * alertLevel;
        }
    }
    void DestroyAlertMeter()
    {
        if(alertMeterInstance != null){
            Destroy(alertMeterInstance);
        }
    }
    public void MaintainAlertMeter()
    {
        if(GetAlertLevel() > 0 && GetAlertLevel() < 1) CreateAlertMeter();
        if(GetAlertLevel() == 0 || GetAlertLevel() == 1) DestroyAlertMeter();
        if(isDamaged()) DestroyAlertMeter();
        UpdateAlertMeter();
    }

    void MoveToTarget()
    {
        if(Vector3.Distance(transform.position, target) > stopBorder)
            GetComponent<NavMeshAgent>().SetDestination(target);
        
    }
    void SetTarget(Vector3 _target){
        target = _target;
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
    
    void ChaseFoe()
    {
        if(currentFoe != null)
            SetTarget(currentFoe.transform.position);
        MoveToTarget();
    }

    public bool isDamaged()
    {
        if(GetHealth() < GetMaxHealth()){
            return true;
        }
        return false;
    }

    public bool canMelee()
    {
        if(currentFoe != null && !startmeleeCooldown){
            if (Vector3.Distance(currentFoe.transform.position,transform.position) < meleeReach && currentFoe.GetComponent<DamageableObject>()) 
                return true;
            else 
                return false;

        }
        else
        {   if(startmeleeCooldown)
            {
                meleeTimer -= Time.deltaTime;
                if(meleeTimer <= 0){
                    startmeleeCooldown = false;
                }
            }   
            return false;
        }
    }
    public void Melee()
    {
        print("Hit " + currentFoe.name);
        currentFoe.GetComponent<DamageableObject>().DealDamage(meleeDamage,gameObject);
        startmeleeCooldown = true;
        meleeTimer = meleeCooldown;
    }

    void Wander()
    {       
        if(!startWandering) timer = wanderPauseTime;

         timer += Time.deltaTime;
   
        if(timer >= wanderPauseTime)
        {
            Vector3 newPos  = RandomNavSphere(transform.position, wanderDistance, -1);
            
            if (AvoidingBorder(newPos))
            {
                if(!isHeightRestricted(newPos))
                {
                    SetTarget(newPos);
                    timer = 0;
                }
            }
        }

        MoveToTarget();

        startWandering = true;
    }
    bool AvoidingBorder(Vector3 newDest)
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(newDest, out hit, NavMesh.AllAreas))
        {
            if(hit.distance < wanderBorderAvoidanceDistance){
                return false;
            }
        }
        return true;
    }
    bool isHeightRestricted(Vector3 newDest)
    {
        if(newDest.y < transform.position.y + wanderHeightRestriction)
        {
            return false;
        }
        else
        {
            return true;
        }
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(searchPosition, Vector3.one * 0.25f);

    }
}
