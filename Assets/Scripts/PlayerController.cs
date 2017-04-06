using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float jumpHeight;
	public Text countText;

	private General generalObject;
	private Rigidbody rb;
	private float distToGround;
	private Vector3 playerLastPosition;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		generalObject = GameObject.Find ("General Scripts").GetComponent<General> ();
		distToGround = 0.5f;

		// stairs.SetActive (false);
	}

	bool IsGrounded() {
		return Physics.Raycast(this.transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void FixedUpdate ()
	{
		Vector3 movement;

		if (SystemInfo.deviceType == DeviceType.Handheld) {
			movement = new Vector3 (Input.acceleration.x, 0.0f, Input.acceleration.y);
		} else {
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		}


		rb.AddForce (movement * speed);


	}

	void Update() {
		// Record players last known location
		if (IsGrounded ()) {
			playerLastPosition = this.transform.position;
		}
		// Return player if they are out of bounds TODO: Do this better
		if (this.transform.position.y <= -5.0f) {
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero; 
			this.transform.position = playerLastPosition;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			if (IsGrounded()) {
				rb.AddForce (new Vector3 (0.0f, jumpHeight, 0.0f) * speed);
			}
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			if (CameraController.zoomFactor == 1.0f) {
				CameraController.zoomFactor = 0.5f;
				return;
			}
			CameraController.zoomFactor = 1.0f;
		}

		// Mobile
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				if (IsGrounded ()) {
					rb.AddForce (new Vector3 (0.0f, jumpHeight, 0.0f) * speed);
				}
			}
			/*
			if (Input.GetTouch(i).phase == TouchPhase.Canceled)
			{
			}
			if (Input.GetTouch(i).phase == TouchPhase.Ended)
			{
			}
			if (Input.GetTouch(i).phase == TouchPhase.Moved)
			{
			}
			if (Input.GetTouch(i).phase == TouchPhase.Stationary)
			{
			}
			*/
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick Up")) {
			// Destroy (other.gameObject);
			other.gameObject.SetActive (false);
			generalObject.SetCountText ();
		}
		if (other.gameObject.CompareTag ("JumpPowerup")) {
			other.gameObject.SetActive (false);
			this.jumpHeight = 30;
		}
	}
		
}