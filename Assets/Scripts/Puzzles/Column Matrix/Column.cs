using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    Vector3 movementVector;
    public Vector3[] movementVectors = {Vector3.forward,Vector3.right};

    Animator anim;

    public int movementIndex;

    [HideInInspector]
    public ColumnMatrixPuzzle puzzle;

    bool stopAtNextPoint = false;
    [HideInInspector]
    public Vector3 startPoint;
    [HideInInspector]
    public Vector3 nextPoint;
    Vector3 previousPoint;

    float speed;
    float stasisSpeed = 6;
    float normalSpeed = 3;

    float timeStamp;
    float freezeTime = 10;
    bool timerFirstRun = true;

    bool changeDirection = false;
    bool canMove = true;

    public bool inPlace;
    bool movingToGoal;
    
    // Start is called before the first frame update
    void Start()
    {
        movementVector = movementVectors[movementIndex];
        speed = normalSpeed;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Move();
    } 

    public void Move()
    {
        //Is in line with matrix point
        if(isInlineWithPoint())
        {
            //If has been instructed to change direction
            if(changeDirection)
            {
                //Change movment axis
                ChangeMovementDirection();
            }

            //Calculate new destination
            SetMovementPoint(transform.position + (movementVector * puzzle.floorSpacing));

            //If point is out of bounds
            if(!puzzle.inBounds(nextPoint) && !puzzle.IsSharedDestination(nextPoint))
            {
                //Reverse movement direction
                movementVector *= -1;
                //Recalculate new destination
                SetMovementPoint(transform.position + movementVector * puzzle.floorSpacing);
            }

            //If has been instructed to stop
            if(stopAtNextPoint)
            {
                if(isPointGoal(transform.position))
                {
                    FloorPoint point = puzzle.GetPoint(transform.position);
                    point.hasColumnOn = true;

                    SetMovementPoint(transform.position - puzzle.columnOffset);
                    movingToGoal = true;
                }
                else
                {
                    RunFreezeTimer();
                }
            } 
        }

        if(CanGoForward() || movingToGoal)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPoint,speed * Time.deltaTime);
            if(movingToGoal && Vector3.Equals(transform.position,nextPoint)) inPlace = true;
        }
    }
    
    

    void RunFreezeTimer()
    {
        if(timerFirstRun == true){
            timeStamp = Time.time + freezeTime;
            timerFirstRun = false;
            speed = stasisSpeed;
            canMove = false;
        }

        if(Time.time >= timeStamp) 
        {
            stopAtNextPoint = false;
            timerFirstRun = true;
            speed = normalSpeed;
            canMove = true;
        }
    }

    public void StopAtNextPoint()
    {
        stopAtNextPoint = true;
    }

    bool isInlineWithPoint(){
        foreach(FloorPoint point in puzzle.floorPoints)
        {
            if(Vector3.Equals(point.point, transform.position - new Vector3(0,12,0)))
            {
                point.hasColumnOn = true;
                return true;
            }
            point.hasColumnOn = false;
        }
        return false;
    }

    bool isPointGoal(Vector3 test){
        return puzzle.GetPoint(test).isGoal;
    }

    bool CanGoForward(){
        if(!canMove) return false;
        else if(Physics.Linecast(transform.position,transform.position + movementVector * (puzzle.floorSpacing))){
            return false;
        }
        return true;
    }

    void SetMovementPoint(Vector3 newPoint)
    {
        previousPoint = nextPoint;
        nextPoint = newPoint;
    }

    void ChangeMovementDirection(){
        movementIndex++;
        if (movementIndex >= movementVectors.Length) movementIndex = 0;
        movementVector = movementVectors[movementIndex];

        if (movementVector == movementVectors[0]) anim.SetTrigger("TurnF");
        if (movementVector == movementVectors[1]) anim.SetTrigger("TurnR");

        print(movementVector);
        changeDirection = false;
    }

    public void StartMovingAgain()
    {
        stopAtNextPoint = false;
    }

    private void OnDrawGizmos() 
    {
        if(nextPoint != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawSphere(nextPoint, 0.5f);

            if (isInlineWithPoint())
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, transform.position - puzzle.columnOffset);
            }


            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + movementVector * 2);
        }
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Projectile>())
        {
            switch(other.gameObject.GetComponent<Projectile>().damageType)
            {
                case DamageMatrix.DamageTypes.Stasis:
                    StopAtNextPoint();
                    break;
                
                case DamageMatrix.DamageTypes.Impact:
                    if(isInlineWithPoint())
                        changeDirection = true;
                    break;
            }
        }
    }
}

