using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject player;
    public static float zoomFactor = 1.0f;
    private float updateSpeed = 0.15f;
    private Vector3 offset;

    void Start() {
        this.offset = transform.position - player.transform.position;
    }

    void LateUpdate() {
        Vector3 targetPosition = player.transform.position + (offset * zoomFactor);
        transform.position = Vector3.Lerp(transform.position, targetPosition, updateSpeed);
    }
}
