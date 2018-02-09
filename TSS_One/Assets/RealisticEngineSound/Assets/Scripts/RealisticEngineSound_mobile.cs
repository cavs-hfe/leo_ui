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

public class RealisticEngineSound_mobile : MonoBehaviour {

    public float engineCurrentRPM = 0.0f;
    public float maxRPMLimit = 7000;
    [Range(0.0f, 1.0f)]
    public float dopplerAmount = 0.8f; // 0 = no doplper effect (louder engine without doppler effect), 1 = full doppler effect (less loud with doppler effect)
    [Range(0.0f, 0.25f)]
    public float optimisationLevel = 0.01f; // audio source with volume level below this value will be destroyed
    public float minDistance = 1; // within the minimum distance the audiosources will cease to grow louder in volume
    public float maxDistance = 100; // maxDistance is the distance a sound stops attenuating at
    public bool isReversing = false; // is car in reverse gear - only if reverse gear is enabled
    public bool useRPMLimit = true; // enable rpm limit at maximum rpm
    public bool enableReverseGear = false; // enable this if you would like to use reverse sound in reverse gears

    // idle clip sound
    public AudioClip idleClip;
    public AnimationCurve idleVolCurve;
    public AnimationCurve idlePitchCurve;
    // low rpm clip sounds
    public AudioClip lowOnClip;
    public AnimationCurve lowVolCurve;
    public AnimationCurve lowPitchCurve;
    // medium rpm clip sounds
    public AudioClip medOnClip;
    public AnimationCurve medVolCurve;
    public AnimationCurve medPitchCurve;
    // high rpm clip sounds
    public AudioClip highOnClip;
    public AnimationCurve highVolCurve;
    public AnimationCurve highPitchCurve;
    public AnimationCurve highVolReversingCurve;
    // maximum rpm clip sound - if RPM limit is enabled
    public AudioClip maxRPMClip;
    public AnimationCurve maxRPMVolCurve;
    public AnimationCurve maxRPMPitchCurve;
    // reverse gear clip sound - if reverse gear is enabled
    public AudioClip reversingClip;
    public AnimationCurve reversingVolCurve;
    public AnimationCurve reversingPitchCurve;

    // idle audio source
    private AudioSource engineIdle;

    // low rpm audio sources
    private AudioSource lowOn;

    // medium rpm audio sources
    private AudioSource medOn;

    // high rpm audio sources
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
        // create audio sources
        // idle
        if (idleVolCurve.Evaluate(clipsValue) > 0.01f)
        {
            if (engineIdle == null)
                CreateIdle();
        }
        //
        // low rpm
        if (lowVolCurve.Evaluate(clipsValue) > 0.01f)
        {
            if (lowOn == null)
                CreateLowOn();
        }
        //
        // medium rpm
        if (medVolCurve.Evaluate(clipsValue) > 0.01f)
        {
            if (medOn == null)
                CreateMedOn();
        }
        //
        // high rpm
        if (highVolCurve.Evaluate(clipsValue) > 0.01f)
        {
            if (highOn == null)
                CreateHighOn();
        }
        //
        // rpm limiting
        if (useRPMLimit) // if rpm limit is enabled, create audio source for it
        {
            if (maxRPMVolCurve.Evaluate(clipsValue) > optimisationLevel)
            {
                if (maxRPM == null)
                    CreateMaxRPM();
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
        //
        //
    }
    private void OnDisable() // destroy audio sources if Realistic Engine Sound's script is disabled
    {
        if (engineIdle != null)
            Destroy(engineIdle);
        if (lowOn != null)
            Destroy(lowOn);
        if (medOn != null)
            Destroy(medOn);
        if (highOn != null)
            Destroy(highOn);

        if (useRPMLimit)
        {
            if(maxRPM!=null)
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
            else // destroy audio sources with volume 0 or less
            {
                if (engineIdle != null)
                    Destroy(engineIdle);
            }
        }
        //
        // low rpm
        if (lowOnClip != null)
        {
            if (lowVolCurve.Evaluate(clipsValue) > optimisationLevel)
            {
                if (lowOn == null)
                    CreateLowOn();
                else
                {
                    lowOn.volume = lowVolCurve.Evaluate(clipsValue);
                    lowOn.pitch = lowPitchCurve.Evaluate(clipsValue);
                }
            }
            else
            {
                if (lowOn != null)
                    Destroy(lowOn);
            }
        }
        //
        // medium rpm
        if (medOnClip != null)
        {
            if (medVolCurve.Evaluate(clipsValue) > optimisationLevel)
            {
                if (medOn == null)
                    CreateMedOn();
                else
                {
                    medOn.volume = medVolCurve.Evaluate(clipsValue);
                    medOn.pitch = medPitchCurve.Evaluate(clipsValue);
                }
            }
            else
            {
                if (medOn != null)
                    Destroy(medOn);
            }
        }
        //
        // high rpm
        if (highOnClip != null)
        {
            if (highVolCurve.Evaluate(clipsValue) > optimisationLevel)
            {
                if (highOn == null)
                    CreateHighOn();
                else
                {
                    if (!isReversing)
                    {
                        highOn.volume = highVolCurve.Evaluate(clipsValue);
                        highOn.pitch = highPitchCurve.Evaluate(clipsValue);
                    }
                }

            }
            else
            {
                if (highOn != null)
                    Destroy(highOn);
            }
        }
        //
        // rpm limiting
        if (maxRPMClip != null)
        {
            if (useRPMLimit) // if rpm limit is enabled, create audio source for it
            {
                if (maxRPMVolCurve.Evaluate(clipsValue) > optimisationLevel)
                {
                    if (maxRPM == null)
                        CreateMaxRPM();
                    else
                    {
                        maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue);
                        maxRPM.pitch = maxRPMPitchCurve.Evaluate(clipsValue);
                    }
                }
                else
                {
                    if (maxRPM != null)
                        Destroy(maxRPM);
                }
            }
        }
        else
        {
            useRPMLimit = false;
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
                            // set high rpm sound to setted settings
                            if (highOn != null)
                            {
                                highOn.volume = highVolReversingCurve.Evaluate(clipsValue);
                                highOn.pitch = highPitchCurve.Evaluate(clipsValue);
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
            if(reversing!=null)
            {
                Destroy(reversing); // someone disabled reversing sound on runtime, destroy audio source
            }
        }
        // rpm limiting
        if (useRPMLimit) // if rpm limit is enabled, create audio source for it
        {
            if (maxRPM == null)
                CreateMaxRPM();
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
            yield return new WaitForSeconds(0.05f); // this is needed to avoid duplicate audio sources at scene start
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
            engineIdle.loop = true;
            engineIdle.spatialBlend = dopplerAmount;
            engineIdle.clip = idleClip;
            engineIdle.Play();
        }
    }
    // low
    void CreateLowOn()
    {
        if (lowOnClip != null)
        {
            lowOn = gameObject.AddComponent<AudioSource>();
            lowOn.volume = lowVolCurve.Evaluate(clipsValue);
            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue);
            lowOn.minDistance = minDistance;
            lowOn.maxDistance = maxDistance;
            lowOn.loop = true;
            lowOn.spatialBlend = dopplerAmount;
            lowOn.clip = lowOnClip;
            lowOn.Play();
        }
    }
    // medium
    void CreateMedOn()
    {
        if (medOnClip != null)
        {
            medOn = gameObject.AddComponent<AudioSource>();
            medOn.volume = medVolCurve.Evaluate(clipsValue);
            medOn.pitch = medPitchCurve.Evaluate(clipsValue);
            medOn.minDistance = minDistance;
            medOn.maxDistance = maxDistance;
            medOn.loop = true;
            medOn.spatialBlend = dopplerAmount;
            medOn.clip = medOnClip;
            medOn.Play();
        }
    }
    // high
    void CreateHighOn()
    {
        if (highOnClip != null)
        {
            highOn = gameObject.AddComponent<AudioSource>();
            highOn.volume = highVolCurve.Evaluate(clipsValue);
            highOn.pitch = highPitchCurve.Evaluate(clipsValue);
            highOn.minDistance = minDistance;
            highOn.maxDistance = maxDistance;
            highOn.loop = true;
            highOn.spatialBlend = dopplerAmount;
            highOn.clip = highOnClip;
            highOn.Play();
        }
    }
    // rpm limit
    void CreateMaxRPM()
    {
        if (maxRPMClip != null)
        {
            maxRPM = gameObject.AddComponent<AudioSource>();
            maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue);
            maxRPM.pitch = maxRPMPitchCurve.Evaluate(clipsValue);
            maxRPM.minDistance = minDistance;
            maxRPM.maxDistance = maxDistance;
            maxRPM.loop = true;
            maxRPM.spatialBlend = dopplerAmount;
            maxRPM.clip = maxRPMClip;
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
            reversing.loop = true;
            reversing.spatialBlend = dopplerAmount;
            reversing.clip = reversingClip;
            reversing.Play();
        }
    }
}
