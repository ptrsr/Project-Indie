using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour {

	public static bool multiplyingBullets = true;

	public static int pointsToVictory = 3; 
	public static int lives = 1;

	public static void Reset ()
	{
		multiplyingBullets = false;
		pointsToVictory = 3;
		lives = 1;
	}

	public static void EnableMultiplyingBullets (bool status)
	{
		multiplyingBullets = status;
	}

	public static void ChangeAmountOfPointsToWin (int amount)
	{
		pointsToVictory = amount;
	}

	public static void ChangeLives (int amount)
	{
		lives = amount;
	}
}
