﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Showtec
{

    public static class ShowtecLLB8
    {
        #region Enums
        public enum SECTIONS_P4
        {
            RED1,
            GREEN1,
            BLUE1,
            RED2,
            GREEN2,
            BLUE2,
            RED3,
            GREEN3,
            BLUE3,
            RED4,
            GREEN4,
            BLUE4,
            RED5,
            GREEN5,
            BLUE5,
            RED6,
            GREEN6,
            BLUE6,
            RED7,
            GREEN7,
            BLUE7,
            RED8,
            GREEN8,
            BLUE8,
            STROBE,
            MASTER
        }

        public enum RGB
        {
            RED,
            GREEN,
            BLUE
        }
        #endregion

        #region Utility
        public static void Init()
        {
            OpenDMX.start();
            SetAllOff(true);
        }

        public static void SendData()
        {
            OpenDMX.writeData();
        }

        public static int ChannelCount()
        {
            return 26;
        }

        public static int GetChannel(SECTIONS_P4 section)
        {
            int channel = (int)section + 1;

            return channel;
        }

        public static List<int> GetAllColorChannels(RGB rgb)
        {
            List<int> channels = new List<int>();

            switch (rgb)
            {
                case RGB.RED:
                    channels.Add(GetChannel(SECTIONS_P4.RED1));
                    channels.Add(GetChannel(SECTIONS_P4.RED2));
                    channels.Add(GetChannel(SECTIONS_P4.RED3));
                    channels.Add(GetChannel(SECTIONS_P4.RED4));
                    channels.Add(GetChannel(SECTIONS_P4.RED5));
                    channels.Add(GetChannel(SECTIONS_P4.RED6));
                    channels.Add(GetChannel(SECTIONS_P4.RED7));
                    channels.Add(GetChannel(SECTIONS_P4.RED8));
                    break;
                case RGB.GREEN:
                    channels.Add(GetChannel(SECTIONS_P4.GREEN1));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN2));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN3));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN4));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN5));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN6));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN7));
                    channels.Add(GetChannel(SECTIONS_P4.GREEN8));
                    break;
                case RGB.BLUE:
                    channels.Add(GetChannel(SECTIONS_P4.BLUE1));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE2));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE3));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE4));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE5));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE6));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE7));
                    channels.Add(GetChannel(SECTIONS_P4.BLUE8));
                    break;
            }

            return channels;
        }

       
        #endregion

        #region Functionality
        public static void SetMasterFader(byte value, bool writeImmediately)
        {
            OpenDMX.setDmxValue(GetChannel(SECTIONS_P4.MASTER), value);
            if (writeImmediately)
                OpenDMX.writeData();
        }

        public static void SetStroboscope(byte value, bool writeImmediately)
        {
            OpenDMX.setDmxValue(GetChannel(SECTIONS_P4.STROBE), value);
            if (writeImmediately)
                OpenDMX.writeData();
        }

        /// <summary>
        ///  Calculates which ledbar has to be active
        ///  Sets the channels of this bar on to maxValue
        /// </summary>
        /// <param name="v"></param>
        public static void SetKnightRiderEffect(byte value)
        {
            int amountLedBars = 10;
            byte maxValue = 255;
            int channels = 25;
            int correction = 1; 
            int activeLedBar = ((value / maxValue) * 100) / amountLedBars;
            int startingChannel = ((activeLedBar * channels) + correction);
            int endChannel = ((activeLedBar * channels) + correction + channels);
            for (int i = startingChannel; i < endChannel; i++)
            {
                OpenDMX.setDmxValue(i, maxValue);
            }
        }

        public static void SetAllSingleColor(RGB color, byte value, bool writeImmediately)
        {
            List<int> channels = GetAllColorChannels(color);

            for (int i = 1; i < ChannelCount() + 1; i++)
            {
                if (channels.Contains(i))
                    OpenDMX.setDmxValue(i, value);
                else if (i == GetChannel(SECTIONS_P4.STROBE) || i == GetChannel(SECTIONS_P4.MASTER))
                    continue;
                else
                    OpenDMX.setDmxValue(i, 0);
            }
            if (writeImmediately)
                OpenDMX.writeData();
        }

        public static void SetAllOff(bool writeImmediately)
        {
            for (int i = 0; i < 26; i++)
            {
                OpenDMX.setDmxValue(i, 0);
            }
            if (writeImmediately)
                OpenDMX.writeData();
        }
        #endregion
    }

    public class OpenDMX
    {

        public static byte[] buffer = new byte[513];
        public static uint handle;
        public static bool done = false;
        public static bool Connected = false;
        public static int bytesWritten = 0;
        public static FT_STATUS status;

        public const byte BITS_8 = 8;
        public const byte STOP_BITS_2 = 2;
        public const byte PARITY_NONE = 0;
        public const UInt16 FLOW_NONE = 0;
        public const byte PURGE_RX = 1;
        public const byte PURGE_TX = 2;

        private static Thread writeThread;

        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Open(UInt32 uiPort, ref uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Close(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Read(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesReturned);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Write(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesWritten);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetDataCharacteristics(uint ftHandle, byte uWordLength, byte uStopBits, byte uParity);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetFlowControl(uint ftHandle, char usFlowControl, byte uXon, byte uXoff);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_GetModemStatus(uint ftHandle, ref UInt32 lpdwModemStatus);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Purge(uint ftHandle, UInt32 dwMask);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_ClrRts(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetBreakOn(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetBreakOff(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_GetStatus(uint ftHandle, ref UInt32 lpdwAmountInRxQueue, ref UInt32 lpdwAmountInTxQueue, ref UInt32 lpdwEventStatus);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_ResetDevice(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetDivisor(uint ftHandle, char usDivisor);


        public static void start()
        {
            handle = 0;
            status = FT_Open(0, ref handle);
            //setting up the WriteData method to be on it's own thread. This will also turn all channels off
            //this unrequested change of state can be managed by getting the current state of all channels 
            //into the write buffer before calling this function.
            Thread thread = new Thread(new ThreadStart(writeData));
            writeThread = thread;
            thread.Start();
        }

        public static void setDmxValue(int channel, byte value)
        {
            if (buffer != null)
            {
                buffer[channel] = value;
            }
        }

        public static void writeData()
        {
            try
            {
                initOpenDMX();
                if (OpenDMX.status == FT_STATUS.FT_OK)
                {
                    status = FT_SetBreakOn(handle);
                    status = FT_SetBreakOff(handle);
                    bytesWritten = write(handle, buffer, buffer.Length);

                    Thread.Sleep(25);      //give the system time to send the data before sending more 

                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }

        }

        public static int write(uint handle, byte[] data, int length)
        {
            try
            {
                IntPtr ptr = Marshal.AllocHGlobal((int)length);
                Marshal.Copy(data, 0, ptr, (int)length);
                uint bytesWritten = 0;
                status = FT_Write(handle, ptr, (uint)length, ref bytesWritten);
                return (int)bytesWritten;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                return 0;
            }
        }

        public static void initOpenDMX()
        {
            status = FT_ResetDevice(handle);
            status = FT_SetDivisor(handle, (char)12);  // set baud rate
            status = FT_SetDataCharacteristics(handle, BITS_8, STOP_BITS_2, PARITY_NONE);
            status = FT_SetFlowControl(handle, (char)FLOW_NONE, 0, 0);
            status = FT_ClrRts(handle);
            status = FT_Purge(handle, PURGE_TX);
            status = FT_Purge(handle, PURGE_RX);
        }

    }

    /// <summary>
    /// Enumaration containing the varios return status for the DLL functions.
    /// </summary>
    public enum FT_STATUS
    {
        FT_OK = 0,
        FT_INVALID_HANDLE,
        FT_DEVICE_NOT_FOUND,
        FT_DEVICE_NOT_OPENED,
        FT_IO_ERROR,
        FT_INSUFFICIENT_RESOURCES,
        FT_INVALID_PARAMETER,
        FT_INVALID_BAUD_RATE,
        FT_DEVICE_NOT_OPENED_FOR_ERASE,
        FT_DEVICE_NOT_OPENED_FOR_WRITE,
        FT_FAILED_TO_WRITE_DEVICE,
        FT_EEPROM_READ_FAILED,
        FT_EEPROM_WRITE_FAILED,
        FT_EEPROM_ERASE_FAILED,
        FT_EEPROM_NOT_PRESENT,
        FT_EEPROM_NOT_PROGRAMMED,
        FT_INVALID_ARGS,
        FT_OTHER_ERROR
    };
}
