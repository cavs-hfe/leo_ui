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

public class RealisticEngineSound : MonoBehaviour {

    public float engineCurrentRPM = 0.0f;
    public bool gasPedalPressing = false;
    [Range(0.0f, 1.0f)]
    public float gasPedalValue = 1; // (simulated or not simulated) 0 = not pressing = 0 engine volume, 0.5 = halfway pressing (half engine volume), 1 = pedal to the metal (full engine volume)
    public enum GasPedalValue { Simulated, NotSimulated } // NotSimulated setting is recommended for joystick controlled games
    public GasPedalValue gasPedalValueSetting = new GasPedalValue();
    [Range(1.0f, 15.0f)]
    public float gasPedalSimSpeed = 5.5f; // simulates how fast the player hit the gas pedal
    public float maxRPMLimit = 7000;
    [Range(0.0f, 1.0f)]
    public float dopplerAmount = 0.8f; // 0 = no doppler effect (louder engine without doppler effect), 1 = full doppler effect (less loud with doppler effect)
    [Range(0.0f, 0.25f)]
    public float optimisationLevel = 0.01f; // audio source with volume level below this value will be destroyed
    public float minDistance = 1; // within the minimum distance the audiosources will cease to grow louder in volume
    public float maxDistance = 100; // maxDistance is the distance a sound stops attenuating at
    public bool isReversing = false; // is car in reverse gear
    public bool useRPMLimit = true; // enable rpm limit at maximum rpm
    public bool enableReverseGear = true; // enable wistle sound for reverse gear

    // idle clip sound
    public AudioClip idleClip;
    public AnimationCurve idleVolCurve;
    public AnimationCurve idlePitchCurve;
    // low rpm clip sounds
    public AudioClip lowOffClip;
    public AudioClip lowOnClip;
    public AnimationCurve lowVolCurve;
    public AnimationCurve lowPitchCurve;
    // medium rpm clip sounds
    public AudioClip medOffClip;
    public AudioClip medOnClip;
    public AnimationCurve medVolCurve;
    public AnimationCurve medPitchCurve;
    // high rpm clip sounds
    public AudioClip highOffClip;
    public AudioClip highOnClip;
    public AnimationCurve highVolCurve;
    public AnimationCurve highPitchCurve;
    public AnimationCurve highVolReversingCurve;
    // maximum rpm clip sound - if RPM limit is enabled
    public AudioClip maxRPMClip;
    public AnimationCurve maxRPMVolCurve;
    public AnimationCurve maxRPMPitchCurve;
    // reverse gear clip sound
    public AudioClip reversingClip;
    public AnimationCurve reversingVolCurve;
    public AnimationCurve reversingPitchCurve;

    // idle audio source
    private AudioSource engineIdle;

    // low rpm audio sources
    private AudioSource lowOff;
    private AudioSource lowOn;

    // medium rpm audio sources
    private AudioSource medOff;
    private AudioSource medOn;

    // high rpm audio sources
    private AudioSource highOff;
    private AudioSource highOn;

    //maximum rpm audio source
    private AudioSource maxRPM;

    // reverse gear audio source
    private AudioSource reversing;

    //private settings
    private float clipsValue;

    private void Start()
    {
        clipsValue = engineCurrentRPM / maxRPMLimit; // calculate % percentage of rpm
        // create and start playing audio sources
        // idle
        if (idleVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (engineIdle == null)
                CreateIdle();
        }
        //
        // low rpm
        if (lowVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (lowOn == null)
                    CreateLowOn();
            }
            else
            {
                if (lowOff == null)
                    CreateLowOff();
            }
        }
        //
        // medium rpm
        if (medVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (medOn == null)
                    CreateMedOn();
            }
            else
            {
                if (medOff == null)
                    CreateMedOff();
            }
        }
        //
        // high rpm
        if (highVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (highOn == null)
                    CreateHighOn();
            }
            else
            {
                if (highOff == null)
                    CreateHighOff();
            }
        }
        //
        // rpm limiting
        if (maxRPMVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (useRPMLimit) // if rpm limit is enabled, create audio source for it
            {
                if (maxRPM == null)
                    CreateRPMLimit();
            }
        }
        //
        // reversing gear sound
        if (enableReverseGear)
        {
            if (isReversing)
            {
                if (reversingVolCurve.Evaluate(clipsValue) > optimisationLevel)
                {
                    if (reversing == null)
                        CreateReverse();
                }
            }
            else
            {
                if (reversing != null)
                    Destroy(reversing);
            }
        }
    }
    private void OnDisable() // destroy audio sources if Realistic Engine Sound's script is disabled
    {
        if (engineIdle != null)
            Destroy(engineIdle);
        if (lowOn != null)
            Destroy(lowOn);
        if (lowOff != null)
            Destroy(lowOff);
        if (medOn != null)
            Destroy(medOn);
        if (medOff != null)
            Destroy(medOff);
        if (highOn != null)
            Destroy(highOn);
        if (highOff != null)
            Destroy(highOff);

        if (useRPMLimit)
        {
            if(maxRPM != null)
                Destroy(maxRPM);
        }
        if (enableReverseGear)
        {
            if (reversing != null)
                Destroy(reversing);
        }
    }
    private void OnEnable() // recreate all audio sources if Realistic Engine Sound's script is reEnabled
    {
        StartCoroutine(WaitForStart());
    }
    private void Update()
    {
        clipsValue = engineCurrentRPM / maxRPMLimit; // calculate % percentage of rpm

        //clipsValue = Mathf.Round(clipsValue * 100f) / 100f; // <- use this line if car's rpm sound is "shaking"

        // gas pedal value simulation
        if (gasPedalValueSetting == GasPedalValue.Simulated)
        {
            if (gasPedalPressing)
            {
                gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
            }
            else
            {
                gasPedalValue = Mathf.Lerp(gasPedalValue, 0, Time.deltaTime * gasPedalSimSpeed);
            }
        }

        // idle
        if (idleClip != null)
        {
            if (idleVolCurve.Evaluate(clipsValue) > optimisationLevel)
            {
                if (engineIdle == null)
                    CreateIdle();
                else
                {
                    engineIdle.volume = idleVolCurve.Evaluate(clipsValue);
                    engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue);
                }
            }
            else
            {
                Destroy(engineIdle);
            }
        }
        //
        // low rpm
        if (lowVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (lowOnClip != null)
                {
                    if (lowOn == null)
                        CreateLowOn();
                    else
                    {
                        lowOn.volume = lowVolCurve.Evaluate(clipsValue) * gasPedalValue;
                        lowOn.pitch = lowPitchCurve.Evaluate(clipsValue);
                        if (lowOff != null)
                        {
                            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue);
                            if (lowOff.volume < 0.1f)
                                Destroy(lowOff);
                        }
                    }
                }
            }
            else
            {
                if (lowOffClip != null)
                {
                    if (lowOff == null)
                        CreateLowOff();
                    else
                    {
                        if (!isReversing)
                        {
                            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue);
                        }
                    }
                    if (lowOn != null)
                    {
                        lowOn.volume = lowVolCurve.Evaluate(clipsValue) * gasPedalValue;
                        lowOn.pitch = lowPitchCurve.Evaluate(clipsValue);
                        if (lowOn.volume < 0.1f)
                            Destroy(lowOn);
                    }
                }
            }
        }
        else
        {
            if (lowOn != null)
                Destroy(lowOn);
            if (lowOff != null)
                Destroy(lowOff);
        }
        //
        // medium rpm
        if (medVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (medOnClip != null)
                {
                    if (medOn == null)
                        CreateMedOn();
                    else
                    {
                        medOn.volume = medVolCurve.Evaluate(clipsValue) * gasPedalValue;
                        medOn.pitch = medPitchCurve.Evaluate(clipsValue);
                    }
                    if (medOff != null)
                    {
                        medOff.volume = medVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                        medOff.pitch = medPitchCurve.Evaluate(clipsValue);
                        if (medOff.volume < 0.1f)
                            Destroy(medOff);
                    }
                }
            }
            else // gas pedal is released
            {
                if (medOffClip != null)
                {
                    if (medOff == null)
                        CreateMedOff();
                    else
                    {
                        if (!isReversing)
                        {
                            medOff.volume = medVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                            medOff.pitch = medPitchCurve.Evaluate(clipsValue);
                        }
                    }
                    if (medOn != null)
                    {
                        medOn.volume = medVolCurve.Evaluate(clipsValue) * gasPedalValue;
                        medOn.pitch = medPitchCurve.Evaluate(clipsValue);
                        if (medOn.volume < 0.1f)
                            Destroy(medOn);
                    }
                }
            }
        }
        else
        {
            if (medOn != null)
                Destroy(medOn);
            if (medOff != null)
                Destroy(medOff);
        }
        //
        // high rpm
        if (highVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (highOnClip != null)
                {
                    if (highOn == null)
                        CreateHighOn();
                    else
                    {
                        if (!isReversing)
                        {
                            highOn.volume = highVolCurve.Evaluate(clipsValue) * gasPedalValue;
                            highOn.pitch = highPitchCurve.Evaluate(clipsValue);
                        }
                    }
                    if (!isReversing)
                    {
                        if (highOff != null)
                        {
                            highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                            highOff.pitch = highPitchCurve.Evaluate(clipsValue);
                            if (highOff.volume < 0.1f)
                                Destroy(highOff);
                        }
                    }
                }
            }
            else // gas pedal is released
            {
                if (highOffClip != null)
                {
                    if (highOff == null)
                        CreateHighOff();
                    else
                    {
                        if (!isReversing)
                        {
                            highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                            highOff.pitch = highPitchCurve.Evaluate(clipsValue);
                        }
                    }
                    if (!isReversing)
                    {
                        if (highOn != null)
                        {
                            highOn.volume = highVolCurve.Evaluate(clipsValue) * gasPedalValue;
                            highOn.pitch = highPitchCurve.Evaluate(clipsValue);
                            if (highOn.volume < 0.1f)
                                Destroy(highOn);
                        }
                    }
                }
            }
        }
        else
        {
            if (highOn != null)
                Destroy(highOn);
            if (highOff != null)
                Destroy(highOff);
        }
        //
        // rpm limiting
        if (maxRPMVolCurve.Evaluate(clipsValue) > optimisationLevel)
        {
            if (maxRPMClip != null)
            {
                if (useRPMLimit) // if rpm limit is enabled, create audio source for it
                {
                    if (maxRPM == null)
                        CreateRPMLimit();
                    else
                    {
                        maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue);
                        maxRPM.pitch = maxRPMPitchCurve.Evaluate(clipsValue);
                    }
                }
            }
            else // missing rpm limit audio clip
            {
                useRPMLimit = false;
            }
        }
        else
        {
            Destroy(maxRPM);
        }
        //
        // reversing gear sound
        if (enableReverseGear)
        {
            if (reversingClip != null)
            {
                if (isReversing)
                {
                    if (reversingVolCurve.Evaluate(clipsValue) > optimisationLevel)
                    {
                        if (reversing == null)
                            CreateReverse();
                        else
                        {
                            if (gasPedalPressing)
                            {
                                if (highOn == null)
                                    CreateHighOn();
                                else
                                {
                                    highOn.volume = highVolReversingCurve.Evaluate(clipsValue) * gasPedalValue;
                                    highOn.pitch = highPitchCurve.Evaluate(clipsValue);
                                }
                                if (highOff != null)
                                {
                                    highOff.volume = highVolReversingCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                    highOff.pitch = highPitchCurve.Evaluate(clipsValue);
                                    if (highOff.volume < 0.1f)
                                        Destroy(highOff);
                                }
                            }
                            else // gas pedal is released
                            {
                                if (highOff == null)
                                    CreateHighOff();
                                else
                                {
                                    highOff.volume = highVolReversingCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                    highOff.pitch = highPitchCurve.Evaluate(clipsValue);
                                }
                                if (highOn != null)
                                {
                                    highOn.volume = highVolReversingCurve.Evaluate(clipsValue) * gasPedalValue;
                                    highOn.pitch = highPitchCurve.Evaluate(clipsValue);
                                    if (highOn.volume < 0.1f)
                                        Destroy(highOn);
                                }
                            }
                            // set reversing sound to setted settings
                            reversing.volume = reversingVolCurve.Evaluate(clipsValue);
                            reversing.pitch = reversingPitchCurve.Evaluate(clipsValue);
                        }
                    }
                    else
                    {
                        if (reversing != null)
                            Destroy(reversing);
                    }
                }
                else
                {
                    if (reversing != null)
                        Destroy(reversing);
                }
            }
            else
            {
                isReversing = false;
                enableReverseGear = false; // disable reversing sound because there is no audio clip for it
            }
        }
        else
        {
            if (isReversing != false)
                isReversing = false;
        }
    }
    private void LateUpdate()
    {
        if(!enableReverseGear)
        {
            if(reversing != null)
            {
                Destroy(reversing); // looks like someone disabled reversing on runtime, destroy this audio source
            }
        }
        // rpm limiting
        if (useRPMLimit) // if rpm limit is enabled, create audio source for it
        {
            if (maxRPM == null)
                CreateRPMLimit();
        }
        else // if disabled, destroy rpm limit's audio source
        {
            if(maxRPM != null)
                Destroy(maxRPM);
        }
    }
    IEnumerator WaitForStart()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f); // this is needed to avoid duplicate audio sources at scene start
            if (engineIdle == null)
                Start();
            break;
        }
    }
    // create audio sources
    // idle
    void CreateIdle()
    {
        if (idleClip != null)
        {
            engineIdle = gameObject.AddComponent<AudioSource>();
            engineIdle.volume = idleVolCurve.Evaluate(clipsValue);
            engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue);
            engineIdle.minDistance = minDistance;
            engineIdle.maxDistance = maxDistance;
            engineIdle.clip = idleClip;
            engineIdle.loop = true;
            engineIdle.spatialBlend = dopplerAmount;
            engineIdle.Play();
        }
    }
    // low
    void CreateLowOff()
    {
        if (lowOffClip != null)
        {
            lowOff = gameObject.AddComponent<AudioSource>();
            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue);
            lowOff.minDistance = minDistance;
            lowOff.maxDistance = maxDistance;
            lowOff.clip = lowOffClip;
            lowOff.spatialBlend = dopplerAmount;
            lowOff.loop = true;
            lowOff.Play();
        }
    }
    void CreateLowOn()
    {
        if (lowOnClip != null)
        {
            lowOn = gameObject.AddComponent<AudioSource>();
            lowOn.volume = lowVolCurve.Evaluate(clipsValue) * gasPedalValue;
            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue);
            lowOn.minDistance = minDistance;
            lowOn.maxDistance = maxDistance;
            lowOn.clip = lowOnClip;
            lowOn.loop = true;
            lowOn.spatialBlend = dopplerAmount;
            lowOn.Play();
        }
    }
    // medium
    void CreateMedOff()
    {
        if (medOffClip != null)
        {
            medOff = gameObject.AddComponent<AudioSource>();
            medOff.volume = medVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
            medOff.pitch = medPitchCurve.Evaluate(clipsValue);
            medOff.minDistance = minDistance;
            medOff.maxDistance = maxDistance;
            medOff.clip = medOffClip;
            medOff.spatialBlend = dopplerAmount;
            medOff.loop = true;
            medOff.Play();
        }
    }
    void CreateMedOn()
    {
        if (medOnClip != null)
        {
            medOn = gameObject.AddComponent<AudioSource>();
            medOn.volume = medVolCurve.Evaluate(clipsValue) * gasPedalValue;
            medOn.pitch = medPitchCurve.Evaluate(clipsValue);
            medOn.minDistance = minDistance;
            medOn.maxDistance = maxDistance;
            medOn.clip = medOnClip;
            medOn.loop = true;
            medOn.spatialBlend = dopplerAmount;
            medOn.Play();
        }
    }
    // high
    void CreateHighOff()
    {
        if (highOffClip != null)
        {
            highOff = gameObject.AddComponent<AudioSource>();
            highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
            highOff.pitch = highPitchCurve.Evaluate(clipsValue);
            highOff.minDistance = minDistance;
            highOff.maxDistance = maxDistance;
            highOff.clip = highOffClip;
            highOff.spatialBlend = dopplerAmount;
            highOff.loop = true;
            highOff.Play();
        }
    }
    void CreateHighOn()
    {
        if (highOnClip != null)
        {
            highOn = gameObject.AddComponent<AudioSource>();
            highOn.volume = highVolCurve.Evaluate(clipsValue) * gasPedalValue;
            highOn.pitch = highPitchCurve.Evaluate(clipsValue);
            highOn.minDistance = minDistance;
            highOn.maxDistance = maxDistance;
            highOn.clip = highOnClip;
            highOn.Play();
            highOn.loop = true;
            highOn.spatialBlend = dopplerAmount;
        }
    }
    // rpm limit
    void CreateRPMLimit()
    {
        if (maxRPMClip != null)
        {
            maxRPM = gameObject.AddComponent<AudioSource>();
            maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue);
            maxRPM.pitch = maxRPMPitchCurve.Evaluate(clipsValue);
            maxRPM.minDistance = minDistance;
            maxRPM.maxDistance = maxDistance;
            maxRPM.clip = maxRPMClip;
            maxRPM.loop = true;
            maxRPM.spatialBlend = dopplerAmount;
            maxRPM.Play();
        }
    }
    // reversing
    void CreateReverse()
    {
        if (reversingClip != null)
        {
            reversing = gameObject.AddComponent<AudioSource>();
            reversing.volume = reversingVolCurve.Evaluate(clipsValue);
            reversing.pitch = reversingPitchCurve.Evaluate(clipsValue);
            reversing.minDistance = minDistance;
            reversing.maxDistance = maxDistance;
            reversing.clip = reversingClip;
            reversing.loop = true;
            reversing.spatialBlend = dopplerAmount;
            reversing.Play();
        }
    }
}
