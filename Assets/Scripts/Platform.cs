using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    public int number;
    public bool passed = false;
    public GameObject startingPoint;
	
	void Start () {
        General.Instance.RegisterPlatform(this);		
	}
}
