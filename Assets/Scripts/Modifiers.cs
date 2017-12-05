using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour {

	public static int pointsToVictory = 4;
	public static int clipSize = 5;

	public static void ChangeAmountOfPointsToWin (int amount)
	{
		pointsToVictory = amount;
	}

	public static void ChangeClipSize (int amount)
	{
		clipSize = amount;
	}
}
