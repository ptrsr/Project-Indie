using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour {

	[SerializeField] private AudioClip [] audioClips;

	void Start ()
	{
		AudioSource sound = GetComponent <AudioSource> ();
		sound.clip = audioClips [Random.Range (0, audioClips.Length)];
		sound.Play ();
	}
}
