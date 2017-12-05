using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

	public GameObject bodyPart;
	public GameObject bodyPivot;
	public float time;
	public float spawnDelay;
	public float animDelay;

	[SerializeField] private Texture [] textures;

	[SerializeField] private Robot previousRobot;
	public int textureNumber;

	private bool armSpawned = false;

	void Start () {
		InvokeRepeating ("StaticLoop", spawnDelay, 3);
		Invoke ("Delay", animDelay);
	}

	void StaticLoop () {
		if (name == "Robot005" && !armSpawned || name == "Robot005_Arm" && !armSpawned)
		{
			textureNumber = 1;
			armSpawned = true;
		}
		else if (name == "Robot001")
			textureNumber = Random.Range (0, textures.Length);
		else
			textureNumber = previousRobot.textureNumber;

		GameObject part = Instantiate (bodyPart, bodyPivot.transform.position, Quaternion.Euler (new Vector3 (bodyPart.transform.rotation.x, 270, bodyPart.transform.rotation.z)), bodyPivot.transform);

		foreach (Renderer rend in part.GetComponentsInChildren <Renderer> ())
			rend.material.mainTexture = textures [textureNumber];
	}

	void Delay () {
		if (gameObject.name != "Robot005_Arm") {
			GetComponent<Animator> ().SetBool ("Play", true);
		}
	}
}
