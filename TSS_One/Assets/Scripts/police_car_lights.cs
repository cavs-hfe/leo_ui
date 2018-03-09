using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class police_car_lights : MonoBehaviour {

    public Light[] Group1;
    public Light[] Group2;
    private float flashTimer;
    public float flashTime;
    private bool flashActive;
    private float delayTimer;
    public float delayTime;
    public int flashCount;
    private int flashCounter;
    private int flashGroup;
    public bool lights_and_sirens;

	// Use this for initialization
	void Start () {
        flashActive = false;
        flashTimer = 0.0f;
        delayTimer = 0.0f;
        flashCounter = 0;
        flashGroup = 1;

	    foreach (Light l in Group1)
        {
            l.enabled = false;
        }
        foreach (Light l in Group2)
        {
            l.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyUp(KeyCode.Space))
        {
            lights_and_sirens = !lights_and_sirens;
        }

        if (lights_and_sirens)
        {
            if (flashActive)
            {
                // update timer
                flashTimer = flashTimer + Time.deltaTime;
                Debug.Log("Update flash timer " + flashTimer);
                if (flashTimer > flashTime)
                {
                    // deactivate flash
                    Debug.Log("Flash Complete");

                    if (flashGroup == 1)
                    {
                        DeactivateLights(Group1);
                        flashGroup = 2;
                    }
                    else
                    {
                        DeactivateLights(Group2);
                        flashGroup = 1;
                    }
                    delayTimer = 0.0f;
                    flashActive = false;
                }
            }
            else
            {
                Debug.Log("Update delay timer " + delayTimer);
                delayTimer = delayTimer + Time.deltaTime;
                if (delayTimer > delayTime)
                {
                    Debug.Log("Delay Complete");
                    // activate flash
                    if (flashGroup == 1)
                    {
                        ActivateLights(Group1);
                    }
                    else
                    {
                        ActivateLights(Group2);
                    }
                    flashTimer = 0.0f;
                    flashActive = true;
                    Debug.Log("Set flashactive: " + flashActive);
                }
            }
        }
	}

    void DeactivateLights(Light[] group)
    {
        Debug.Log("Deactivate Lights");
        foreach (Light l in group)
        {
            l.enabled = false;
        }
    }

    void ActivateLights(Light[] group)
    {
        Debug.Log("Deactivate Lights");
        foreach (Light l in group)
        {
            l.enabled = true;
        }
    }
}
