using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;
	private Transform aim;
	private Transform activeBullets;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 400.0f;
	[SerializeField] private float rotationSpeed = 5.0f;

	[Header ("Prefabs")]
	[SerializeField] private GameObject bullet;

	void Start ()
	{
		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0);
		activeBullets = GameObject.Find ("ActiveBullets").transform;
	}

	void FixedUpdate ()
	{
		Move ();
		Aim ();
		UpdateShooting ();
	}

	void Move ()
	{
		float x, y;

		if (Input.GetJoystickNames ().Length <= 1)
		{
			x = Input.GetAxis ("HorizontalMovePC");
			y = Input.GetAxis ("VerticalMovePC");
		}
		else
		{
			x = Input.GetAxis ("HorizontalMoveP" + name);
			y = Input.GetAxis ("VerticalMoveP" + name);
		}

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
		float x, y;

		if (Input.GetJoystickNames ().Length <= 1)
		{
			x = Input.GetAxis ("HorizontalAimPC");
			y = Input.GetAxis ("VerticalAimPC");
		}
		else
		{
			x = Input.GetAxis ("HorizontalAimP" + name);
			y = Input.GetAxis ("VerticalAimP" + name);
		}

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (aim.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed * 2);
			aim.rotation = rotation;
		}
	}

	void UpdateShooting ()
	{
		if (Input.GetButtonDown ("FireP" + name) || Input.GetJoystickNames ().Length <= 1 && Input.GetButtonDown ("FirePC"))
		{
			Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);

			print ("Player " + name + " Shoots");
		}
	}
}
