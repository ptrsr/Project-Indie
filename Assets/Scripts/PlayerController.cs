using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;
	[HideInInspector] public Transform aim;
	private Transform activeBullets;
	[HideInInspector] public Image cooldownBar;
	private Animator anim;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 400.0f;
	[SerializeField] private float rotationSpeed = 5.0f;
	[SerializeField] private int clipSize = 5;
	[SerializeField] private float cooldown = 3.0f;
	[SerializeField] private float shieldCooldown = 0.5f;
	[SerializeField] private float shieldDuration = 0.25f;
	[SerializeField] private float maxShieldAngle = 75.0f;

	private float curCooldown;
	private float curShieldCooldown;

	private bool parrying = false;

	[Header ("Prefabs")]
	[SerializeField] private GameObject bullet;

	void Start ()
	{
		#if UNITY_EDITOR
		if (name == "Player")
			name = "1";
		#endif

		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0);
		if (GameObject.Find ("ActiveBullest") != null)
			activeBullets = GameObject.Find ("ActiveBullets").transform;

		anim = GetComponent <Animator> ();
		anim.speed = 8;
		anim.SetBool ("Moving", false);

		curShieldCooldown = shieldCooldown;
	}

	void Update ()
	{
		UpdateAbilities ();
	}

	void FixedUpdate ()
	{
		Move ();
		Aim ();
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

			anim.SetBool ("Moving", true);
		}
		else
		{
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			anim.SetBool ("Moving", false);
		}
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

	void UpdateAbilities ()
	{
		if (Input.GetButtonDown ("FireP" + name) || Input.GetJoystickNames ().Length <= 1 && Input.GetButtonDown ("FirePC"))
			Shoot ();

		if (Input.GetButtonDown ("ParryP" + name) || Input.GetJoystickNames ().Length <= 1 && Input.GetButtonDown ("ParryPC"))
			Parry ();

		UpdateCooldown ();
	}

	void Shoot ()
	{
		if (curCooldown < cooldown)
			return;
		
		Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);

		curCooldown -= cooldown;

		print ("Player " + name + " Shoots");
	}

	public void ReflectBullet ()
	{
		GameObject newBullet = Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);
		newBullet.GetComponent <Bullet> ()._speed *= 2;
	}

	void UpdateCooldown ()
	{
		//Shoot cooldown
		if (curCooldown < cooldown * clipSize)
			curCooldown += Time.deltaTime;

		if (cooldownBar != null)
			cooldownBar.fillAmount = curCooldown / (cooldown * clipSize);

		//Shield cooldown
		if (curShieldCooldown < shieldCooldown)
			curShieldCooldown += Time.deltaTime;

		//Shield duration
		if (curShieldCooldown >= shieldDuration)
			parrying = false;
	}

	void Parry ()
	{
		if (curShieldCooldown < shieldCooldown)
			return;
		
		curShieldCooldown = 0.0f;
		parrying = true;
	}

	public bool CanParry (Vector3 bulletPosition)
	{
		Vector3 dir = aim.position - new Vector3 (bulletPosition.x, aim.position.y, bulletPosition.z);

		float angle = Vector3.Angle (aim.forward, dir);

		Debug.Log ("Angle: " + angle / 2);

		if (angle >= maxShieldAngle && parrying)
			return true;

		return false;
	}

	public void Die ()
	{
		if (cooldownBar != null)
			Destroy (cooldownBar.gameObject);
		
		Destroy (gameObject);
	}
}
