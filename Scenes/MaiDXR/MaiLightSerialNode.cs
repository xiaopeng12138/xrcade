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
using System.Linq;

public partial class MaiLightSerialNode : Node
{
    [Export]
    public string Port = "COM21";
    [Export]
    public int BaudRate = 115200;
    [Export]
	public Node3D RingLightsRoot;
	[Export]
	public Light3D BodyLight, DisplayLight, SideLight;
    [Export]
    public int FadeResolution = 16;

    private List<Light3D> RingLights;
    private SerialPort lightSerial;
    private Thread thread;
    private List<byte[]> ringLightDataList = new List<byte[]>();
    private bool isrequestCMD = false;
    private byte[] header = new byte[3];
    private List<byte> dataBytes = new List<byte>();
    private List<byte[]> dataBytesList = new List<byte[]>();
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
    
    public override void _Ready()
    {
        RingLights = RingLightsRoot.GetChildren().OfType<Light3D>().ToList();
        // GD.Print("Ring Lights: " + RingLights.Count);
        initialSerial(Port, BaudRate);
    }
    public override void _Process(double delta)
    {
        if (dataBytesList.Count > 0)
        {
            requestCMD(dataBytesList[0]);
            dataBytesList.RemoveAt(0);
        }
    }

    public override void _ExitTree()
    {
        lightSerial.Close();
        thread.Interrupt();
    }
    private void initialSerial(string port, int baudRate)
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

                dataBytes = new List<byte>();
                for (int i = 0; i <= header[2]; i++) 
                    dataBytes.Add((byte)_serial.ReadByte());
                // GD.Print("Raw Data: " + BitConverter.ToString(dataBytes.ToArray()));
                dataBytesList.Add(dataBytes.ToArray());
            }
        }
    }

    private void requestCMD(byte[] data)
    {
        // GD.Print("CMD: " + data[0]);
        if (data.Length < 1)
            return;
        switch (data[0])
        {
            case (byte)CMD.LedFet:
                OnCabLightUpdate(Color.Color8(data[1], data[2], data[3]));
                break;
            case (byte)CMD.LedGs8Bit:
            case (byte)CMD.LedGs8BitMulti:
            case (byte)CMD.LedGs8BitMultiFade:
                ringLightDataList.Add(data);
                break;
            case (byte)CMD.LedGs8BitUpdate:
                onRingLightUpdateCMD();
                break;
            default:
                break;
        }
    }

    private void onRingLightUpdateCMD()
    {
        byte[] prevdata = new byte[16]; // 16 is a probable max length of data
        if (ringLightDataList.Count < 1)
            return;
        // GD.Print("Ring Light Update: " + ringLightDataList.Count);
        
        foreach (var data in ringLightDataList)
        {
            // GD.Print("Ring Light Update: " + BitConverter.ToString(data));
            switch (data[0])
            {
                case (byte)CMD.LedGs8Bit:
                    // GD.Print("Ring Light Single");
                    // GD.Print(data[1]);
                    OnRingLightUpdate(data[1], Color.Color8(data[2], data[3], data[4]));
                    break;
                case (byte)CMD.LedGs8BitMulti:
                    // GD.Print("Ring Light Multi");
                    // GD.Print(data[1] + " " + data[2]);
                    for (int i = (int)data[1]; i < (int)data[2]; i++)
                        OnRingLightUpdate(i, Color.Color8(data[4], data[5], data[6]));
                    prevdata = data;
                    break;
                case (byte)CMD.LedGs8BitMultiFade:
                    // GD.Print("Ring Light Fade");
                    // GD.Print(BitConverter.ToString(data) + " " + BitConverter.ToString(prevdata));
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
                OnRingLightUpdate(i, prevColor.Lerp(nowColor, progress));
            await Task.Delay(FadeResolution);
            elapsed += FadeResolution / 1000f;
        }
    }

    private void OnCabLightUpdate(Color data) { 
        CabLightUpdate(data);
    }
    private void OnRingLightUpdate(int index, Color color) { 
        // GD.Print("Ring Light Update");
        // GD.Print(index + " " + color);
        RingLightUpdate(index, color);
    }

    private void CabLightUpdate(Color data)
	{
		if (BodyLight != null)
			BodyLight.LightColor = new Color(data.R, data.R, data.R);
		if (DisplayLight != null)
			DisplayLight.LightColor = new Color(data.G, data.G, data.G);
		if (SideLight != null)
			SideLight.LightColor = new Color(data.B, data.B, data.B);
	}
    private void RingLightUpdate(int index, Color color)
	{
		if (RingLights != null && RingLights.Count > index)
			RingLights[index].LightColor = color;
	}
}
