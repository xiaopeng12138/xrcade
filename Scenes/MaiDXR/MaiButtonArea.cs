using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MaiButtonArea : Area3D
{
	[Export]
	public VirtualKeyCode Key = VirtualKeyCode.RETURN;

	private int insideColliderCount = 0;

	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
	}

	private void OnTochEntered(Area3D area)
	{
		EmulateKeyboard.PressKey(Key);
		insideColliderCount += 1;

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Medium);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Medium);
		GD.Print(this.Name + " Entered" + "with " + area.Name);
	}

	private void OnTochExited(Area3D area)
	{
		insideColliderCount -= 1;
		if (insideColliderCount <= 0)
		{
			EmulateKeyboard.ReleaseKey(Key);
			insideColliderCount = 0;
		}

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Light);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Light);
		GD.Print(this.Name + " Exited" + "with " + area.Name);
	}

}
