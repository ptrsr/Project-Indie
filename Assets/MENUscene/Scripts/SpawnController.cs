using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

	public GameObject PlayerTrack;
	[SerializeField] private Texture [] textures;

	void Start () {
		InvokeRepeating ("Spawn", 0, 3);
	}
	void Spawn () {
		GameObject tracks = Instantiate (PlayerTrack, transform.root);
		Destroy(tracks, 14);
	}
}
