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
    public bool strobeActive = false;
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

    /// <summary>
    /// Toggles the stroboscope
    /// </summary>
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

    /// <summary>
    /// Set the master fader value for a given led bar
    /// </summary>
    /// <param name="value">0 - 255</param>
    /// <param name="ledbarIndex">The index of the led bar (starting at 0)</param>
    public void SetMasterFader(byte value, int ledbarIndex = 0)
    {
        ResetCounter();
        ShowtecLLB8.SetMasterFader(value, true, ledbarIndex);
    }

    /// <summary>
    /// Sets one of the led bars active with the knightRiderColor, while turning all others off. 
    /// </summary>
    /// <param name="percentage">Determines which of the led bars gets activated</param>
    public void SetKnightRider(float percentage)
    {
        SetAllOff();

        int activeBar = Mathf.RoundToInt(percentage * (float)maxLedbarIndex);
        SetAllSingleColor(knightRiderColor, false, activeBar);
        SetMasterFader(255, activeBar);
    }

    /// <summary>
    /// Enables the stroboscope for a given led bar.
    /// Strobe intensity is determined by strobeVal.
    /// </summary>
    /// <param name="active"></param>
    /// <param name="ledbarIndex"></param>
    public void SetStroboscope(bool active, int ledbarIndex = 0)
    {
        dmxSignalIntervalSeconds = active ? strobeIntervalSeconds : regularIntervalSeconds;
        ResetCounter();

        byte value = active ? (byte)strobeVal : (byte)0;
        ShowtecLLB8.SetStroboscope(value, true, ledbarIndex);
    }

    /// <summary>
    /// Sets an entire led bar to a single color.
    /// Does NOT activate the master fader.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="writeImmediately">Send DMX data to device</param>
    /// <param name="ledbarIndex"></param>
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
