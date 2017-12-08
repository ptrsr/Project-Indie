using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftManager : MonoBehaviour {

	private LiftController [] lift;

	private float timer;
	private float maxTime;

	private bool isMoving = false;
	private bool canMove = false;
	private bool direction = true;

	void OnEnable ()
	{
		lift = new LiftController [4];

		lift [0] = transform.GetChild (0).GetComponent <LiftController> ();
		lift [1] = transform.GetChild (1).GetComponent <LiftController> ();
		lift [2] = transform.GetChild (2).GetComponent <LiftController> ();
		lift [3] = transform.GetChild (3).GetComponent <LiftController> ();

		foreach (LiftController item in lift)
			item.liftHeight = -3.0f;

		isMoving = false;
		canMove = true;
		direction = true;
		timer = 0.0f;
		maxTime = Random.Range (3, 16);
	}

	void Move ()
	{
		isMoving = true;
		timer = 0;
		GetComponent <BoxCollider> ().enabled = true;
	}

	void Update ()
	{
		if (timer < maxTime && !isMoving)
			timer += Time.deltaTime;
		else if (canMove)
			Move ();

		if (isMoving)
		{
			foreach (LiftController item in lift)
			{
				if (direction)
				{
					if (item.liftHeight > -45.0f)
						item.liftHeight -= 16.0f * Time.deltaTime;
					else
					{
						direction = false;
						Reset ();
					}
				}
				else
				{
					if (item.liftHeight < -3.0f)
						item.liftHeight += 16.0f * Time.deltaTime;
					else
					{
						direction = true;
						GetComponent <BoxCollider> ().enabled = false;
						Reset ();
					}
				}
			}
		}
	}

	void Reset ()
	{
		isMoving = false;
		timer = 0;
		maxTime = Random.Range (3, 16);
	}

	void OnTriggerStay (Collider col)
	{
		if (col.tag == "Player")
			canMove = false;
	}

	void OnTriggerExit (Collider col)
	{
		if (col.tag == "Player")
			canMove = true;
	}
}
