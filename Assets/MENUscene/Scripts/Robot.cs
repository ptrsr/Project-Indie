using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

	public GameObject bodyPart;
	public GameObject bodyPivot;
	public float time;
	public float spawnDelay;
	public float animDelay;


	void Start () {
		//Time.timeScale = time;
		InvokeRepeating ("StaticLoop", spawnDelay, 3);
		Invoke ("Delay", animDelay);
	}

	void StaticLoop () {
		Instantiate (bodyPart, bodyPivot.transform.position, Quaternion.Euler (new Vector3 (bodyPart.transform.rotation.x, 270, bodyPart.transform.rotation.z)), bodyPivot.transform);
	}

	void Delay () {
		if (gameObject.name != "Robot005_Arm") {
			GetComponent<Animator> ().SetBool ("Play", true);

		}
	}
}
