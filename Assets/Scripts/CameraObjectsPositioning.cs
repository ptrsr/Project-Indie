using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObjectsPositioning : MonoBehaviour {

	[Header ("Game Objects")]
	[SerializeField] private Transform maps;

	[Header ("Vectors")]
	[SerializeField] private Vector3 [] cameraPositions;

	private GameObject arena1, arena2, arena3;

	private bool canMove = false;

	void Awake ()
	{
		arena1 = maps.GetChild (0).gameObject;
		arena2 = maps.GetChild (1).gameObject;
		arena3 = maps.GetChild (2).gameObject;
	}

	void Update ()
	{
		if (!canMove)
			return;
		
		if (arena1.activeInHierarchy)
		{
			Camera.main.transform.position = cameraPositions [0];
			transform.position = cameraPositions [0];
		}
		else if (arena2.activeInHierarchy)
		{
			Camera.main.transform.position = cameraPositions [1];
			transform.position = cameraPositions [1];
		}
		else if (arena3.activeInHierarchy)
		{
			Camera.main.transform.position = cameraPositions [2];
			transform.position = cameraPositions [2];
		}
	}

	public void EnableThis ()
	{
		this.enabled = true;
		StartCoroutine (ActivateMoving ());
	}

	IEnumerator ActivateMoving ()
	{
		yield return new WaitForSeconds (1.5f);
		EnableMoving ();
	}

	public void EnableMoving ()
	{
		canMove = true;
	}

	void OnDisable ()
	{
		StopAllCoroutines ();
		canMove = false;
		this.enabled = false;
	}
}
