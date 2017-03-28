using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public static float zoomFactor = 1.0f;

	private Vector3 offset;

	void Start() {
		this.offset = transform.position - player.transform.position;
	}

	void LateUpdate() {
		transform.position = player.transform.position + (this.offset * zoomFactor);
	}
}
