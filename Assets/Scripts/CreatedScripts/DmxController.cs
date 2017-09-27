using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Showtec;

public class DmxController : MonoBehaviour
{
    public enum LedColors
    {
        RED,
        YELLOW,
        GREEN,
        CYAN,
        BLUE,
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

    private bool masterFaderControlActive = false;
    private bool knightRiderActive = false;
    private bool strobeActive = false;
    private bool colorChangeActive = false;
    private bool flashActive = false;

    public float flashDuration = .8f;
    private float currentFlashPercentage = 0f;

    [Range(0, 255)]
    public byte strobeVal = 220;

    [Range(0, 255)]
    public int masterFaderVal = 255;

    [Range(0f, 1f)]
    public float knightRiderPercentage = 0.5f;
    public LedColors knightRiderColor = LedColors.PURPLE;

    private float dmxSignalIntervalSeconds = 0.025f;
    private float count;
    private float maxLedbarIndex = 0;

    [Range(0f, 1f)]
    public float colorchangePercentage = 0f;
    private LedColors nextColorChangeColor;
    private LedColors currentColorChangeColor;

    private void Awake()
    {
        maxLedbarIndex = ledbarCount - 1;
        ShowtecLLB8.Init();
    }

    private void Start()
    {
        SetActiveEffect(LedEffects.COLOR_CHANGE);
        SetMasterFaderAll(255);
    }

    private void Update()
    {
        count += Time.deltaTime;
        if (count >= dmxSignalIntervalSeconds)
        {
            ResetCounter();

            if (masterFaderControlActive)
                SetMasterFader((byte)masterFaderVal);
            else if (knightRiderActive)
                SetKnightRider(knightRiderPercentage);
            else if (strobeActive)
                SetStroboscopeAll(strobeVal);
            else if (colorChangeActive)
                SetColorChange(colorchangePercentage);
            else if (flashActive) { }

            // Always send data every dmxSignalIntervalSeconds for maximum smoothness
            ShowtecLLB8.SendData();
        }
    }

    private void SetColorChange(float percentage)
    {
        if (percentage < 0.33f)
        {
            Mathf.Clamp(percentage, 0f, 0.33f);

            byte red = (byte)(255f * (1f - (percentage / 0.33f)));
            byte green = (byte)(255f * ((percentage / 0.33f)));

            for (int i = 0; i < ledbarCount; i++)
            {
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, red, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, green, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 0, false, i, false);
            }
        }
        else if (percentage < 0.66f)
        {
            percentage -= 0.33f;
            Mathf.Clamp(percentage, 0f, 0.33f);

            byte green = (byte)(255f * (1f - (percentage / 0.33f)));
            byte blue = (byte)(255f * ((percentage / 0.33f))); ;

            for (int i = 0; i < ledbarCount; i++)
            {
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, green, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, blue, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 0, false, i, false);
            }
        }
        else if (percentage < 0.99f)
        {
            percentage -= 0.66f;
            Mathf.Clamp(percentage, 0f, 0.33f);

            byte blue = (byte)(255f * (1f - (percentage / 0.33f)));
            byte red = (byte)(255f * ((percentage / 0.33f)));

            for (int i = 0; i < ledbarCount; i++)
            {
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, blue, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, red, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 0, false, i, false);

            }
        }
        else
        {
            // This only happens when percantage == 1. Safety measure.
            for (int i = 0; i < ledbarCount; i++)
            {
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 0, false, i, false);
                ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 0, false, i, false);
            }
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

    /// <summary>
    /// Set all 'active' booleans to false
    /// </summary>
    private void DisableAllEffects()
    {
        colorChangeActive = false;
        flashActive = false;
        strobeActive = false;
        knightRiderActive = false;
        masterFaderControlActive = false;
    }

    /// <summary>
    /// Reduce count by dmxSignalIntervalSeconds. Clamp at 0.
    /// </summary>
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

    /// <summary>
    /// Set all led bars to a single color and fade out over time.
    /// </summary>
    public void Flash()
    {
        if (!flashActive)
            return;
        StopCoroutine("FlashFade");
        currentFlashPercentage = 1f;
        SetSingleColorAll(LedColors.WHITE, false);
        SetMasterFaderAll((byte)(currentFlashPercentage * 255));
        StartCoroutine("FlashFade");
    }

    /// <summary>
    /// Fade all led bars out over time
    /// </summary>
    /// <returns></returns>
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
            case LedColors.WHITE:
                SetWhite(writeImmediately, ledbarIndex);
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
