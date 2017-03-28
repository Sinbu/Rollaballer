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
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rb.AddForce (movement * speed);

		// transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
	}

	void Update() {
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