using Godot;
using System;
using System.Collections.Generic;

public partial class XRHaptic : Node
{
	private XROrigin3D origin;
	private static XRNode3D leftHandNode, rightHandNode;
	private static bool initialized = false;	

	public enum Hand
	{
		Left,
		Right
	}
	public enum HapticType
	{
		Light,
		Medium,
		Heavy
	}

	private static Dictionary<string, double[]> hapticTypePreset = new Dictionary<string, double[]>()
	{
		{ "Light", new double[] { 0.3, 0.02 } },
		{ "Medium", new double[] { 0.6, 0.06 } },
		{ "Heavy", new double[] { 1.0, 0.15 } }
	};

	public override void _Ready()
	{
		if (!initialized)
		{
			initialize();
			initialized = true;
		}
	}

	private void initialize()
	{
		origin = this.GetParent<XROrigin3D>();
		leftHandNode = origin.GetNode<XRNode3D>("LeftHand");
		rightHandNode = origin.GetNode<XRNode3D>("RightHand");
	}

	public static void SendHapticPulse(Hand hand, HapticType type)
	{
		string handName = hand.ToString();
		string hapticName = type.ToString();
		double[] hapticData = hapticTypePreset[hapticName];
		XRNode3D controller = hand == Hand.Left ? leftHandNode : rightHandNode;
		controller.TriggerHapticPulse("haptic", 0, hapticData[0], hapticData[1], 0);
	}
}
