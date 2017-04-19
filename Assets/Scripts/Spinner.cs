using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {
    // Spin on the z axis as soon as it hits the top and bottom y-axis value; 0 for either means spin all the time!
    // TODO(sina): Add ability for spinning on all axis!
    public float topValue;
    public float bottomValue;
    public float speed;
    private bool isRotating = false;

    void Update() { 
        if (topValue == 0f || bottomValue == 0f || isRotating) {
            this.transform.Rotate(new Vector3(0, 0, 180) * Time.deltaTime * speed);
        } else if (this.transform.position.y >= topValue || this.transform.position.y <= bottomValue) {
            this.transform.Rotate(new Vector3(0, 0, 180) * Time.deltaTime * speed);
            isRotating = true;
        }
        if (isRotating && this.transform.rotation.eulerAngles.z > 180) {
            this.transform.eulerAngles = new Vector3(0, 0, 0);
            isRotating = false;
        }
    }
}
