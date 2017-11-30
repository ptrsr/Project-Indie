using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRotation : MonoBehaviour {

	private float curRotation = 0.0f;
	private float rotationLimit = 0.0f;

	public float rotationInterval = 3.0f;

	void Start ()
	{
		Invoke ("ChangeRotationLimit", rotationInterval);
	}

	void Update ()
	{
		if (curRotation < rotationLimit)
		{
			curRotation += 45.0f * Time.deltaTime;
			transform.rotation = Quaternion.Euler (new Vector3 (0.0f, curRotation, 0.0f));
		}
	}

	void ChangeRotationLimit ()
	{
		rotationLimit += 45.0f;
		Invoke ("ChangeRotationLimit", rotationInterval);
	}
}
