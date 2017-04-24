using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float speed;

    public Text countText;
    public Material standardBallMaterial;
    public Material jumpMaterial;
    public Material jumpMaterialUsed;
    public Material boostMaterial;
    public Material boostMaterialUsed;
    public Material cameraMaterial;

    // Ball Rigidbody and calclulations
    private General generalObject;
    private Rigidbody rb;
    private Renderer rendererComponent;
    private float distToGround;
    private Vector3 playerLastPosition;
    private GameObject playerLastPlatform;
    // Assuming player will only be on one platform at a time

    private bool gotJumpPowerup = false;
    private bool gotCameraPowerup = false;
    private bool gotBoostPowerup = false;
    private int powerupCount = 0;

    // Jump Cooldown
    private bool hasJumped = false;
    public float jumpHeight = 20.0f;

    // Boost Power up
    private bool hasBoosted = false;
    private float boostForce = 400.0f;

    // Stuff for ball texture
    private bool scaleSwitch;
    private Vector2 uvAnimationRate = new Vector2(0.5f, 0.5f);
    Vector2 uvOffset = Vector2.zero;

    // Keycode for teleporting ball to start of platform (cheat code)
    private KeyCode playerTeleportKeyMin = KeyCode.Alpha1;
    private KeyCode playerTeleportKeyMaxInclusive = KeyCode.Alpha4;

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
        int movingPlatformMask = 1 << 8; // This is the layer mask for moving platforms

        // NOTE: All moving platforms must use this layer (use the moving platform prefab)

        if (Physics.Raycast(this.transform.position, Vector3.down, 0.6f, ~movingPlatformMask)) {
            playerLastPosition = this.transform.position;

            // This will remove the reference to the last platform the player was on for reset purposes
            if (this.playerLastPlatform != null) {
                this.playerLastPlatform = null;
            }

            // For Debugging collisions (sp?)
            /* foreach(var ray in Physics.RaycastAll(this.transform.position, Vector3.down, 0.6f)) {
             *  print(ray.transform.name);
            } */ 
        }
    }

    void Update() {
        // Return player if they are out of bounds
        if (this.transform.position.y <= playerLastPosition.y - 10.0f) {
            ResetPlayerPosition();
        }
        // Cheat TODO: make cheats a part of a global setting or something
        for (KeyCode key = playerTeleportKeyMin; key <= playerTeleportKeyMaxInclusive; ++key) {
            if (Input.GetKeyDown(key)) {
                this.generalObject.SetPlayerToPlatform(key - playerTeleportKeyMin + 1);
            }
        }

        // Boost powerup used
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && this.gotBoostPowerup && this.hasBoosted == false) {
            this.Boost(true);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.Jump();
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            CameraController.zoomFactor = CameraController.zoomFactor == 1.0f ? 0.5f : 1.0f;
        }

        // Mobile
        if (Input.touchCount > 0) {
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 1) {
                this.Jump();
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 2) {
                this.Boost(true);
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

        // Controller
        if (Input.GetAxis("Xbox A Button") > 0) {
            this.Jump();
        }
        if (Input.GetAxis("Xbox X and B Button") > 0) {
            this.Boost(true);
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

    void OnCollisionEnter(Collision other) {
        if (HasPlatformTag(other)) {
            // Limit the amount of upward mobility from hitting a platform(this only prevents extreme cases)
            if (this.rb.velocity.y > 5.0f) {
                // print("Y Vector reduced");
                var v = this.rb.velocity;
                v.y = 5.0f;
                this.rb.velocity = v;
            }
                
        }
        if (other.gameObject.CompareTag("Moving Platform")) {
            // Remembering player's last platform they were on, used for resetting the player when jumping off a platform
            // Note: We only want this if the player is on TOP of the moving platform (for now)

            // This is seeing if player is above the platform (fully on). player position - player height (approx, since it's a sphere) >= y position + height
            // TODO: Set .45 as a tolerence point for the sphere's half height, or what we consider its feet (its .5 at scale 1, but that's the tip)
            if (this.transform.position.y - 0.45f >= (other.gameObject.transform.position.y + (other.gameObject.transform.lossyScale.y / 2))) {
                this.playerLastPlatform = other.gameObject;
            }
        }

        if (other.gameObject.CompareTag("Wall")) {
            // Limit the amount of upward mobility from hitting a wall (this is a bit more extreme, I don't want the player to get lucky with walls)
            if (this.rb.velocity.y > 1.0f) {
                // print("Y Vector reduced");
                var v = this.rb.velocity;
                v.y = 1.0f;
                this.rb.velocity = v;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.GetComponent<PickUp>().Collect();
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

    // Helper Methods
    private void ResetPlayerPosition() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (this.playerLastPlatform != null) {
            // If player jumped off a platform but didn't hit any other ground, it should reset to the platform
            var platformPosition = this.playerLastPlatform.transform.position;
            playerLastPosition = platformPosition + new Vector3(0, (this.playerLastPlatform.transform.lossyScale.y / 2), 0);
        }
        this.transform.position = playerLastPosition;
    }

    private bool HasPlatformTag(Collision other) {
        return (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Crusher") || other.gameObject.CompareTag("Moving Platform"));
    }

    private void Jump(bool ignoreCheckingIfOnGround = false) {
        if (this.IsGrounded() || ignoreCheckingIfOnGround && this.hasJumped == false) {
            this.hasJumped = true;
            this.rb.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * speed);
            CooldownTimer.StartTimer(0.2f, JumpCooldown);
            this.RenderBallAfterPowerup();
        }
    }

    private void Boost(bool ignoreCheckingIfOnGround = false) {
        Vector3 normalizedMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if ((this.IsGrounded() || ignoreCheckingIfOnGround) && this.gotBoostPowerup && this.hasBoosted == false && normalizedMovement != Vector3.zero) {
            this.hasBoosted = true;
            
            // Strip away all velocity not in the direction of desired movement.
            rb.angularVelocity = Vector3.zero;
            rb.velocity = normalizedMovement * Mathf.Max(Vector3.Dot(normalizedMovement, rb.velocity), 0);

            this.rb.AddForce(normalizedMovement * boostForce);
            CooldownTimer.StartTimer(2.0f, BoostCooldown);
            this.RenderBallAfterPowerup();
        }
    }

    private void BoostCooldown() {
        this.hasBoosted = false;
        this.RenderBallAfterPowerup();
    }

    private void JumpCooldown() {
        this.hasJumped = false;
        this.RenderBallAfterPowerup();
    }

    private void RenderBallAfterPowerup() {
        // Will color the ball and set the count for powerups
        var materials = new List<Material>();
        powerupCount = 0;

        if (this.gotJumpPowerup) {
            if (this.hasJumped) {
                materials.Add(jumpMaterialUsed);
            } else {
                materials.Add(jumpMaterial);
            }
            powerupCount++;
        }
        if (this.gotCameraPowerup) {
            materials.Add(cameraMaterial);
            powerupCount++;
        }
        if (this.gotBoostPowerup) {
            if (this.hasBoosted) {
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
