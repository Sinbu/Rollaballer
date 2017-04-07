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
    public GameObject glassCeiling;

    private PlayerController playerController;
    private int pickupCount = -1;

    // States
    private bool? startedGame = false;
    private bool endedGame = false;

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
        SetCountText();
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
        if (startedGame == true) {
            startTime -= Time.deltaTime;
            if (startTime >= 2) {
                countdownText.text = "3!!!";
            } else if (startTime >= 1) {
                countdownText.text = "2!!";
            } else if (startTime >= 0) {
                countdownText.text = "1!";
            }
            if (startTime <= 0) { 
                startedGame = null; // This is terrible
                playerController.enabled = true;
                countText.enabled = true;
                timeText.enabled = true;
                countdownText.enabled = false;
            }
        }
        if (startedGame == null && endedGame == false) {
            timer += Time.deltaTime;
            timeText.text = "Time: " + timer.ToString("0.00");
        }
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            if ((Input.GetTouch(0).phase == TouchPhase.Began) && startedGame == false) {
                startedGame = true;
                glassCeiling.SetActive(false);
                glassCeiling.transform.position = new Vector3(0, 3.5f, 0);
            }
        } else if (Input.GetKeyDown(KeyCode.Space) && startedGame == false) {
            startedGame = true;
            glassCeiling.SetActive(false);
            glassCeiling.transform.position = new Vector3(0, 3.5f, 0);
        }
    }

    public void SetCountText() {
        this.pickupCount += 1;
        countText.text = "Count: " + pickupCount.ToString();
        if (pickupCount >= 13) {
            endedGame = true;
            winText.text = "You win! Your Time: " + this.timer.ToString("0.000");
        }
    }
}
