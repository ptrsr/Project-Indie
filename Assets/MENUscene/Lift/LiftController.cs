using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : MonoBehaviour {


	public float liftHeight;
	[HideInInspector]
	public GameObject [] lift;
	// Use this for initialization
	void Start () {

		liftHeight = -3;
	}
	
	// Update is called once per frame
	void Update () {
		if (liftHeight >= -2) {
			liftHeight = -3;
		}
		// Bot, mid, top , TopBox, TopPivot
		lift[0].transform.eulerAngles = new Vector3 (0, 0, liftHeight);
		lift[1].transform.eulerAngles = new Vector3 (0, 0, - liftHeight* 1.5f);
		lift[2].transform.eulerAngles = new Vector3 (0, 0, liftHeight * 1.5f);
		lift[3].transform.position = new Vector3 (transform.position.x, lift [4].transform.position.y, transform.position.z);

		// Bot mid,
		lift[5].transform.eulerAngles = new Vector3 (0, 180, liftHeight);
		lift[6].transform.eulerAngles = new Vector3 (0, 180, - liftHeight* 1.5f);
		lift[7].transform.eulerAngles = new Vector3 (0, 180, liftHeight * 1.5f);
	}
}
