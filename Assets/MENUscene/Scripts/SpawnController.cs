using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

	public GameObject PlayerTrack;
	// Use this for initialization
	void Start () {
		InvokeRepeating ("Spawn", 0, 3);
	}
	void Spawn () {
		Destroy(Instantiate (PlayerTrack, transform.root),14);
	}
}
