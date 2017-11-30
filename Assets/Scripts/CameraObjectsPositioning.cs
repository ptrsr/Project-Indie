using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObjectsPositioning : MonoBehaviour {

	[Header ("Game Objects")]
	[SerializeField] private Transform maps;

	[Header ("Vectors")]
	[SerializeField] private Vector3 [] cameraPositions;

	private GameObject arena1, arena2;

	void Start ()
	{
		arena1 = maps.GetChild (0).gameObject;
		arena2 = maps.GetChild (1).gameObject;
	}

	void Update ()
	{
		if (arena1.activeInHierarchy)
		{
			transform.position = cameraPositions [0];
		}
		else if (arena2.activeInHierarchy)
		{
			transform.position = cameraPositions [1];
		}
	}
}
