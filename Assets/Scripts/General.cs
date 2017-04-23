using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class General : MonoBehaviour {
    // Singleton - There should only be one general script (sky hates the name of the class)
    private static General instance;

    public static General Instance { get { return instance; } }

    public Text countdownText;
    public Text countText;
    public Text timeText;
    public Text winText;
    public TextMesh hint1;
    public Platform currentPlatform;

    private PlayerController playerController;

    // States
    private bool? startedGame = null;
    private bool endedGame = false;

    private Dictionary<int, Platform> platforms = new Dictionary<int, Platform>();

    // Timers
    private float startTime = 3.0f;
    private float timer = 0.0f;

    private void Awake() {
        if (instance != null && instance != this) {
            print("Destroying game object: " + this.gameObject);
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void OnDestroy() {
        if (this == instance) {
            instance = null;
        }
    }

    void Start() {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.enabled = false;
        countText.enabled = false;
        timeText.enabled = false;
        countdownText.enabled = true;
        winText.text = "";
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            hint1.text = "Tap = Jump";
            countdownText.text = "Tap to Start";
        } else {
            hint1.text = "Space = Jump";
            countdownText.text = "Press Space to Start";
        }
    }

    void Update() {
        // Can restart game at any time
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("Minigame");
        }
        if (startedGame == false) {
            startTime -= Time.deltaTime;
            if (startTime >= 2) {
                countdownText.text = "3!!!";
            } else if (startTime >= 1) {
                countdownText.text = "2!!";
            } else if (startTime >= 0) {
                countdownText.text = "1!";
            }
            if (startTime <= 0) { 
                startedGame = true; // This is terrible
                playerController.enabled = true;
                countText.enabled = true;
                timeText.enabled = true;
                countdownText.enabled = false;
            }
        }
        if (startedGame == true && endedGame == false) {
            timer += Time.deltaTime;
            timeText.text = "Time: " + timer.ToString("0.00");
        }

        if (startedGame == null && (Input.touchCount > 1 || Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Xbox A Button") > 0)) {
            // if the player hits jump on their input, start the game
            this.StartGame();
        }
    }

    // Helper Methods
    private void StartGame() {
        // Sets state to false (pre game countdown), and restores gravity to player object (no more glass ceiling)
        startedGame = false;
        this.playerController.GetComponent<Rigidbody>().useGravity = true;
    }

    // Public Methods

    public void SetPlayerToPlatform(int platformNumber = 1) {
        if (this.platforms.ContainsKey(platformNumber)) {
            this.currentPlatform = this.platforms[platformNumber];
            if (this.playerController != null && this.currentPlatform != null) {
                this.playerController.transform.position = currentPlatform.startingPoint.transform.position;
                this.playerController.GetComponent<Rigidbody>().velocity = this.playerController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    public void RegisterPlatform(Platform platform) {
        if (this.platforms.ContainsKey(platform.number)) {
            throw new System.Exception("A platform attempted to registered with number already in use " + platform.number);
        }
        platforms[platform.number] = platform;
    }

    public void OnPickUpCollected() {
        countText.text = "Count: " + currentPlatform.PickupsCollected;
        if (currentPlatform.IsPassed) {
            SetPlayerToPlatform(currentPlatform.number + 1);
            endedGame = true;
            winText.text = "You win! Your Time: " + this.timer.ToString("0.000");
        }
    }
}
