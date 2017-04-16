using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownTimer : MonoBehaviour {
    public delegate void TimerCallback();

    private float timeLeft;
    private TimerCallback callback;

    public static void StartTimer(float duration, TimerCallback callback)
    {
        StartTimer(duration, callback, GameObject.Find("Timer"));
    }

    public static void StartTimer(float duration, TimerCallback callback, GameObject currentObject) {
        CooldownTimer timer = currentObject.AddComponent<CooldownTimer>();
        timer.timeLeft = duration;
        timer.callback = callback;
    }
    
    void Update() {
        this.timeLeft -= Time.deltaTime;
        if (this.timeLeft <= 0)
        {
            this.callback();
            Destroy(this);
        }
    }
}