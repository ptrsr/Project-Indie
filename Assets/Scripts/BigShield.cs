using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShield : MonoBehaviour {

	void Start ()
	{
		Destroy (gameObject, 5);
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Bullet")
		{
			col.GetComponent <Bullet> ().Hit ();
			Destroy (gameObject);
			Destroy (col.gameObject);
		}
	}
}
