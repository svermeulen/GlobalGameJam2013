using UnityEngine;
using System;
using XInputDotNetPure;

public class InputPulse : MonoBehaviour
{
	public PlayerIndex ControllerIndex;
	public float Duration;
	public float LeftStrength;
	public float RightStrength;
	
	private float StartTime;
	private bool IsStarted;
	
	public void Begin()
	{
		StartTime = Time.time;
		IsStarted = true;
		GamePad.SetVibration(ControllerIndex, LeftStrength, RightStrength);
	}
	
	void Update()
	{
		if(IsStarted && Time.time - StartTime > Duration)
		{
			GamePad.SetVibration(ControllerIndex, 0.0f, 0.0f);
			IsStarted = false;
		}
	}
}

