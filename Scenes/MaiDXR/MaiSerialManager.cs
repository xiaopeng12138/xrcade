using Godot;
using System.IO.Ports;
using System;
using System.Collections;
using System.Threading;
public partial class MaiSerialManager : Node
{
    static SerialPort p1Serial = new SerialPort ("COM5", 9600);
    static SerialPort p2Serial = new SerialPort ("COM6", 9600);
    static byte[] settingPacket = new byte[6] {40, 0, 0, 0, 0, 41};
    static byte[] touchData = new byte[9] {40, 0, 0, 0, 0, 0, 0, 0, 41};
    static byte[] touchData2 = new byte[9] {40, 0, 0, 0, 0, 0, 0, 0, 41};
    public static bool startUp = false; //use ture for default start up state to prevent restart game
    static string recivData;
    private Thread touchThread;
    private Queue touchQueue;
    
    public override void _Ready()
    {
        try
        {
            GD.Print("Try start Serial");
            p1Serial.Open();
            p2Serial.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to Open Serial Ports: {ex}");
        }
        touchQueue = Queue.Synchronized(new Queue());
        touchThread = new Thread(TouchThread);
        MaiTouchArea.touchDidChange += PingTouchThread;
        touchThread.Start();
        GD.Print("Serial Started");
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.T))
            startUp = !startUp;
    }
	private void OnPingTouchThreadTimerTimeout()
	{
		PingTouchThread();
	}
    private void PingTouchThread()
    {
        touchQueue.Enqueue(1);
    }
    private void TouchThread()
    {
        while(true)
        {
            if(p1Serial.IsOpen)
                ReadData(p1Serial);
            if(p2Serial.IsOpen)
                ReadData(p2Serial);
            if(touchQueue.Count > 0)
            {
                touchQueue.Dequeue();
                UpdateTouch();
            }
        }
    }
    private void OnDestroy()
    {
        touchThread.Abort();
        p1Serial.Close();
        p2Serial.Close();
    }

    private void ReadData(SerialPort Serial)
    {
        if (Serial.BytesToRead == 6)
        {
            recivData = Serial.ReadExisting();
            TouchSetUp(Serial, recivData); 
        }
    }
    private void TouchSetUp(SerialPort Serial, string data)
    {
        switch (Convert.ToByte(data[3]))
        {
            case 76:
            case 69:
                startUp = false;
                break;
            case 114:
            case 107:
                for (int i=1; i<5; i++)
                    settingPacket[i] = Convert.ToByte(data[i]);    
                Serial.Write(settingPacket, 0, settingPacket.Length);
                break;
            case 65:
                startUp = true;
                break;
        }
    }
    public static void UpdateTouch()
    {
        if (!startUp)
            return;
		p1Serial.Write(touchData, 0, 9);
		p2Serial.Write(touchData2, 0, 9);
    }

    public static void ChangeTouch(bool isP1, TouchArea touchArea, bool State)
    {
        if (isP1)
            ByteArrayExt.SetBit(touchData, (int)touchArea + 8, State);
        else
            ByteArrayExt.SetBit(touchData2, (int)touchArea + 8, State);
    }
	public enum TouchArea
    {
        A1 = 0, A2 = 1, A3 = 2, A4 = 3, A5 = 4, 
        A6 = 8, A7 = 9, A8 = 10, B1 = 11, B2 = 12, 
        B3 = 16, B4 = 17, B5 = 18, B6 = 19, B7 = 20, 
        B8 = 24, C1 = 25, C2 = 26, D1 = 27, D2 = 28, 
        D3 = 32, D4 = 33, D5 = 34, D6 = 35, D7 = 36, 
        D8 = 40, E1 = 41, E2 = 42, E3 = 43, E4 = 44, 
        E5 = 48, E6 = 49, E7 = 50, E8 = 51,
    }
}

public static class ByteArrayExt
{
    public static byte[] SetBit(this byte[] self, int index, bool value)
    { 
        var bitArray = new BitArray(self);
        bitArray.Set(index, value);
        bitArray.CopyTo(self, 0);
        return self;
    }
}