using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 1;
	public float jumpForce = 0.1f;
	float gravity = 2.0F;

	public float Slerpspeed = 0.1F;

	public bool LockCursor;

	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;

	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	private Vector3 moveDirection = Vector3.zero;
	public Transform cameraTransform;

	private float jumprayheight = 1.5f;
	public bool isGrounded;

	public Animator Anim;


	// Use this for initialization
	void Start ()
	{
		Anim = GetComponentInChildren<Animator> ();
		if (LockCursor == true) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Look ();
		Movement ();
		Jumpchecked ();
	}

	void Jumpchecked(){
		if (Physics.Raycast (transform.position, Vector3.down, jumprayheight)) {
			isGrounded = true;
		} else {
			isGrounded = false;
		}
	}
	void AnimationManager(){


		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
			if (isGrounded == true) {
				Anim.SetBool ("IsRunning", true);
			}
		}else {
			Anim.SetBool ("IsRunning", false);

		}
	}
	public void Look ()
	{

		transform.Rotate (Vector3.up * Input.GetAxis ("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis ("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp (verticalLookRotation, -60, 60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

	}


	public void Movement ()
	{
		CharacterController controller = GetComponent<CharacterController> ();

		if (controller.isGrounded) {

			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);

			if (Input.GetButton("Jump")) {
					 moveDirection.y = jumpForce;
			 }

		}
		moveDirection.y -= gravity  * Time.deltaTime;

		controller.Move (moveDirection * Time.deltaTime * speed);
	}
}
