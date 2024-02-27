using Godot;
using Godot.Collections;
using System.IO.Ports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

public partial class MaiLightManager : Node
{
	[Export]
	public Array<Light3D> P1RingLights, P2RingLights;
	[Export]
	public Light3D P1BodyLight, P2BodyLight, P1DisplayLight, P2DisplayLight, P1SideLight, P2SideLight;

	private MaiLightSerial p1Serial, p2Serial;
	private Color cabLightColorDataP1, cabLightColorDataP2;
	private int p1RingLightIndex, p2RingLightIndex;
	private Color p1RingLightColor, p2RingLightColor;
	private bool isP1RingLightUpdate, isP2RingLightUpdate;
	private bool isP1CabLightUpdate, isP2CabLightUpdate;

	public override void _Ready()
	{
		p1Serial = new MaiLightSerial("COM51", 115200);
		p2Serial = new MaiLightSerial("COM53", 115200);
		p1Serial.OnCabLightUpdate += OnCabLightUpdateP1;
		p2Serial.OnCabLightUpdate += OnCabLightUpdateP2;
	}

	public override void _Process(double delta)
	{
		if (isP1CabLightUpdate)
			CabLightUpdateP1(cabLightColorDataP1);
			
		if (isP2CabLightUpdate)
			CabLightUpdateP2(cabLightColorDataP2);
		
		if (isP1RingLightUpdate)
			RingLightUpdateP1(p1RingLightIndex, p1RingLightColor);
		
		if (isP2RingLightUpdate)
			RingLightUpdateP2(p2RingLightIndex, p2RingLightColor);
	}

	private void CabLightUpdateP1(Color data)
	{
		if (P1BodyLight != null)
			P1BodyLight.LightColor = new Color(data.R, data.R, data.R);
		if (P1DisplayLight != null)
			P1DisplayLight.LightColor = new Color(data.G, data.G, data.G);
		if (P1SideLight != null)
			P1SideLight.LightColor = new Color(data.B, data.B, data.B);
		isP1CabLightUpdate = false;
	}

	private void CabLightUpdateP2(Color data)
	{
		if (P2BodyLight != null)
			P2BodyLight.LightColor = new Color(data.R, data.R, data.R);
		if (P2DisplayLight != null)
			P2DisplayLight.LightColor = new Color(data.G, data.G, data.G);
		if (P2SideLight != null)
			P2SideLight.LightColor = new Color(data.B, data.B, data.B);
		isP2CabLightUpdate = false;
	}
	private void RingLightUpdateP1(int index, Color color)
	{
		if (P1RingLights != null && P1RingLights.Count > index)
			P1RingLights[index].LightColor = color;
		isP1RingLightUpdate = false;
	}

	private void RingLightUpdateP2(int index, Color color)
	{
		if (P2RingLights != null && P2RingLights.Count > index)
			P2RingLights[index].LightColor = color;
		isP2RingLightUpdate = false;
	}

	private void OnCabLightUpdateP1(Color data)
	{
		cabLightColorDataP1 = data;
		isP1CabLightUpdate = true;
	}

	private void OnCabLightUpdateP2(Color data)
	{
		cabLightColorDataP2 = data;
		isP2CabLightUpdate = true;
	}

	private void OnRingLightUpdateP1(int index, Color color)
	{
		p1RingLightColor = color;
		p1RingLightIndex = index;
		isP1RingLightUpdate = true;
	}

	private void OnRingLightUpdateP2(int index, Color color)
	{
		p2RingLightColor = color;
		p2RingLightIndex = index;
		isP2RingLightUpdate = true;
	}

}