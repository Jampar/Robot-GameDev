using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	float speed = 5.0f;
	float legLerpSpeed = 5.0f;
	public bool LockCursor;

	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;

	float verticalLookRotation;

	public Transform cameraTransform;
	public Transform chestTransform;
	public Transform hipTransform;

	CharacterController characterController;

	// Use this for initialization
	void Start ()
	{
		characterController = GetComponent<CharacterController>();

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
	}

	
	public void Look ()
	{

		//chestTransform.Rotate (Vector3.forward * Input.GetAxis ("Mouse X") * mouseSensitivityX);

		verticalLookRotation += Input.GetAxis ("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp (verticalLookRotation, -60, 60);
		
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

	}
	public void Movement ()
	{


	}


}
