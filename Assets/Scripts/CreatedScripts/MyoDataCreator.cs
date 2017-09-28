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
            DmxControllerScript.knightRiderPercentage = ((float)OrientationScript.ArmHorizontal / 100f);
        }
        else if (Intensity)
        {
            float anglepercentage = ((float)OrientationScript.ArmVertical / 180f);
            float fadervalue = Mathf.Pow(255f, anglepercentage);
            MasterFader = Mathf.Clamp((int)fadervalue, 0, 255);
            DmxControllerScript.masterFaderVal = ((byte)MasterFader);
        }
        else if (Stroboscope)
        {
			float anglepercentage = ((float)OrientationScript.ArmVertical / 180f);
            float fadervalue = 200 + (anglepercentage * 55);
			MasterFader = Mathf.Clamp((int)fadervalue, 0, 255);
			DmxControllerScript.strobeVal = ((byte)MasterFader);
        }
        else if (ColorPalette)
        {
            DmxControllerScript.colorchangePercentage = ((float)OrientationScript.ArmHorizontal / 100f);
        }
        else if (Flash)
        {
            if (MyoScript.accelerometer.x > 1 && !punch)
            {
                StartCoroutine(PunchCooldown());
                punch = true;
                DmxControllerScript.Flash();
            }
        }
    }

    /// <summary>
    /// /////////////////////
    /// </summary>


    public void activateIntensity()
    {
        ResetBools();
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.MASTER_CONTROL);
        Intensity = true;

    }
    public void activateStroboscope()
    {
        ResetBools();
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.STROBE_SPEED);
        Stroboscope = true;

    }
    public void activateColorPalette()
    {
        ResetBools();
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.COLOR_CHANGE);
        ColorPalette = true;

    }
    public void activateFlash()
    {
        ResetBools();
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.FLASH);
        Flash = true;
    }
    public void activateKnightrider()
    {
        ResetBools();
        DmxControllerScript.SetActiveEffect(DmxController.LedEffects.KNIGHT_RIDER);
        Knightrider = true;
    }

    IEnumerator PunchCooldown()
    {
        yield return new WaitForSeconds(0.4f);
        punch = false;
        StopCoroutine(PunchCooldown());
    }

    public void ResetBools()
    {
        Knightrider = false;
        Intensity = false;
        Stroboscope = false;
        ColorPalette = false;
        Flash = false;
    }

}
