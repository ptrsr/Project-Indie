using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithBelt : MonoBehaviour {

	public bool direction;

	void OnTriggerStay (Collider col)
	{
		if (col.tag == "Player")
		{
			if (direction)
				col.transform.position -= new Vector3 (5.0f, 0.0f, 0.0f) * Time.deltaTime;
			else
				col.transform.position += new Vector3 (5.0f, 0.0f, 0.0f) * Time.deltaTime;
		}
	}
}
