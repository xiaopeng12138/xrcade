using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MaiTouchArea : Area3D
{
	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
	}

	private void OnTochEntered(Area3D area)
	{
		GD.Print(this.Name + "Touch Entered");
	}

	private void OnTochExited(Area3D area)
	{
		GD.Print(this.Name + "Touch Exited");
	}

}
