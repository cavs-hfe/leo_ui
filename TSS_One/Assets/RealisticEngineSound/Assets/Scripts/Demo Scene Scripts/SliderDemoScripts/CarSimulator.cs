//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2017 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSimulator : MonoBehaviour {
    public bool gasPedalPressing = false;
    public float maxRPM = 7000;
    public float idle = 900;
    public float rpm = 0;
    public float accelerationSpeed = 1000f;
    public float decelerationSpeed = 1200f;

    private void Start()
    {
        rpm = idle;
    }
    void Update ()
    {
        if (gasPedalPressing)
        {
            if (rpm <= maxRPM)
                rpm = Mathf.Lerp(rpm, rpm + accelerationSpeed, Time.deltaTime);
        }
        else
        {
            if (rpm > idle)
                rpm = Mathf.Lerp(rpm, rpm - decelerationSpeed, Time.deltaTime);
        }
	}
    public void onPointerDownRaceButton()
    {
        gasPedalPressing = true;
    }
    public void onPointerUpRaceButton()
    {
        gasPedalPressing = false;
    }
}
