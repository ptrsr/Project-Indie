using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;
	private Transform aim;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 200.0f;
	[SerializeField] private float rotationSpeed = 5.0f;

	void Start ()
	{
		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0);
	}

	void FixedUpdate ()
	{
		Move ();
		Aim ();
	}

	void Move ()
	{
		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (transform.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed);
			Quaternion aimRotation = aim.rotation;
			transform.rotation = rotation;
			aim.rotation = aimRotation;

			rb.AddForce ((transform.forward * Time.deltaTime * moveSpeed) - rb.velocity, ForceMode.VelocityChange);
		}
		else
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
	}

	void Aim ()
	{
		float x = Input.GetAxis ("Horizontal2");
		float y = Input.GetAxis ("Vertical2");

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (aim.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed * 2);
			aim.rotation = rotation;
		}
	}
}
