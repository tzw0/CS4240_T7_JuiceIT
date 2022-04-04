using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EyeTrack : MonoBehaviour
{
    public GameObject recipe;
    public float threshold;

    private bool isLooking;
    private DateTime startLook;
    private TimeSpan timeSpendLooking;

    // Start is called before the first frame update
    void Start()
    {
        timeSpendLooking = TimeSpan.Zero;
    }

    // Update is called once per frame
    void Update()
    {
        float lookingVector = Vector3.Dot(transform.forward, (recipe.transform.position - this.transform.position).normalized);
        if (isLooking) {
            if (lookingVector < threshold) {
                timeSpendLooking += (DateTime.Now - startLook);
                // Debug.Log("looking for: " + (DateTime.Now - startLook).ToString());
                isLooking = false;
            }
        } else {
            if (lookingVector > threshold) {
                // Debug.Log("start look");
                startLook = DateTime.Now;
                isLooking = true;
            }
        }
    }

    public TimeSpan GetLookTime() {
        if (isLooking) {
            timeSpendLooking += (DateTime.Now - startLook);
        }
        return timeSpendLooking;
    }

    public void Reset() {
        timeSpendLooking = TimeSpan.Zero;
    }
}
