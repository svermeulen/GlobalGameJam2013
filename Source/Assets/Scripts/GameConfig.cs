using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XInputDotNetPure;

public enum ControlType
{
    Controller1LeftJoystick,
    Controller1RightJoystick,
    Controller2LeftJoystick,
    Controller2RightJoystick,
    KeyboardArrows,
    KeyboardWasd,
}

public class GameConfig
{
	private static Player player1;
	private static Player player2;

    private static int selectedLevel;
	
	public static Player GetPlayer (PlayerIndex index)
	{
		switch(index)
		{
		case PlayerIndex.One:
			return player1;
		case PlayerIndex.Two:
			return player2;
		default:
			return null;
		}
	}
	
	public static void SetPlayer (PlayerIndex index, Player player)
	{
		switch(index)
		{
		case PlayerIndex.One:
			player1 = player;
			return;
		case PlayerIndex.Two:
			player2 = player;
			return;
		default:
			return;
		}
	}

    public static ControlType GetControlType(int playerIndex)
    {
        return (ControlType)PlayerPrefs.GetInt("playerControls" + playerIndex);
    }

    public static void SetControlType(int playerIndex, ControlType type)
    {
        PlayerPrefs.SetInt("playerControls" + playerIndex, (int)type);
    }

    public static void SetLevel(int level)
    {
        selectedLevel = level;
    }

    public static int GetLevel()
    {
        return selectedLevel;
    }
}
