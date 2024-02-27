using Godot;
//using Godot.Collections;
using System.IO.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using Godot.Collections;
using System.Threading.Tasks;

public class MaiLightSerial
{
    private SerialPort lightSerial;
    private Thread thread;
    private List<byte[]> ringLightDataList = new List<byte[]>();
    private bool isUpdateCMD = false;
    private enum CMD : byte
    {
        StartByte = 224,
        BootMode = 253,
        Dc = 63,
        DcUpdate = 59,
        DisableResponse = 126,
        EEPRom = 123,
        EnableResponse = 125,
        LedDirect = 130,
        LedFet = 57,
        LedGs8Bit = 49,
        LedGs8BitMulti = 50,
        LedGs8BitMultiFade = 51,
        LedGs8BitUpdate = 60,
        Timeout = 17,
    }
    
    
    public MaiLightSerial(string port, int baudRate)
    {
        lightSerial = new SerialPort(port, baudRate);
        try
        {
            GD.Print("Try start LED Serial");
            lightSerial.Open();
        }
        catch (Exception ex)
        {
            GD.Print($"Failed to Open Serial Ports: {ex}");
        }
        GD.Print($"LED Serial on {port} Started");
        thread = new Thread(new ParameterizedThreadStart(getDataListFrom));
        thread.Start(lightSerial);
    }

    private void getDataListFrom(object Serial)
    {
        SerialPort _serial = (SerialPort)Serial;
        byte[] header = new byte[3];

        while (true)
        {
            if (!_serial.IsOpen && _serial.BytesToRead < 1)
                continue;
                
            if ((byte)_serial.ReadByte() == (byte)CMD.StartByte)
            {
                _serial.Read(header, 0, 3);

                List<byte> dataBytes = new List<byte>();
                for (int i = 0; i < header[2]; i++) 
                    dataBytes.Add((byte)_serial.ReadByte());

                requestCMD(dataBytes.ToArray());
            }
        }
    }
    public Action<Color> OnCabLightUpdate;
    public Action<int, Color> OnRingLightUpdate;
    private void requestCMD(byte[] data)
    {
        GD.Print("CMD: " + data[0]);
        switch (data[0])
        {
            case (byte)CMD.LedFet:
                OnCabLightUpdate?.Invoke(Color.Color8(data[1], data[2], data[3]));
                break;
            case (byte)CMD.LedGs8Bit:
            case (byte)CMD.LedGs8BitMulti:
            case (byte)CMD.LedGs8BitMultiFade:
                ringLightDataList.Add(data);
                break;
            case (byte)CMD.LedGs8BitUpdate:
                GD.Print($"Ring Light Update: {ringLightDataList.Count}");
                onRingLightDataUpdate();
                break;
            default:
                break;
        }
    }

    private void onRingLightDataUpdate()
    {
        byte[] prevdata = new byte[0];
        foreach (var data in ringLightDataList)
        {
            switch (data[0])
            {
                case (byte)CMD.LedGs8Bit:
                    OnRingLightUpdate?.Invoke(data[1], Color.Color8(data[2], data[3], data[4]));
                    break;
                case (byte)CMD.LedGs8BitMulti:
                    for (int i = data[1]; i < data[2]; i++)
                        OnRingLightUpdate?.Invoke(data[i], Color.Color8(data[4], data[5], data[6]));
                    prevdata = data;
                    break;
                case (byte)CMD.LedGs8BitMultiFade:
                    updateRingLightFade(data, prevdata);
                    prevdata = data;                     
                    break;
                default:
                    break;
            }
        }
        ringLightDataList.Clear();
    }

    async private void updateRingLightFade(byte[] data, byte[] prevdata)
    {
        float elapsed = 0;
        float duration = 4095 / data[7] * 8 / 1000;
        Color prevColor = Color.Color8(prevdata[4], prevdata[5], prevdata[6]);
        Color nowColor = Color.Color8(data[4], data[5], data[6]);
        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            for (int i = data[1]; i < data[2]; i++)
                OnRingLightUpdate?.Invoke(data[i], prevColor.Lerp(nowColor, progress));
            await Task.Delay(100);
            elapsed += 0.1f;
        }
    }
}
