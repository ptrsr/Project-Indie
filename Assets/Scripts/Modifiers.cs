using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour {

	public static int pointsToVictory = 2;
	public static int lives = 1;
	public static int clipSize = 3;

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
