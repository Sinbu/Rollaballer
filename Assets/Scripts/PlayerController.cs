using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpHeight;
    public Text countText;
    public Material standardBallMaterial;
    public Material jumpMaterial;
    public Material boostMaterial;
    public Material boostMaterialUsed;
    public Material cameraMaterial;

    private General generalObject;
    private Rigidbody rb;
    private Renderer rendererComponent;
    private float distToGround;
    private Vector3 playerLastPosition;
    // private float softMaxVelocity = 2.0f;

    private bool gotJumpPowerup = false;
    private bool gotCameraPowerup = false;
    private bool gotBoostPowerup = false;
    private int powerupCount = 0;

    // Boost Power up
    private bool hasBoosted = false;
    private float boostCooldownMaxtime = 2.0f;
    private float boostCooldownTimer = 0.0f;
    private float boostForce = 400.0f;

    // Stuff for ball texture
    private bool scaleSwitch;
    private Vector2 uvAnimationRate = new Vector2(0.5f, 0.5f);
    Vector2 uvOffset = Vector2.zero;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rendererComponent = GetComponent<Renderer>();
        generalObject = GameObject.Find("General Scripts").GetComponent<General>();
    }

    bool IsGrounded() {
        // return Physics.Raycast(this.transform.position, Vector3.down, 0.6f);
        return Physics.SphereCast(new Ray(this.transform.position, Vector3.down), 0.2f, 0.4f);
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

        // TODO: Add "speed limit" (top speed) here

        // Record players last known location, using the center of the ball
        if (Physics.Raycast(this.transform.position, Vector3.down, 0.6f)) {
            playerLastPosition = this.transform.position;

            // For Debugging collisions (sp?)
            /* foreach(var ray in Physics.RaycastAll(this.transform.position, Vector3.down, 0.6f)) {
             *  print(ray.transform.name);
            } */ 
        }
    }

    void Update() {
        // Return player if they are out of bounds TODO: Do this better
        if (this.transform.position.y <= playerLastPosition.y - 10.0f) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; 
            this.transform.position = playerLastPosition;
        }
        // Cheat TODO: make cheats a part of a global setting or something
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            // Go to platform 1 with the 1 button on the keyboard
            this.generalObject.SetPlayerToPlatform(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            // Go to platform 2 with the 2 button on the keyboard
            this.generalObject.SetPlayerToPlatform(2);
        }

        // Boost powerup used
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && this.gotBoostPowerup && this.hasBoosted == false && IsGrounded()) {
            this.Boost();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.Jump();
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            CameraController.zoomFactor = CameraController.zoomFactor == 1.0f ? 0.5f : 1.0f;
        }

        // Boost Cooldown
        if (this.boostCooldownTimer > 0) {
            this.boostCooldownTimer -= Time.deltaTime;
        } else {
            this.hasBoosted = false;
            this.RenderBallAfterPowerup();
        }

        // Mobile
        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 1) {
                this.Jump();
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 2) {
                this.Boost();
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
        if (this.gotJumpPowerup || this.gotCameraPowerup || this.gotBoostPowerup) {
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
                if (powerupCount >= 1) {
                    this.rendererComponent.materials[0].SetTextureOffset("_MainTex", uvOffset);
                }
                if (powerupCount >= 2) {
                    this.rendererComponent.materials[1].SetTextureOffset("_MainTex", -uvOffset);
                }
                if (powerupCount == 3) {
                    this.rendererComponent.materials[2].SetTextureOffset("_MainTex", uvOffset * 2);
                }
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
            this.RenderBallAfterPowerup();
            this.jumpHeight = 30;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("CameraPowerup")) {
            this.gotCameraPowerup = true;
            this.RenderBallAfterPowerup();
            // TODO: Implement Camera Powerup
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("BoostPowerup")) {
            this.gotBoostPowerup = true;
            this.RenderBallAfterPowerup();
            // TODO: Implement boost
            Destroy(other.gameObject);
        }
    }

    // Helper functions
    private void Jump(bool ignoreIfGrounded = false) {
        if (this.IsGrounded() || ignoreIfGrounded) {
            this.rb.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * speed);
        }
    }

    private void Boost(bool ignoreIfGrounded = false) {
        if ((this.IsGrounded() || ignoreIfGrounded) && this.gotBoostPowerup && this.hasBoosted == false) {
            this.hasBoosted = true;
            Vector3 normalizedMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            // Strip away all velocity not in the direction of desired movement.
            rb.angularVelocity = Vector3.zero;
            rb.velocity = normalizedMovement * Mathf.Max(Vector3.Dot(normalizedMovement, rb.velocity), 0);

            this.rb.AddForce(normalizedMovement * boostForce);
            this.boostCooldownTimer = this.boostCooldownMaxtime;
            this.RenderBallAfterPowerup();
        }
    }

    private void RenderBallAfterPowerup() {
        // Will color the ball and set the count for powerups
        var materials = new List<Material>();
        powerupCount = 0;

        if (this.gotJumpPowerup) {
            materials.Add(jumpMaterial);
            powerupCount++;
        }
        if (this.gotCameraPowerup) {
            materials.Add(cameraMaterial);
            powerupCount++;
        }
        if (this.gotBoostPowerup) {
            if (this.hasBoosted == true) {
                // Cooldown state
                materials.Add(boostMaterialUsed);
            } else {
                materials.Add(boostMaterial);
            }
            powerupCount++;
        }
        materials.Add(standardBallMaterial);

        this.rendererComponent.materials = materials.ToArray();
    }
}
