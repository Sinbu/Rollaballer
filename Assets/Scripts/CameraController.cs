using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static float zoomFactor = 1.0f;

    public GameObject player;
    public float updateSpeed = 0.1f;
    public float lowerSpeedDeltaThreshold = 10.0f;
    public float upperSpeedDeltaThreshold = 20.0f;
    public float maxShakeDuration = 0.3f;
    public float maxTranslateAmountStart = 0.2f;
    public float maxRotateAmountStart = 4.0f;
    public float minShakePercent = 0.25f;

    private Vector3 offset;
    private Quaternion initialRotation;
    private Vector3 previousVelocity;
    private float shakeLeftTime = 0.0f;

    void Start() {
        this.offset = transform.position - player.transform.position;
        this.previousVelocity = Vector3.zero;
        this.initialRotation = transform.rotation;
    }

    void LateUpdate() {
        Vector3 targetPosition = player.transform.position + (offset * zoomFactor);
        transform.position = Vector3.Lerp(transform.position, targetPosition, updateSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, updateSpeed);
        if (shakeLeftTime > 0)
        {
            // The goal here is to have the initial shaking be the biggest, and then decay to zero as it stops.
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

        float directionPercent = Mathf.InverseLerp(0, maxDelta, actualDelta);
        float speedPercent = Mathf.InverseLerp(lowerSpeedDeltaThreshold, upperSpeedDeltaThreshold, actualDelta);
        float shakePercentToApply = Mathf.Min(directionPercent, speedPercent);

        if (shakePercentToApply > minShakePercent) {
            shakeLeftTime = shakePercentToApply * maxShakeDuration;
        }
        previousVelocity = currentVelocity;
    }
}
