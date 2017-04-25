using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {
    public Area area;

    void Start() {
        area.RegisterPickUp(this);
    }

    public void Collect() {
        area.CollectPickUp(this);
        Destroy(this.gameObject);
    }
}
