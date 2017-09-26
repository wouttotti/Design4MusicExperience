using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class MyoDataCreator : MonoBehaviour
{

    public GameObject myo = null;

    public JointOrientation OrientationScript = null;

    public DmxController DmxControllerScript = null;

    public bool Knightrider = false;
    public bool Intensity = false;
    public bool Stroboscope = false;
    public bool ColorPalette = false;
    public bool Flash = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        print(OrientationScript.ArmHorizontal);

        if (Knightrider)
        {
            StartKnightRider((OrientationScript.ArmHorizontal / 100f), Knightrider);
        }
        else if (Intensity)
        {
            StartIntensity((byte)(OrientationScript.ArmVertical), Intensity);
        }
        else if (Stroboscope)
        {
            StartStroboscope();
        }
        else if (ColorPalette)
        {
            StartColorPalette();
        }
        else if (Flash)
        {
            StartFlash();
        }

    }

    private void ResetValues()
    {
        Knightrider = false;
        Intensity = false;
        Stroboscope = false;
        ColorPalette = false;
        Flash = false;
        if (DmxControllerScript.knightRiderActive)
        {
            DmxControllerScript.knightRiderActive = false;
        }
        if (DmxControllerScript.masterFaderControlActive)
        {
            DmxControllerScript.masterFaderControlActive = false;
        }
        if (DmxControllerScript.strobeActive)
        {
            DmxControllerScript.strobeActive = false;
        }
    }

    private void StartFlash()
    {
        throw new NotImplementedException();
    }

    private void StartColorPalette()
    {
        throw new NotImplementedException();
    }

    private void StartStroboscope()
    {
        DmxControllerScript.ToggleStrobe();
    }

    private void StartIntensity(byte value, bool status)
    {
        DmxControllerScript.masterFaderControlActive = status;
        DmxControllerScript.masterFaderVal = value;
    }

    void StartKnightRider(float percentage, bool status)
    {
        DmxControllerScript.knightRiderActive = status;
        DmxControllerScript.knightRiderPercentage = percentage;
    }

    public void activateIntensity()
    {
        ResetValues();
        Intensity = true;

    }
    public void activateStroboscope()
    {
        ResetValues();
        Stroboscope = true;

    }
    public void activateColorPalette()
    {
        ResetValues();
        ColorPalette = true;

    }
    public void activateFlash()
    {
        ResetValues();
        Flash = true;

    }
    public void activateKnightrider()
    {
        ResetValues();
        Knightrider = true;
    }

}
