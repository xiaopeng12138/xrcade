using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Area3DToKeyPress : Area3D
{
	[Export]
	public VirtualKeyCode Key = VirtualKeyCode.RETURN;
	[Export	]
	public VirtualKeyCode Key2 = VirtualKeyCode.NULL;
	[Export]
	public VirtualKeyCode Key3 = VirtualKeyCode.NULL;
	[Export]
	public VirtualKeyCode Key4 = VirtualKeyCode.NULL;

	private int insideColliderCount = 0;

	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
	}

	private void OnTochEntered(Area3D area)
	{
		EmulateKeyboard.PressKey(Key);
		if (Key2 != VirtualKeyCode.NULL)
			EmulateKeyboard.PressKey(Key2);
		if (Key3 != VirtualKeyCode.NULL)
			EmulateKeyboard.PressKey(Key3);
		if (Key4 != VirtualKeyCode.NULL)
			EmulateKeyboard.PressKey(Key4);
		insideColliderCount += 1;

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Heavy);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Heavy);
		GD.Print(this.Name + " Entered" + "with " + area.Name);
	}

	private void OnTochExited(Area3D area)
	{
		insideColliderCount -= 1;
		if (insideColliderCount <= 0)
		{
			EmulateKeyboard.ReleaseKey(Key);
			if (Key2 != VirtualKeyCode.NULL)
				EmulateKeyboard.ReleaseKey(Key2);
			if (Key3 != VirtualKeyCode.NULL)
				EmulateKeyboard.ReleaseKey(Key3);
			if (Key4 != VirtualKeyCode.NULL)
				EmulateKeyboard.ReleaseKey(Key4);

			insideColliderCount = 0;
		}

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Medium);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Medium);
		GD.Print(this.Name + " Exited" + "with " + area.Name);
	}

}
