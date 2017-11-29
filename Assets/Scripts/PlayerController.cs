using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Players;

public class PlayerController : MonoBehaviour {

	public Player playerNumber;

	private Rigidbody rb;
	[HideInInspector] public Transform aim;
	private Transform activeBullets;
	private Image cooldownBar;
	private Image outerCooldownBar;
	[HideInInspector] public Animator anim;
	[HideInInspector] public Animator bodyAnim;

	[HideInInspector] public string playerColor;

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
	private int curAmmo;

	private float curShieldCooldown;

	private bool parrying = false;
	private bool fallingThroughFloor = false;
	[HideInInspector] public bool dead = false;

	[Header ("Prefabs")]
	[SerializeField] private GameObject bullet;

	[Header ("Textures/Materials")]
	[SerializeField] private Texture blue;
	[SerializeField] private Texture red, green, yellow;
	[SerializeField] private Material player1Mat, player2Mat, player3Mat, player4Mat;

	void Awake ()
	{
		cooldownBar = transform.GetChild (2).GetChild (0).GetComponent <Image> ();
		outerCooldownBar = cooldownBar.transform.GetChild (0).GetComponent <Image> ();
		anim = transform.GetChild (1).GetComponent <Animator> ();
		bodyAnim = transform.GetChild (0).GetComponent <Animator> ();
	}

	void Start ()
	{
		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0).GetChild (0);
		if (GameObject.Find ("ActiveBullets") != null)
			activeBullets = GameObject.Find ("ActiveBullets").transform;

		anim.speed = 8;
		anim.SetBool ("Moving", false);

		curShieldCooldown = shieldCooldown;

		bodyAnim.SetInteger ("playerClip", 0);
	}

	void Update ()
	{
		if (!dead)
			UpdateAbilities ();

		if (fallingThroughFloor)
			FallThroughFloor ();
	}

	void FixedUpdate ()
	{
		if (dead)
			return;
		
		Move ();
		Aim ();
	}

	public void AssignColor ()
	{
		Renderer renderer = transform.GetChild (0).GetChild (0).GetComponent <Renderer> ();

		foreach (Renderer rend in renderer.GetComponentsInChildren <Renderer> ())
		{
			switch ((int) playerNumber)
			{
			case 1:
				rend.material = player1Mat;
				break;
			case 2:
				rend.material = player2Mat;
				break;
			case 23:
				rend.material = player3Mat;
				break;
			case 4:
				rend.material = player4Mat;
				break;
			}
		}

		switch ((int) playerNumber)
		{
		case 1:
			renderer.sharedMaterial.mainTexture = blue;
			outerCooldownBar.color = Color.blue;
			playerColor = "Blue";
			break;
		case 2:
			renderer.sharedMaterial.mainTexture = red;
			outerCooldownBar.color = Color.red;
			playerColor = "Red";
			break;
		case 3:
			renderer.sharedMaterial.mainTexture = green;
			outerCooldownBar.color = Color.green;
			playerColor = "Green";
			break;
		case 4:
			renderer.sharedMaterial.mainTexture = yellow;
			outerCooldownBar.color = Color.yellow;
			playerColor = "Yellow";
			break;
		}
	}

	void Move ()
	{
		float x, y;

		x = InputHandler.GetAxis (playerNumber, InputType.Move, Axis.Hor);
		y = InputHandler.GetAxis (playerNumber, InputType.Move, Axis.Ver);

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

		x = InputHandler.GetAxis (playerNumber, InputType.Aim, Axis.Hor);
		y = InputHandler.GetAxis (playerNumber, InputType.Aim, Axis.Ver);

		if (x != 0.0f || y != 0.0f)
		{
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Slerp (aim.rotation, Quaternion.AngleAxis (90.0f - angle, Vector3.up), Time.deltaTime * rotationSpeed * 2);
			aim.rotation = rotation;
		}
	}

	void UpdateAbilities ()
	{
		if (InputHandler.GetButton (playerNumber, Players.Button.Fire))
			Shoot ();

		if (InputHandler.GetButtonDown (playerNumber, Players.Button.Parry))
			Parry ();

		UpdateCooldown ();
	}

	void Shoot ()
	{
		if (curAmmo == 0 || curGlobalCooldown < globalCooldown)
			return;

		bodyAnim.SetInteger ("playerClip", 1);
		Invoke ("Idle", globalCooldown);

		Instantiate (bullet, new Vector3 (aim.position.x, aim.position.y, aim.position.z) + aim.forward * 2, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);

		curGlobalCooldown = 0.0f;
		curCooldown = 0.0f;
		curAmmo--;

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
		if (curAmmo != clipSize)
		{
			curCooldown += Time.deltaTime;

			if (curCooldown >= cooldown)
			{
				curAmmo++;
				if (curAmmo != clipSize)
					curCooldown = 0.0f;
			}
		}

		//Inner cooldown circle
		if (cooldownBar != null)
		{
			cooldownBar.fillAmount = curCooldown / cooldown;
			cooldownBar.transform.parent.rotation = Quaternion.Euler (new Vector3 (90.0f, 0.0f, 0.0f));
		}

		//Outer cooldown circle
		if (outerCooldownBar != null)
			outerCooldownBar.fillAmount = curAmmo * 0.11f;

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
		dead = true;

		Destroy (GetComponent <Rigidbody> ());

		if (anim.GetBool ("Moving"))
			anim.SetBool ("Moving", false);
		
		bodyAnim.SetInteger ("playerClip", 4);

		GetComponent <BoxCollider> ().enabled = false;

		Invoke ("ActivateFallingThroughFloor", 2.0f);

		Destroy (gameObject, 4.25f);
	}

	void ActivateFallingThroughFloor ()
	{
		fallingThroughFloor = true;
	}

	void FallThroughFloor ()
	{
		transform.position -= new Vector3 (0.0f, 2.0f, 0.0f) * Time.deltaTime;
	}
}
