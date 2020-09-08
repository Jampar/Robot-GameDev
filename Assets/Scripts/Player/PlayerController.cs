using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    #region Speed Variables
    float speed;    //Speed for Character locomotion
    float lerpSpeed = 7.0f;     //Speed for Dynamic Animation Interpolation
    float sprintSpeed = 6f;
    float walkSpeed = 3f;
    float crouchSpeed = 2.0f;
    float jumpSpeed = 5.0f;

    Vector3 surfaceNormal;
    #endregion

    public bool LockCursor;     //Boolean state for locking the cursor

    #region Dynamic Animation Bones 
    public Transform headBone;  //Transform of the head bone
    public Transform chestBone; //Transform of the chest bone

    public Transform hipBone;   //Transform of the hip bone

    public bool matchHead;
    public bool matchChest;
    public bool matchHip;
    public bool matchBody;
    #endregion

	#region Player Locomotion
    private Vector3 moveDirection;	//Vector for the direction of Character locomotion
	private const float _g = -9.81f;	//Gravity constant

    [HideInInspector]
    public enum MovementTypeLookup { Idle, Walk, Sprint, Backwards};
    [HideInInspector]
    public static MovementTypeLookup movementType;

    bool isCrouched = false;

    float movementMagnitude;

    float groundStepLimit = 0.1f;

    RaycastHit hit;
    public string[] movePointRaycastLayerMasks;
	#endregion

    #region Global Component Variables
    CharacterController characterController;	//Component Variable for CharacterController
    Animator animator;		//Component Variable for Animator
    #endregion

    #region Player Ground Tracking
    float groundingDistance = 0.2f;
    Vector3 groundingDirection = Vector3.down;
    #endregion 


    // Use this for initialization
    void Start()
    {
		//Assign Animator Component
        animator = GetComponent<Animator>();
		//Assign CharacterController Component
        characterController = GetComponent<CharacterController>();

		//Check Boolean State
        if (LockCursor == true)
        {
			//Lock the Cursor
            Cursor.lockState = CursorLockMode.Locked;
			//Make the Cursor invisible
            Cursor.visible = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
		//Method to handle Character locomotion
        CharacterLocomotion();
    }
	//LateUpdate is called at the end of the Game Logic execution cycle once per frame.
    void LateUpdate()
    {
        //Method to handle the code controlled animations
        DynamicAnimations();
        
    }

    void DynamicAnimations()
    {
		//Get Main Camera GameObject
        GameObject camera = Camera.main.gameObject;

        /*
        if(GetComponent<PlayerCombat>().isAiming() && movementType != MovementTypeLookup.Sprint){
            chestBone.LookAt(camera.transform.position + camera.transform.forward * 10);
        }	
        */

        if (matchChest || GetComponent<PlayerCombat>().isAiming())
        {
            //Match the chest bone's Y rotation to the camera's Y rotation
            chestBone.eulerAngles = new Vector3(chestBone.rotation.eulerAngles.x,
                                                camera.transform.rotation.eulerAngles.y,
                                                chestBone.rotation.eulerAngles.z);
        }

        if(matchHead){			
            //Match the head bone's X rotation to the camera's X rotation
            headBone.eulerAngles = new Vector3(camera.transform.rotation.eulerAngles.x,
                                                headBone.transform.rotation.eulerAngles.y,
                                                headBone.rotation.eulerAngles.z);
        }
		if(matchHip && movementType != MovementTypeLookup.Backwards){			
            //Check if the player is moving					
            if (movementMagnitude > 0)
            {
                //Create a rotation to the movement vector
                Quaternion movementVectorRotation = Quaternion.LookRotation(moveDirection);
                //Match the hip bone's Y rotation to the movement rotation's Y value 
                hipBone.eulerAngles = new Vector3(hipBone.eulerAngles.x,
                                                    movementVectorRotation.eulerAngles.y + 180,		//180 degrees added to adjust for blender coordinate system
                                                    hipBone.eulerAngles.z);
            }
            else
            {

            }
        }
    }

    void CharacterLocomotion()
    {
        //if(Input.GetButtonDown("Crouch")) isCrouched = !isCrouched;

        //If grounded
        if (isGrounded())
        {
            //Calculate the movement vector from input
            moveDirection = CalculateMovementVector();
        }
        else
        {
        }

        if(isCrouched)
        {
            ChangeControllerDimensions(new Vector3(0,2f,0),4);
        }else
        {
            ChangeControllerDimensions(new Vector3(0,3,0),6);

        }


        //Assign correct movement type for the input vector
        SetCorrectMovementType(moveDirection);

        //Enable animations to match locomotion
        ApplyMovementTypeAnimation();

        //Apply gravity to the movement vector
        moveDirection.y += _g * Time.deltaTime;


        if (matchBody)
        {
            if(moveDirection.magnitude != 0)
                transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        }
        

    }

    float CameraLookAngle(){
        RaycastHit hit;
        Vector3 target = transform.forward;
        int layer_mask = LayerMask.GetMask(movePointRaycastLayerMasks);
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray, out hit, layer_mask))
        {
            target = new Vector3(hit.point.x,transform.position.y,hit.point.z) - transform.position;
        }
        
        return Vector3.SignedAngle(target,transform.forward,Vector3.up);
    }

    Vector3 CalculateMovementVector(){

        //Receive axis input
        Vector3 movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Multiply by correct speed value for movement type
        movementVector *= MatchSpeedForMovementType();

        //Convert movement vector to world space
        movementVector = transform.TransformDirection(movementVector);

        movementVector = Quaternion.AngleAxis(CameraLookAngle(), Vector3.up) * movementVector;

        //Collect the square magnitude for movement vector
        movementMagnitude = movementVector.sqrMagnitude;

        //If Jump button is pressed
        if (Input.GetButton("Jump")) 
        {
            //Set movement vector y value
            movementVector.y = jumpSpeed;
        }
        else if(isGrounded())
        {
            //Match the movement direction to ground angle
            movementVector = MatchVectorToGroundAngle(movementVector);
        }

        return movementVector;
    }

    void ApplyMovementTypeAnimation(){
         
        //Set movement animation
        animator.SetInteger("MovementType", (int)movementType);
        animator.SetBool("Falling", !isGrounded());

        //animator.SetBool("Crouched",isCrouched);
    }


    void SetCorrectMovementType(Vector3 movementVector)
    {
      
		//If the player is moving
        if (isMoving())
        {
            //Rotate gameobject to match the movement direction
            MatchRotationToMovementDirection();

            //Change movement type to walking
            ChangeMovementType(MovementTypeLookup.Walk);

            //If left shift is down
            if(Input.GetKey(KeyCode.LeftShift)){

                //Change movement type to sprinting
                ChangeMovementType(MovementTypeLookup.Sprint);
            }

            if(Input.GetKey(KeyCode.S))
            {
                ChangeMovementType(MovementTypeLookup.Backwards);
            }
        }
        else
        {
            //Change movement type to idling
            ChangeMovementType(MovementTypeLookup.Idle);
        }
    }
    
    void ChangeMovementType(MovementTypeLookup type){

        movementType = type;

    }
    

    void ChangeControllerDimensions(Vector3 centre, float height){
        GetComponent<CharacterController>().center = centre;
        GetComponent<CharacterController>().height = height;
    }
    
    float MatchSpeedForMovementType(){

        if(isCrouched)
        {
            return crouchSpeed;
        }
        if(movementType == MovementTypeLookup.Sprint)  
        {
            return sprintSpeed; 
        }
        else 
        {
            return walkSpeed;
        }

    }

    void MatchRotationToMovementDirection()
    {

		//Interpolate current Y rotation to the camera's Y rotation
        transform.eulerAngles = AngleLerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x,
                                                                             Camera.main.transform.eulerAngles.y,
                                                                             transform.eulerAngles.z),
                                                                             lerpSpeed * Time.deltaTime);
    }


    bool isGrounded(){

        RaycastHit hit;

        if(Physics.Raycast(transform.position, groundingDirection, out hit, groundingDistance))
        {      
            return true;
        }
        else
        {
            return false;
        }

    }

    bool isMoving(){
        
        if(movementMagnitude != 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool isSprinting()
    {
        if(movementType == MovementTypeLookup.Sprint) return true;
        else return false;
    }
    

    Vector3 MatchVectorToGroundAngle(Vector3 movementVector){

        //Calculate the surface normal of the below face
        surfaceNormal = FindBelowSurfaceNormal();

        //Collect the negative surface normal for ground detection
        groundingDirection = -surfaceNormal;

        //Calculate the tangent to the surface normal
        Vector3 tangent = FindTangent(surfaceNormal);

        //If player is moving
        if(movementMagnitude != 0 && tangent.y < groundStepLimit)
        {
            //Move the player along the tangent
            movementVector.y = tangent.y * MatchSpeedForMovementType();
        }
        else
        {
            movementVector.y = 0;
        }

        return movementVector;
    }

    Vector3 FindTangent(Vector3 normal){

        Vector3 tangent;

        Vector3 t1 = Vector3.Cross( normal, hipBone.transform.right );
        Vector3 t2 = Vector3.Cross( normal, -hipBone.transform.right );

        if( t1.magnitude > t2.magnitude )
        {
            tangent = t1;
        }
        else
        {
            tangent =  t2;
        }
        return tangent.normalized;

    }

    Vector3 FindBelowSurfaceNormal(){

        if(Physics.Raycast(transform.position, Vector3.down,out hit))
        {      
            return hit.normal;
        }
        else 
        {
            return Vector3.zero;
        }

    }


	//Interpolate an angle properly
    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
		//Lerp each dimension individually
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);

		//Compile into Vector3
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
		
        return Lerped;
    }

    void OnDrawGizmos(){
        if(isGrounded())    Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + groundingDirection * groundingDistance);

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,transform.position + FindBelowSurfaceNormal());

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position,transform.position + FindTangent(FindBelowSurfaceNormal()));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,transform.position + moveDirection);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,0.2f);
    }
}
