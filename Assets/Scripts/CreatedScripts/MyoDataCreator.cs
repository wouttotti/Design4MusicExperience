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
            StartIntensity();
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

    private void StartIntensity()
    {
        DmxControllerScript.light
    }

    void StartKnightRider(float percentage, bool status)
    {
        DmxControllerScript.knightRiderActive = status;
        DmxControllerScript.knightRiderPercentage = percentage;
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

    }
    public void activateKnightrider()
    {
        Knightrider = true;
    }

}
