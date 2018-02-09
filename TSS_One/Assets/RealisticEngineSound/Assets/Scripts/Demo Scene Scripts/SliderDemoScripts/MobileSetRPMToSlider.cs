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

public class MobileSetRPMToSlider : MonoBehaviour {

    RealisticEngineSound_mobile controllerScript;
    public GameObject controllerGameobject; // GameObject with Realistic Engine Sound script
    public Slider rpmSlider; // UI slider to set RPM
    public Text rpmText; // UI text to show current RPM
    public Toggle ReversingCheckbox; // UI toggle for is reversing
    public bool simulated = true;
    public GameObject gasPedalButton;
    CarSimulator carSimulator;

    private void Start()
    {
        controllerScript = controllerGameobject.GetComponent<RealisticEngineSound_mobile>();
        rpmSlider.maxValue = controllerScript.maxRPMLimit; // set UI slider's max value to Realistic Engine Sound script's setted maximum RPM
        carSimulator = gasPedalButton.GetComponent<CarSimulator>();
    }
    public void SetRPM()
    {
        if (controllerScript != null)
            controllerScript.engineCurrentRPM = rpmSlider.value; // set Realistic Engine Sound script's current RPM to slider value
    }
    private void Update()
    {
        if (!simulated)
        {
            rpmText.text = "Engine RPM: " + rpmSlider.value.ToString("0"); // show current RPM
            if (controllerScript != null)
                controllerScript.engineCurrentRPM = rpmSlider.value; // set Realistic Engine Sound script's current RPM to slider value
        }
        else
        {
            rpmText.text = "Engine RPM: " + carSimulator.rpm.ToString("0"); // show current RPM
            controllerScript.engineCurrentRPM = carSimulator.rpm;
            rpmSlider.value = carSimulator.rpm;
        }
    }
    public void ReverseGearCheckbox() // enable/disable reverse gear
    {
        if (controllerScript != null)
        {
            if (controllerScript.enableReverseGear) // turn off gas pedal pressing
            {
                controllerScript.enableReverseGear = false;
                ReversingCheckbox.gameObject.SetActive(false);
                ReversingCheckbox.isOn = false;
            }
            else // turn on gas pedal pressing
            {
                controllerScript.enableReverseGear = true;
                ReversingCheckbox.gameObject.SetActive(true);
            }
        }
    }
    public void Reversing() // enable/disable reversing sound
    {
        if (controllerScript != null)
        {
            if (controllerScript.isReversing) // turn off reversing sound
                controllerScript.isReversing = false;
            else
                controllerScript.isReversing = true;
        }
    }
    public void RPMLimit() // enable/disable rpm limit
    {
        if (controllerScript != null)
        {
            if (controllerScript.useRPMLimit) // turn off rpm limit
                controllerScript.useRPMLimit = false;
            else
                controllerScript.useRPMLimit = true;
        }
    }
}
