using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Showtec;

public class DmxController : MonoBehaviour
{
    public enum LedColors
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        CYAN,
        PURPLE
    }

    public int ledbarCount = 2;
    public bool active = false;
    [Range(0, 255)]
    public int strobeVal = 220;

    public bool masterFaderControlActive = false;
    [Range(0, 255)]
    public int masterFaderVal = 255;

    public bool knightRiderActive = false;
    [Range(0f, 1f)]
    public float knightRiderPercentage = 0.5f;
    public LedColors knightRiderColor = LedColors.PURPLE;

    private float dmxSignalIntervalSeconds = 0;
    private float count;
    private float regularIntervalSeconds = 0.05f;
    private bool strobeActive = false;
    private float strobeIntervalSeconds = 0.05f;
    private float maxLedbarIndex = 0;

    private void Awake()
    {
        maxLedbarIndex = ledbarCount - 1;
        ShowtecLLB8.Init();
        dmxSignalIntervalSeconds = regularIntervalSeconds;
        SetAllSingleColor(LedColors.RED, false, 0);
        SetAllSingleColor(LedColors.PURPLE, false, 1);
        SetMasterFader(255, 0);
        SetMasterFader(255, 1);
    }

    private void Update()
    {
        count += Time.deltaTime;
        if (count >= dmxSignalIntervalSeconds && active)
        {
            ResetCounter();

            if (masterFaderControlActive)
                SetMasterFader((byte)masterFaderVal);
            else
            {
                if (knightRiderActive)
                    SetKnightRider(knightRiderPercentage);

                ShowtecLLB8.SendData();
            }
        }
    }

    private void ResetCounter()
    {
        count -= dmxSignalIntervalSeconds;
        if (count < 0)
            count = 0;
    }

    public void ToggleStrobe()
    {
        strobeActive = !strobeActive;
        SetStroboscope(strobeActive);
    }

    #region LedBar functions
    /// <summary>
    /// Shuts off all led bars
    /// </summary>
    public void SetAllOff()
    {
        for (int i = 0; i < ledbarCount; i++)
        {
            ShowtecLLB8.SetAllOff(false, i);
        }
        ResetCounter();
    }

    public void SetMasterFader(byte value, int ledbarIndex = 0)
    {
        ResetCounter();
        ShowtecLLB8.SetMasterFader(value, true, ledbarIndex);
    }

    public void SetKnightRider(float percentage)
    {
        SetAllOff();

        int activeBar = Mathf.RoundToInt(percentage * (float)maxLedbarIndex);
        Debug.Log(activeBar);
        SetAllSingleColor(knightRiderColor, false, activeBar);
        SetMasterFader(255, activeBar);

    }

    public void SetStroboscope(bool active, int ledbarIndex = 0)
    {
        dmxSignalIntervalSeconds = active ? strobeIntervalSeconds : regularIntervalSeconds;
        ResetCounter();

        byte value = active ? (byte)strobeVal : (byte)0;
        ShowtecLLB8.SetStroboscope(value, true, ledbarIndex);
    }

    public void SetAllSingleColor(LedColors color, bool writeImmediately, int ledbarIndex = 0)
    {
        switch (color)
        {
            case LedColors.BLUE:
                SetAllBlue(writeImmediately, ledbarIndex);
                break;
            case LedColors.GREEN:
                SetAllGreen(writeImmediately, ledbarIndex);
                break;
            case LedColors.RED:
                SetAllRed(writeImmediately, ledbarIndex);
                break;
            case LedColors.YELLOW:
                SetAllYellow(writeImmediately, ledbarIndex);
                break;
            case LedColors.CYAN:
                SetAllCyan(writeImmediately, ledbarIndex);
                break;
            case LedColors.PURPLE:
                SetAllPurple(writeImmediately, ledbarIndex);
                break;
        }
    }

    #region Individual colors
    private void SetAllBlue(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex);
    }

    private void SetAllRed(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex);
    }

    private void SetAllGreen(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex);
    }

    private void SetAllPurple(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
    }

    private void SetAllYellow(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
    }

    private void SetAllCyan(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
    }
    #endregion
    #endregion

}
