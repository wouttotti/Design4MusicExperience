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

    public bool active = false;
    [Range(0, 255)]
    public int strobeVal = 220;
    public bool masterFaderControlActive = false;
    [Range(0, 255)]
    public int masterFaderVal = 255;

    private float dmxSignalIntervalSeconds = 0;
    private float count;
    private float regularIntervalSeconds = 0.05f;
    private bool strobeActive = false;
    private float strobeIntervalSeconds = 0.05f;

    private void Awake()
    {

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
                ShowtecLLB8.SendData();
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
    public void AllOff()
    {
        ShowtecLLB8.SetAllOff(true);
        ResetCounter();
    }

    public void SetMasterFader(byte value, int ledbarIndex = 0)
    {
        ResetCounter();
        ShowtecLLB8.SetMasterFader(value, true, ledbarIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetKnightRiderEffect(byte value)
    {
        AllOff();
        int amountLedBars = 10;
        byte maxValue = 255;
        int channels = 25;
        int correction = 1;
        int stroboChannel = 24;
        int masterChannel = 25;
        int activeLedBar = ((value / maxValue) * 100) / amountLedBars;
        int startingChannel = ((activeLedBar * channels) + correction);
        int endChannel = ((activeLedBar * channels) + correction + channels);
        for (int i = startingChannel; i <= endChannel; i++)
        {
            if (i == stroboChannel || i == masterChannel)
                continue;
            else
                SetAllSingleColor(LedColors.BLUE, true, i);
        }
        
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
        ResetCounter();
    }

    private void SetAllRed(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex);
        ResetCounter();
    }

    private void SetAllGreen(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex);
        ResetCounter();
    }

    private void SetAllPurple(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
        ResetCounter();
    }

    private void SetAllYellow(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
        ResetCounter();
    }

    private void SetAllCyan(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
        ResetCounter();
    }
    #endregion
    #endregion

}
