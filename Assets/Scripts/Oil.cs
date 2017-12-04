using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour {

	[SerializeField] private Sprite [] oilSprites;

	void Start ()
	{
		GetComponent <SpriteRenderer> ().sprite = oilSprites [Random.Range (0, 3)];
		Destroy (gameObject, 5);
	}
}
