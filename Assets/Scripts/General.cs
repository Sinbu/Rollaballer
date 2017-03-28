﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General : MonoBehaviour {

	private float startTime = 3.0f;
	private bool? startedGame = false;
	private bool endedGame = false;
	private PlayerController playerController;
	public Text countdownText;
	public Text countText;
	public Text timeText;
	public Text winText;
	public GameObject glassCeiling;
	private float timer = 0.0f;

	private int pickupCount = -1;

	// Use this for initialization
	void Start () {
		playerController = GameObject.Find ("Player").GetComponent<PlayerController> ();
		playerController.enabled = false;
		countText.enabled = false;
		timeText.enabled = false;
		countdownText.enabled = true;
		SetCountText ();
		winText.text = "";
	}

	// Update is called once per frame
	void Update () {
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
				glassCeiling.SetActive(true);
			}
		}
		if (startedGame == null && endedGame == false) {
			timer += Time.deltaTime;
			timeText.text = "Time: " + timer.ToString ("0.00");
		}
		if (Input.GetKeyDown (KeyCode.Space) && startedGame == false) {
			startedGame = true;
			glassCeiling.SetActive(false);
			glassCeiling.transform.position = new Vector3(0, 3.5f, 0);
		}
	}

	void FixedUpdate() {
		
	}

	public void SetCountText() {
		this.pickupCount += 1;
		countText.text = "Count: " + pickupCount.ToString ();
		if (pickupCount >= 13) {
			endedGame = true;
			winText.text = "You win! Your Time: " + this.timer.ToString("0.000");
			// stairs.SetActive (true);
		}
	}
}
