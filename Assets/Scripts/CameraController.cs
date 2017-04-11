using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static float zoomFactor = 1.0f;

    public GameObject player;
    public float updateSpeed = 0.15f;
    public float shakeVelocityThreshold = 10.0f;
    public float maxShakeDuration = 0.2f;
    public float maxTranslateAmountStart = 0.2f;
    public float maxRotateAmountStart = 4.0f;
    public float shakeLeftTime = 0.0f;

    private Vector3 offset;
    private Quaternion initialRotation;
    private Vector3 previousVelocity;

    void Start() {
        this.offset = transform.position - player.transform.position;
        this.previousVelocity = Vector3.zero;
        this.initialRotation = transform.rotation;
    }

    void Update() {
        Vector3 targetPosition = player.transform.position + (offset * zoomFactor);
        transform.position = Vector3.Lerp(transform.position, targetPosition, updateSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, updateSpeed);
        if (shakeLeftTime > 0)
        {
            float shakePercent = Mathf.Lerp(0, 1, shakeLeftTime / maxShakeDuration);
            transform.position += shakePercent * maxTranslateAmountStart * Random.insideUnitSphere ;
            transform.rotation *= Quaternion.Euler(0, 0, Random.value * shakePercent * maxRotateAmountStart);
            shakeLeftTime -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        Vector3 currentVelocity = player.GetComponent<Rigidbody>().velocity;
        float maxDelta = currentVelocity.magnitude + previousVelocity.magnitude;
        float actualDelta = Vector3.Distance(currentVelocity, previousVelocity);
        // The comparison between actualDelta and maxDelta is making sure that the direction of the velocity changed significantly.
        // The comparison between actualDelta and shakeVelocityThreshold is making sure the player was traveling quickly.
        if (actualDelta > (maxDelta / 2) && actualDelta >= shakeVelocityThreshold) {
            float shakePercentToApply = (actualDelta / maxDelta) * Mathf.Lerp(0, 1, actualDelta / shakeVelocityThreshold);
            shakeLeftTime = shakePercentToApply * maxShakeDuration;
        }
        previousVelocity = currentVelocity;
    }
}
