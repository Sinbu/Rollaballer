using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpHeight;
    public Text countText;
    public Material jumpMaterial;

    private General generalObject;
    private Rigidbody rb;
    private Renderer rendererComponent;
    private float distToGround;
    private Vector3 playerLastPosition;

    private bool gotJumpPowerup = false;

    // Stuff for ball texture
    private bool scaleSwitch;
    private Vector2 uvAnimationRate = new Vector2(0.5f, 0.5f);
    Vector2 uvOffset = Vector2.zero;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rendererComponent = GetComponent<Renderer>();
        generalObject = GameObject.Find("General Scripts").GetComponent<General>();
        distToGround = 0.5f;
    }

    bool IsGrounded() {
        return Physics.Raycast(this.transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void FixedUpdate() {
        Vector3 movement;

        if (SystemInfo.deviceType == DeviceType.Handheld) {
            movement = new Vector3(Input.acceleration.x, 0.0f, Input.acceleration.y);
        } else {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        }
            
        rb.AddForce(movement * speed);
    }

    void Update() {
        // Record players last known location
        if (IsGrounded()) {
            playerLastPosition = this.transform.position;
        }
        // Return player if they are out of bounds TODO: Do this better
        if (this.transform.position.y <= playerLastPosition.y - 10.0f) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; 
            this.transform.position = playerLastPosition;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (IsGrounded()) {
                rb.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * speed);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (CameraController.zoomFactor == 1.0f) {
                CameraController.zoomFactor = 0.5f;
                return;
            }
            CameraController.zoomFactor = 1.0f;
        }

        // Mobile
        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                if (IsGrounded()) {
                    rb.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * speed);
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

    void LateUpdate() {
        // Stuff for the jump powerup texture
        if (gotJumpPowerup) {
            Vector2 step = (uvAnimationRate * Time.deltaTime);

            // Ternary operation... boo ya (this makes the code unreadable, but basically if switch is true, it subtracts the texture scale, and false, the other way
            uvOffset += scaleSwitch ? -step : step;

            if (uvOffset.y >= 10 || uvOffset.y <= 0) {
                scaleSwitch = !scaleSwitch;
                if (uvOffset.y >= 10) {
                    uvOffset.y = uvOffset.x = 10;
                } else if (uvOffset.y <= 0) {
                    uvOffset.y = uvOffset.x = 0;
                }
            }

            if (this.rendererComponent.enabled) {
                this.rendererComponent.materials[0].SetTextureOffset("_MainTex", uvOffset);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            generalObject.SetCountText();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("JumpPowerup")) {
            this.gotJumpPowerup = true;
            this.rendererComponent.materials = new Material[] { jumpMaterial };
            this.jumpHeight = 30;
            Destroy(other.gameObject);
        }
    }
}