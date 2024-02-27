using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MaiTouchArea : Area3D
{
	[Export]
	private bool isPlayer1 = true;
	private MaiTouchManager.TouchArea touchAarea;
	private int insideColliderCount = 0;
	public static event Action touchDidChange;
	public override void _Ready()
	{
		this.AreaEntered += OnTochEntered; 
		this.AreaExited += OnTochExited;
		touchAarea = (MaiTouchManager.TouchArea)Enum.Parse(typeof(MaiTouchManager.TouchArea), this.Name);
	}

	private void OnTochEntered(Area3D area)
	{
		GD.Print(this.Name + "Touch Entered" + "with " + area.Name);
		insideColliderCount += 1;
		MaiTouchManager.ChangeTouch(isPlayer1, touchAarea, true);
        touchDidChange?.Invoke();

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Medium);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Medium);
	}

	private void OnTochExited(Area3D area)
	{
		GD.Print(this.Name + "Touch Exited" + "with " + area.Name);
		insideColliderCount -= 1;
        if (insideColliderCount <= 0)
        {
            MaiTouchManager.ChangeTouch(isPlayer1, touchAarea, false);
            touchDidChange?.Invoke();
			insideColliderCount = 0;
        }

		if (area.Name.ToString()[0] == 'L')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Left, XRHaptic.HapticType.Light);
		else if (area.Name.ToString()[0] == 'R')
			XRHaptic.SendHapticPulse(XRHaptic.Hand.Right, XRHaptic.HapticType.Light);
	}
	
}
