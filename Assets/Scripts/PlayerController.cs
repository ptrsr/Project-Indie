using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;
	private Transform aim;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 400.0f;
	[SerializeField] private float rotationSpeed = 5.0f;

	[Header ("Prefabs")]
	[SerializeField] private GameObject bullet;

	[Header ("GameObjects")]
	[SerializeField] private Transform activeBullets;

	void Start ()
	{
		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0);
	}

	void FixedUpdate ()
	{
		Move ();
		Aim ();
		UpdateShooting ();
	}

	void Move ()
	{
		float x = Input.GetAxis ("HorizontalMoveP1");
		float y = Input.GetAxis ("VerticalMoveP1");

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
		float x = Input.GetAxis ("HorizontalAimP1");
		float y = Input.GetAxis ("VerticalAimP1");

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (aim.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed * 2);
			aim.rotation = rotation;
		}
	}

	void UpdateShooting ()
	{
		if (Input.GetButtonDown ("Fire1"))
			Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);
	}
}
