using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MaiButtonArea : Area3D
{
	[Export]
	public VirtualKeyCode Key = VirtualKeyCode.RETURN;

	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
	}

	private void OnTochEntered(Area3D area)
	{
		EmulateKeyboard.PressKey(Key);
		GD.Print(this.Name + " Entered");
	}

	private void OnTochExited(Area3D area)
	{
		EmulateKeyboard.ReleaseKey(Key);
		GD.Print(this.Name + " Exited");
	}

}
