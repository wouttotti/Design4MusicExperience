using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using XDirection = Thalmic.Myo.XDirection;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;
using Arm = Thalmic.Myo.Arm;


public class MyoDataCreator : MonoBehaviour
{

    public GameObject myo = null;

    public JointOrientation OrientationScript = null;

    public ThalmicMyo MyoScript = null;

    public DmxController DmxControllerScript = null;

    public bool Knightrider = false;
    public bool Intensity = false;
    public bool Stroboscope = false;
    public bool ColorPalette = false;
    public bool Flash = false;

    private bool punch;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Knightrider)
        {
            StartKnightRider((OrientationScript.ArmHorizontal / 100f), Knightrider);
        }
        else if (Intensity)
        {
        }
        else if (Stroboscope)
        {
        }
        else if (ColorPalette)
        {
        }
        else if (Flash)
        {
            if (MyoScript.accelerometer.x > 1 && !punch)
            {
                StartCoroutine(PunchCooldown());
                punch = true;
                StartFlash();
            }
        }

    }

    private void StartIntensity()
    {
    }

    void StartKnightRider(float percentage, bool status)
    {
        DmxControllerScript.knightRiderPercentage = percentage;
    }
    void StartFlash()
    {
        DmxControllerScript.Flash();
    }

    public void activateIntensity()
    {

    }
    public void activateStroboscope()
    {

    }
    public void activateColorPalette()
    {

    }
    public void activateFlash()
    {
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.FLASH);
        Flash = true;
    }
    public void activateKnightrider()
    {
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.KNIGHT_RIDER);
        Knightrider = true;
    }

    IEnumerator PunchCooldown()
    {
        yield return new WaitForSeconds(0.4f);
        punch = false;
        StopCoroutine(PunchCooldown());
    }

}
