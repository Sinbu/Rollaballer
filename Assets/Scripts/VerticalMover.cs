using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMover : MonoBehaviour {
    private bool goingUp;
    public float topValue;
    public float bottomValue;
    public float speed;

    void Start() {
        goingUp = true;
    }

    void Update() {
        if (this.transform.position.y >= topValue) {
            goingUp = false;
        }
        if (this.transform.position.y <= bottomValue) {
            goingUp = true;
        }
        if (goingUp == true) {
            this.transform.Translate(0, speed * Time.deltaTime, 0, Space.World);
        }
        if (goingUp == false) {
            this.transform.Translate(0, -speed * Time.deltaTime, 0, Space.World);
        }
    }
}
