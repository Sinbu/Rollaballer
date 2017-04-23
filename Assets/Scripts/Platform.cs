using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    public int number;
    public GameObject startingPoint;

    private HashSet<PickUp> outstandingPickUps = new HashSet<PickUp>();
    private int pickUpsCollected = 0;

    void Start() {
        General.Instance.RegisterPlatform(this);
    }

    public void RegisterPickUp(PickUp pickUp) {
        outstandingPickUps.Add(pickUp);
    }

    public void CollectPickUp(PickUp pickUp) {
        outstandingPickUps.Remove(pickUp);
        pickUpsCollected++;
        if (General.Instance != null) {
            General.Instance.OnPickUpCollected();
        }
    }

    public bool IsPassed { get { return outstandingPickUps.Count == 0; } }

    public int PickupsCollected { get { return pickUpsCollected; } }
}
