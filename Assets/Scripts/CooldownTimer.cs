using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownTimerWrapper {

    public delegate void TimerCallback();

    public CooldownTimer timer;

    public static void StartACooldownTimerFor(float t, TimerCallback c) {
        var timeObject = GameObject.Find("Timer");
        GameObject newObject = MonoBehaviour.Instantiate(timeObject);
        CooldownTimer newTimer = newObject.GetComponent<CooldownTimer>();
        newTimer.StartTimer(t, c, newObject);
    }


}

public class CooldownTimer: MonoBehaviour {

    private float time;
    CooldownTimerWrapper.TimerCallback callback;
    private GameObject currentObject;

    public void StartTimer(float t, CooldownTimerWrapper.TimerCallback c, GameObject currentObject) {
        this.time = t;
        this.callback = c;
        this.currentObject = currentObject;
    }

    // Update is called once per frame
    void Update() {
        // Skip the main Timer
        if (this.name == "Timer") {
            return;
        }

        if (this.time > 0) {
            this.time -= Time.deltaTime;
        } else {
            this.callback();
            MonoBehaviour.Destroy(this.currentObject);
            return;
        }
    }
}