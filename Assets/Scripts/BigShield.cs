using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShield : MonoBehaviour {

	void Start ()
	{
		Destroy (gameObject, 3);
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Bullet")
		{
			col.GetComponent <Bullet> ().Hit ();
			Destroy (col.gameObject);
		}
	}
}
