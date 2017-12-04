using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour {


	private bool triggered;

	void OnTriggerEnter (Collider other) {
		if (!triggered) {
			transform.parent = other.gameObject.transform;
			if (transform.tag == "BodyPart")
				transform.GetComponent <Animator> ().enabled = true;
			triggered = true;
		}
	}
}
