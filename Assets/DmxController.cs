using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Showtec;

public class DmxController : MonoBehaviour
{
    public bool active = false;
    [Range(0, 255)]
    public int strobeVal = 210;
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
        AllBlue(false, 0);
        AllBlue(true, 1);


        string debugString = "";
        List<int> channels = ShowtecLLB8.GetAllColorChannels(ShowtecLLB8.RGB.RED, 1);
        foreach (int c in channels)
        {
            debugString += c + ", ";
        }
        Debug.Log(debugString);



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

    public void SetStroboscope(bool active, int ledbarIndex = 0)
    {
        dmxSignalIntervalSeconds = active ? strobeIntervalSeconds : regularIntervalSeconds;
        ResetCounter();

        byte value = active ? (byte)strobeVal : (byte)0;
        ShowtecLLB8.SetStroboscope(value, true, ledbarIndex);
    }

    public void AllBlue(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.BLUE, 255, false, ledbarIndex);
        ShowtecLLB8.SetMasterFader(255, writeImmediately, ledbarIndex);
        ResetCounter();
    }

    public void AllRed(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.RED, 255, false, ledbarIndex);
        ShowtecLLB8.SetMasterFader(255, writeImmediately, ledbarIndex);
        ResetCounter();
    }

    public void AllGreen(bool writeImmediately, int ledbarIndex = 0)
    {
        ShowtecLLB8.SetAllSingleColor(ShowtecLLB8.RGB.GREEN, 255, false, ledbarIndex);
        ShowtecLLB8.SetMasterFader(255, writeImmediately, ledbarIndex);
        ResetCounter();
    }

    #endregion

}
