using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 3f;
	float lerpSpeed = 5.0f;
	public bool LockCursor;

	public Transform lookBone;

	CharacterController characterController;
	Animator animator;
	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

		if (LockCursor == true) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Movement ();
	}
	void LateUpdate(){
	    Look();
	}

	void Look(){
		GameObject camera = Camera.main.gameObject;
		Vector3 lookPoint = camera.transform.position + camera.transform.forward * 100;
		lookBone.LookAt(lookPoint);
	}

	void Movement ()
	{
		float forward = Input.GetAxis("Vertical");
		float right = Input.GetAxis("Horizontal");
		
		Vector3 moveVector = new Vector3(right,0,forward);

		MovementProcess(moveVector);

		moveVector.y -= 9.81f * Time.deltaTime;

		moveVector = transform.TransformDirection(moveVector);
		characterController.Move(moveVector * speed * Time.deltaTime);

	}

	void MovementProcess(Vector3 movementVector){

		if(movementVector.z > 0){
			MatchCameraRotation();
			animator.SetBool("F_Walking",true);		
		}
		else if(movementVector.z < 0){
			MatchCameraRotation();
			animator.SetBool("B_Walking",true);	
		}
		else{
			animator.SetBool("F_Walking",false);
			animator.SetBool("B_Walking",false);
		}
		
	}

	void MatchCameraRotation(){
		transform.rotation =new Quaternion(transform.rotation.x,
														Mathf.Lerp(transform.rotation.y,Camera.main.transform.rotation.y,lerpSpeed*Time.deltaTime),
														transform.rotation.z,
														Mathf.Lerp(transform.rotation.w,Camera.main.transform.rotation.w,lerpSpeed*Time.deltaTime));
	}
}
