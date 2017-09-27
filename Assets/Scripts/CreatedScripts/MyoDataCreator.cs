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

    public ThalmicMyo MyoScript = null;

    public DmxController DmxControllerScript = null;

    public bool Knightrider = false;
    public bool Intensity = false;
    public bool Stroboscope = false;
    public bool ColorPalette = false;
    public bool Flash = false;

    private bool punch;

    private float MasterFader;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Knightrider)
        {
            StartKnightRider(OrientationScript.ArmHorizontal / 100f);
        }
        else if (Intensity)
        {

            float anglepercentage = ((float)OrientationScript.ArmVertical / 180f);
            
            float fadervalue = Mathf.Pow(255f, anglepercentage);

            MasterFader = Mathf.Clamp((int)fadervalue, 0, 255);
            
            StartIntensity((byte)MasterFader);
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
            if (MyoScript.accelerometer.x > 1 && !punch)
            {
                StartCoroutine(PunchCooldown());
                punch = true;
                StartFlash();
            }
        }

    }

    private void StartColorPalette()
    {
        throw new NotImplementedException();
    }

    private void StartStroboscope()
    {
    }

    private void StartIntensity(byte value)
    {
        DmxControllerScript.masterFaderVal = value;
    }

    void StartKnightRider(float percentage)
    {
        DmxControllerScript.knightRiderPercentage = percentage;
    }
    void StartFlash()
    {
        DmxControllerScript.Flash();
    }

    public void activateIntensity()
    {
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.MASTER_CONTROL);
        Intensity = true;

    }
    public void activateStroboscope()
    {
        Stroboscope = true;

    }
    public void activateColorPalette()
    {
        ColorPalette = true;

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
