using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MaiTouchArea : Area3D
{
	[Export]
	private bool isPlayer1 = true;
	private MaiSerialManager.TouchArea touchAarea;
	private int _insideColliderCount = 0;
	public static event Action touchDidChange;
	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
		touchAarea = (MaiSerialManager.TouchArea)Enum.Parse(typeof(MaiSerialManager.TouchArea), this.Name);
	}

	private void OnTochEntered(Area3D area)
	{
		GD.Print(this.Name + "Touch Entered");
		_insideColliderCount += 1;
		MaiSerialManager.ChangeTouch(isPlayer1, touchAarea, true);
        touchDidChange?.Invoke();
	}

	private void OnTochExited(Area3D area)
	{
		GD.Print(this.Name + "Touch Exited");
		_insideColliderCount -= 1;
        if (_insideColliderCount <= 0)
        {
            MaiSerialManager.ChangeTouch(isPlayer1, touchAarea, false);
            touchDidChange?.Invoke();
			_insideColliderCount = 0;
        }
	}
	
}
