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
    public Text resetText;
    public TextMesh hint1;
    public Area currentArea;

    private PlayerController playerController;
    private int resetCounter = 0;

    // States
    private bool endedGame = false;

    private Dictionary<int, Area> areas = new Dictionary<int, Area>();

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
        resetText.gameObject.SetActive(false);
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
            if (resetCounter == 1) {
                resetCounter = 0;
                SceneManager.LoadScene("Minigame");
            }
            if (resetCounter == 0) {
                resetCounter++;
                this.resetText.gameObject.SetActive(true);
                CooldownTimer.StartTimer(2.0f, resetCooldownTimer);
            }
        } else if (!playerController.GetComponent<Rigidbody>().useGravity) {
            if (Input.touchCount > 1 || Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Xbox A Button") > 0) {
                // If the player hits jump on their input, start the game.
                this.playerController.GetComponent<Rigidbody>().useGravity = true;
            }
        } else if (!playerController.enabled) {
            startTime -= Time.deltaTime;
            if (startTime > 0) {
                int secondsLeft = (int)Mathf.Ceil(startTime);
                countdownText.text = secondsLeft.ToString() + new string('!', secondsLeft);
            } else {
                playerController.enabled = true;
                countText.enabled = true;
                timeText.enabled = true;
                countdownText.enabled = false;
            }
        } else if (!endedGame) {
            timer += Time.deltaTime;
            timeText.text = "Time: " + timer.ToString("0.00");
        }
    }

    private void resetCooldownTimer() {
        this.resetText.gameObject.SetActive(false);
        this.resetCounter = 0;
    }

    // Public Methods
    public void SetPlayerToArea(int areaNumber = 1) {
        if (this.areas.ContainsKey(areaNumber)) {
            this.currentArea = this.areas[areaNumber];
            this.playerController.transform.position = currentArea.startingPoint.transform.position;
            this.playerController.GetComponent<Rigidbody>().velocity = this.playerController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    public void RegisterPlatform(Area area) {
        if (this.areas.ContainsKey(area.number)) {
            throw new System.Exception("A area attempted to registered with number already in use " + area.number);
        }
        areas[area.number] = area;
    }

    public void OnPickUpCollected() {
        countText.text = "Count: " + currentArea.PickupsCollected;
        if (currentArea.IsPassed) {
            SetPlayerToArea(currentArea.number + 1);
            endedGame = true;
            winText.text = "You win! Your Time: " + this.timer.ToString("0.000");
        }
    }
}
