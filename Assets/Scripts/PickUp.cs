using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {
    public Platform platform;

    void Start() {
        platform.RegisterPickUp(this);
    }

    void OnDestroy() {
        platform.CollectPickUp(this);
    }
}
