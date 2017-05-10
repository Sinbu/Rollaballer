using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
    public float speed;
    public Vector3 destinationPoint;
    private Vector3 originPoint;
    private Rigidbody rb;
    private bool reachedDestination;
    private float distanceBetweenPoints;
    private Vector3 midPoint;

    public enum MoverType {Linear, StartAndFinishDecay, MidpointDecay, Decay};

    public MoverType type;

    void Start() {
        reachedDestination = false;
        originPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        distanceBetweenPoints = Vector3.Distance(this.originPoint, this.destinationPoint);
        midPoint = Vector3.Lerp(this.originPoint, this.destinationPoint, 0.5f);
    }

    public void FixedUpdate() {
        if (!reachedDestination) {
            var distance = Vector3.Distance(transform.position, this.destinationPoint);
            if (distance > .1) {
                this.Move(this.transform.position, this.destinationPoint);
            } else {
                reachedDestination = true;
            }
        } else {
            var distance = Vector3.Distance(transform.position, this.originPoint);
            if (distance > .1) {
                this.Move(transform.position, this.originPoint);
            } else {
                reachedDestination = false;
            }
        }
    }

    void Move(Vector3 pos, Vector3 towards) {
        Vector3 direction = (towards - pos).normalized;
        var calculatedSpeed = speed;

        switch (type) {
            case MoverType.Linear:
                break;
            case MoverType.StartAndFinishDecay:
                // speed / (abs(position-midpoint) + 1)
                calculatedSpeed = speed / (Mathf.Abs(Vector3.Distance(this.midPoint,pos)) + 1);
                break;
            case MoverType.MidpointDecay:
                calculatedSpeed = speed / ((distanceBetweenPoints / 2) / (Mathf.Abs(Vector3.Distance(pos, this.midPoint)) + 1));
                break;
            case MoverType.Decay:
                calculatedSpeed = Vector3.Distance(pos, towards) / (distanceBetweenPoints / 2) * speed;
                break;
        }
        rb.MovePosition(this.rb.position + direction * calculatedSpeed * Time.deltaTime);
    }
}