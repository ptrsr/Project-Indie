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
	private Animator bodyAnim;

	public Color playerColor;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 600.0f;
	[SerializeField] private float rotationSpeed = 10.0f;
	[SerializeField] private int clipSize = 5;
	[SerializeField] private float cooldown = 3.0f;
	[SerializeField] private float globalCooldown = 0.3f;
	[SerializeField] private float shieldCooldown = 0.5f;
	[SerializeField] private float shieldDuration = 0.25f;
	[SerializeField] private float maxShieldAngle = 60.0f;

	private float curGlobalCooldown;
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
		aim = transform.GetChild (0).GetChild (0);
		if (GameObject.Find ("ActiveBullest") != null)
			activeBullets = GameObject.Find ("ActiveBullets").transform;

		anim = GetComponent <Animator> ();
		bodyAnim = transform.GetChild (0).GetComponent <Animator> ();
		anim.speed = 8;
		anim.SetBool ("Moving", false);

		curShieldCooldown = shieldCooldown;

		bodyAnim.SetInteger ("playerClip", 0);
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

		x = Input.GetAxis ("HorizontalMoveP" + name);
		y = Input.GetAxis ("VerticalMoveP" + name);

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (transform.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed);
			Quaternion aimRotation = aim.rotation;
			transform.rotation = rotation;
			aim.rotation = aimRotation;

			rb.AddForce ((transform.forward * Time.deltaTime * moveSpeed) - rb.velocity, ForceMode.VelocityChange);

			if (!anim.GetBool ("Moving"))
				anim.SetBool ("Moving", true);
		}
		else
		{
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			if (anim.GetBool ("Moving"))
				anim.SetBool ("Moving", false);
		}
	}

	void Aim ()
	{
		float x, y;

		x = Input.GetAxis ("HorizontalAimP" + name);
		y = Input.GetAxis ("VerticalAimP" + name);

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (aim.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed * 2);
			aim.rotation = rotation;
		}
	}

	void UpdateAbilities ()
	{
		if (Input.GetButtonDown ("FireP" + name))
			Shoot ();

		if (Input.GetButtonDown ("ParryP" + name))
			Parry ();

		UpdateCooldown ();
	}

	void Shoot ()
	{
		if (curCooldown < cooldown || curGlobalCooldown < globalCooldown)
			return;

		bodyAnim.SetInteger ("playerClip", 1);
		Invoke ("Idle", globalCooldown);

		Instantiate (bullet, new Vector3 (aim.position.x, aim.position.y, aim.position.z) + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);

		curGlobalCooldown = 0;
		curCooldown -= cooldown;

		print ("Player " + name + " Shoots");
	}

	void Idle ()
	{
		bodyAnim.SetInteger ("playerClip", 0);
	}

	public void ReflectBullet ()
	{
		if (Modifiers.multiplyingBullets)
		{
			GameObject newBullet = Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y - 40.0f, 0.0f)), activeBullets);
			newBullet.GetComponent <Bullet> ()._speed *= 2;

			GameObject newBullet2 = Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y + 40.0f, 0.0f)), activeBullets);
			newBullet2.GetComponent <Bullet> ()._speed *= 2;
		}
		else
		{
			GameObject newBullet = Instantiate (bullet, aim.position + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);
			newBullet.GetComponent <Bullet> ()._speed *= 2;
		}
	}

	void UpdateCooldown ()
	{
		//Shoot cooldown
		if (curCooldown < cooldown * clipSize)
			curCooldown += Time.deltaTime;

		if (cooldownBar != null)
			cooldownBar.fillAmount = curCooldown / (cooldown * clipSize);

		//Shoot global cooldown
		if (curGlobalCooldown < globalCooldown)
			curGlobalCooldown += Time.deltaTime;

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
		bodyAnim.SetInteger ("playerClip", 2);
		Invoke ("Idle", shieldCooldown);
	}

	public bool CanParry (Vector3 bulletPosition)
	{
		Vector3 dir = aim.position - new Vector3 (bulletPosition.x, aim.position.y, bulletPosition.z);

		float angle = Vector3.Angle (aim.forward, dir);

		print ("Angle: " + angle / 2);

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
