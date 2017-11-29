using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour {

	public static bool multiplyingBullets = false;
	public static bool graduallySpeedingUp = false;

	public static int pointsToVictory = 3; 
	public static int lives = 1;
	public static int clipSize = 3;

	public static void Reset ()
	{
		multiplyingBullets = false;
		graduallySpeedingUp = false;

		pointsToVictory = 3;
		lives = 1;
		clipSize = 3;
	}

	public static void EnableMultiplyingBullets (bool status)
	{
		multiplyingBullets = status;
	}

	public static void EnableGraduallySpeedingUp (bool status)
	{
		graduallySpeedingUp = status;
	}

	public static void ChangeAmountOfPointsToWin (int amount)
	{
		pointsToVictory = amount;
	}

	public static void ChangeLives (int amount)
	{
		lives = amount;
	}

	public static void ChangeClipSize (int amount)
	{
		clipSize = amount;
	}
}
