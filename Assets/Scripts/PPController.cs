using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPController : MonoBehaviour {

	[SerializeField] private PostProcessingProfile menu;
	[SerializeField] private PostProcessingProfile game;

	public enum PP
	{
		menu,
		game
	}

	public void ChangePP (PP state)
	{
		switch (state)
		{
		case PP.menu:
			Camera.main.GetComponent <PostProcessingBehaviour> ().profile = menu;
			break;
		case PP.game:
			Camera.main.GetComponent <PostProcessingBehaviour> ().profile = game;
			break;
		}
	}
}
