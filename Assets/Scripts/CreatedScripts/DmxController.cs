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
        PURPLE,
        WHITE
    }

    public enum LedEffects
    {
        NONE,
        MASTER_CONTROL,
        KNIGHT_RIDER,
        COLOR_CHANGE,
        STROBE_SPEED,
        FLASH
    }

    public int ledbarCount = 2;
    public bool active = false;
    [Range(0, 255)]
    public byte strobeVal = 220;

    private bool masterFaderControlActive = false;
    private bool knightRiderActive = false;
    private bool strobeActive = false;
    private bool colorChangeActive = false;
    private bool flashActive = false;

    private float flashDuration = .8f;
    private float currentFlashPercentage = 0f;

    [Range(0, 255)]
    public int masterFaderVal = 255;

    [Range(0f, 1f)]
    public float knightRiderPercentage = 0.5f;
    public LedColors knightRiderColor = LedColors.PURPLE;

    private float dmxSignalIntervalSeconds = 0.025f;
    private float count;
    private float regularIntervalSeconds = 0.05f;
    private float strobeIntervalSeconds = 0.05f;
    private float maxLedbarIndex = 0;


    private void Awake()
    {
        maxLedbarIndex = ledbarCount - 1;
        ShowtecLLB8.Init();

        /*
        SetAllSingleColor(LedColors.RED, false, 0);
        SetAllSingleColor(LedColors.PURPLE, false, 1);
        SetMasterFader(255, 0);
        SetMasterFader(255, 1);
        */

    }

    private void Start()
    {
        SetActiveEffect(LedEffects.FLASH);
        Flash();
    }

    private void Update()
    {
        count += Time.deltaTime;
        if (count >= dmxSignalIntervalSeconds && active)
        {
            ResetCounter();

            if (masterFaderControlActive)
                SetMasterFader((byte)masterFaderVal);
            else if (knightRiderActive)
                SetKnightRider(knightRiderPercentage);
            else if (strobeActive)
                SetStroboscopeAll(strobeVal);
            else if (colorChangeActive) { }
            else if (flashActive) { }

            ShowtecLLB8.SendData();
        }
    }

    /// <summary>
    /// Enables the desired effect. Once enabled, the effect can be controlled by changing their respective values.
    /// </summary>
    /// <param name="effect"></param>
    public void SetActiveEffect(LedEffects effect)
    {
        DisableAllEffects();

        switch (effect)
        {
            case LedEffects.COLOR_CHANGE:
                colorChangeActive = true;
                break;
            case LedEffects.FLASH:
                flashActive = true;
                break;
            case LedEffects.KNIGHT_RIDER:
                knightRiderActive = true;
                break;
            case LedEffects.MASTER_CONTROL:
                masterFaderControlActive = true;
                break;
            case LedEffects.STROBE_SPEED:
                strobeActive = true;
                break;
            case LedEffects.NONE:
                DisableAllEffects();
                break;
        }
    }

    private void DisableAllEffects()
    {
        colorChangeActive = false;
        flashActive = false;
        strobeActive = false;
        knightRiderActive = false;
        masterFaderControlActive = false;
    }

    private void ResetCounter()
    {
        count -= dmxSignalIntervalSeconds;
        if (count < 0)
            count = 0;
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
    }

    /// <summary>
    /// Set the master fader value for a given led bar
    /// </summary>
    /// <param name="value">0 - 255</param>
    /// <param name="ledbarIndex">The index of the led bar (starting at 0)</param>
    public void SetMasterFader(byte value, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetMasterFader(value, false, ledbarIndex);
    }

    public void SetMasterFaderAll(byte value)
    {
        for (int i = 0; i < ledbarCount; i++)
        {
            ShowtecLLB8.SetMasterFader(value, false, i);
        }
    }

    public void Flash()
    {
        if (!flashActive)
            return;
        StopCoroutine("FlashFade");
        currentFlashPercentage = 1f;
        SetSingleColorAll(LedColors.CYAN, false);
        SetMasterFaderAll((byte)(currentFlashPercentage * 255));
        StartCoroutine("FlashFade");
    }

    private IEnumerator FlashFade()
    {
        while (currentFlashPercentage > 0f)
        {
            currentFlashPercentage -= (Time.deltaTime * (1 / flashDuration));
            if (currentFlashPercentage < 0f)
                currentFlashPercentage = 0;
            SetMasterFaderAll((byte)(currentFlashPercentage * 255f));
            yield return null;
        }
    }

    /// <summary>
    /// Sets one of the led bars active with the knightRiderColor, while turning all others off. 
    /// </summary>
    /// <param name="percentage">Determines which of the led bars gets activated</param>
    public void SetKnightRider(float percentage)
    {
        SetAllOff();

        int activeBar = Mathf.RoundToInt(percentage * (float)maxLedbarIndex);
        SetSingleColor(knightRiderColor, false, activeBar);
        SetMasterFader(255, activeBar);
    }

    /// <summary>
    /// Enables the stroboscope for a given led bar.
    /// </summary>
    /// <param name="active"></param>
    /// <param name="ledbarIndex"></param>
    public void SetStroboscope(byte value, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetStroboscope(value, false, ledbarIndex);
    }

    /// <summary>
    /// Enables stroboscope for all led bars
    /// </summary>
    /// <param name="value"></param>
    public void SetStroboscopeAll(byte value)
    {
        for (int i = 0; i < ledbarCount; i++)
        {
            ShowtecLLB8.SetStroboscope(value, false, i);
        }
    }

    public void SetSingleColorAll(LedColors color, bool writeImmediately)
    {
        for (int i = 0; i < ledbarCount; i++)
        {
            SetSingleColor(color, writeImmediately, i);
        }
    }

    /// <summary>
    /// Sets an entire led bar to a single color.
    /// Does NOT activate the master fader.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="writeImmediately">Send DMX data to device</param>
    /// <param name="ledbarIndex"></param>
    public void SetSingleColor(LedColors color, bool writeImmediately, int ledbarIndex = 0)
    {
        switch (color)
        {
            case LedColors.BLUE:
                SetBlue(writeImmediately, ledbarIndex);
                break;
            case LedColors.GREEN:
                SetGreen(writeImmediately, ledbarIndex);
                break;
            case LedColors.RED:
                SetRed(writeImmediately, ledbarIndex);
                break;
            case LedColors.YELLOW:
                SetYellow(writeImmediately, ledbarIndex);
                break;
            case LedColors.CYAN:
                SetCyan(writeImmediately, ledbarIndex);
                break;
            case LedColors.PURPLE:
                SetPurple(writeImmediately, ledbarIndex);
                break;
        }
    }

    #region Individual colors
    private void SetBlue(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex);
    }

    private void SetRed(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex);
    }

    private void SetGreen(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex);
    }

    private void SetPurple(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
    }

    private void SetYellow(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
    }

    private void SetCyan(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
    }

    private void SetWhite(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex, false);
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex, false);

    }
    #endregion
    #endregion

}
