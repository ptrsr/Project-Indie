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
	private Image shieldCooldownBar;
	private Transform gun;
	[HideInInspector] public Animator anim;
	[HideInInspector] public Animator bodyAnim;

	[HideInInspector] public string playerColor;

	[Header ("Values")]
	[SerializeField] private float moveSpeed = 600.0f;
	[SerializeField] private float rotationSpeed = 10.0f;
	[SerializeField] private float cooldown = 3.0f;
	[SerializeField] private float globalCooldown = 0.3f;
	[SerializeField] private float maxShieldDuration = 2.5f;
	[SerializeField] private float shieldFillSpeed = 5.0f;
	[SerializeField] private float maxShieldAngle = 60.0f;
    [SerializeField] private float lightEffectDuration = 0.1f;

    [SerializeField] private Light[] lights;

	private float curGlobalCooldown;
	private float curCooldown;
	private int clipSize;
	private int curAmmo;

	private float curShieldDuration;

	private Settings settings;
	private GameController gameController;

	private bool parrying = false;
	private bool fallingThroughFloor = false;
    private bool multiplyingBullets = false;

	[HideInInspector] public bool dead = false;

	[Header ("Prefabs")]
	[SerializeField] private GameObject bullet;
	[SerializeField] private GameObject oil;

	[Header ("Textures/Materials")]
	[SerializeField] private Texture blue;
	[SerializeField] private Texture red, green, yellow;
	[SerializeField] private Material player1Mat, player2Mat, player3Mat, player4Mat;
	[SerializeField] private Texture shieldBlue, shieldRed, shieldGreen, shieldYellow;

	void Awake ()
	{
		cooldownBar = transform.GetChild (2).GetChild (0).GetComponent <Image> ();
		outerCooldownBar = cooldownBar.transform.GetChild (0).GetComponent <Image> ();
		shieldCooldownBar = cooldownBar.transform.GetChild (1).GetComponent <Image> ();
		anim = transform.GetChild (1).GetComponent <Animator> ();
		bodyAnim = transform.GetChild (0).GetComponent <Animator> ();
	}

	void Start ()
	{
		settings = ServiceLocator.Locate <Settings> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();

		rb = GetComponent <Rigidbody> ();
		aim = transform.GetChild (0).GetChild (0);
		if (GameObject.Find ("ActiveBullets") != null)
			activeBullets = GameObject.Find ("ActiveBullets").transform;

		anim.speed = 8;

		bodyAnim.SetInteger ("playerClip", 0);

        multiplyingBullets = ServiceLocator.Locate<Settings>().GetBool(Setting.multiplyingBulletsOnBlock);

		curShieldDuration = maxShieldDuration;
		clipSize = Modifiers.clipSize;
		curAmmo = 1;

		if (clipSize == curAmmo)
			curCooldown = cooldown;
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
		Renderer shieldRenderer = null;

		foreach (Renderer rend in renderer.GetComponentsInChildren <Renderer> ())
		{
			if (rend.transform.tag == "Gun")
			{
				gun = rend.transform;
				continue;
			}
			else if (rend.transform.tag == "Shield")
			{
				shieldRenderer = rend;
				continue;
			}
			
			switch ((int) playerNumber)
			{
			case 1:
				rend.material = player1Mat;
				break;
			case 2:
				rend.material = player2Mat;
				break;
			case 3:
				rend.material = player3Mat;
				break;
			case 4:
				rend.material = player4Mat;
				break;
			}
		}

        Laser laser = GetComponentInChildren<Laser>();

		switch ((int) playerNumber)
		{
		case 1:
			renderer.sharedMaterial.mainTexture = blue;
			shieldRenderer.material.mainTexture = shieldBlue;
			shieldCooldownBar.color = Color.blue;
			playerColor = "Blue";
            laser.Color = new Color (0.2f, 0.67f, 1, 0.2f);
			break;
		case 2:
			renderer.sharedMaterial.mainTexture = red;
			shieldRenderer.material.mainTexture = shieldRed;
			shieldCooldownBar.color = Color.red;
			playerColor = "Red";
            laser.Color = Color.red;
            break;
		case 3:
			renderer.sharedMaterial.mainTexture = green;
			shieldRenderer.material.mainTexture = shieldGreen;
			shieldCooldownBar.color = Color.green;
			playerColor = "Green";
            laser.Color = Color.green;
            break;
		case 4:
			renderer.sharedMaterial.mainTexture = yellow;
            laser.Color = Color.white;
			shieldRenderer.material.mainTexture = shieldYellow;
			shieldCooldownBar.color = Color.white;
			playerColor = "White";
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

		if (InputHandler.GetButtonDown (playerNumber, Players.Button.Parry) && curShieldDuration >= maxShieldDuration * 0.4f)
			StartParry ();
		if (InputHandler.GetButton (playerNumber, Players.Button.Parry) && curShieldDuration > 0 && parrying)
			Parry ();
		else if (parrying)
			StopParry ();

		UpdateCooldown ();
	}

	void Shoot ()
	{
		if (curAmmo == 0 || curGlobalCooldown < globalCooldown)
			return;

        foreach (Light light in lights)
            StartCoroutine(LightEffect(light));

        bodyAnim.SetInteger ("playerClip", 1);
		Invoke ("Idle", globalCooldown);

		Instantiate (bullet, gun.position + new Vector3 (0.0f, 0.35f, 0.0f) + aim.forward, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);

		curGlobalCooldown = 0.0f;

		if (curAmmo == Modifiers.clipSize)
			curCooldown = 0.0f;
		curAmmo--;

		print ("Player " + name + " Shoots");
	}

    IEnumerator LightEffect(Light light)
    {
        float intensity = light.intensity;
        float timer = 0;

        light.enabled = true;

        while (timer < lightEffectDuration)
        {
            light.intensity = Mathf.Sin((timer / lightEffectDuration) * Mathf.PI) * intensity;
            print(light.intensity);
            timer += Time.deltaTime;
            yield return null;
        }
        light.enabled = false;
        light.intensity = intensity;
    }

	void Idle ()
	{
		if (gameController.gameStarted)
			bodyAnim.SetInteger ("playerClip", 0);
	}

	public void ReflectBullet (float curSpeed)
	{
		float speedMultiplier = 2;

		Vector3 gunPos = new Vector3 (aim.position.x, gun.position.y + 0.35f, aim.position.z) + aim.forward;

		if (settings.GetBool (Setting.multiplyingBulletsOnBlock))
		{
			GameObject newBullet = Instantiate (bullet, gunPos - aim.right, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y - 40.0f, 0.0f)), activeBullets);
			Bullet _newbullet = newBullet.GetComponent <Bullet> ();
			_newbullet._speed = curSpeed * speedMultiplier;

			GameObject newBullet2 = Instantiate (bullet, gunPos + aim.right, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y + 40.0f, 0.0f)), activeBullets);
			Bullet _newbullet2 = newBullet2.GetComponent <Bullet> ();
			_newbullet2._speed = curSpeed * speedMultiplier;
		}
		else
		{
			GameObject newBullet = Instantiate (bullet, gunPos, Quaternion.Euler (new Vector3 (0.0f, aim.rotation.eulerAngles.y, 0.0f)), activeBullets);
			Bullet _newbullet = newBullet.GetComponent <Bullet> ();
			_newbullet._speed = curSpeed * speedMultiplier;
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
		if (curShieldDuration < maxShieldDuration && !parrying)
			curShieldDuration += shieldFillSpeed * Time.deltaTime;
		else if (curShieldDuration > maxShieldDuration)
			curShieldDuration = maxShieldDuration;

		//Shield cooldown circle
		if (shieldCooldownBar != null)
			shieldCooldownBar.fillAmount = (curShieldDuration / maxShieldDuration) * 0.5f;
	}

	void StartParry ()
	{
		parrying = true;
		bodyAnim.SetInteger ("playerClip", 2);
	}

	void Parry ()
	{
		curShieldDuration -= Time.deltaTime;
	}

	void StopParry ()
	{
		parrying = false;
		Idle ();
	}

	public bool CanParry (Vector3 bulletPosition)
	{
		Vector3 dir = aim.position - new Vector3 (bulletPosition.x, aim.position.y, bulletPosition.z);

		float angle = Vector3.Angle (aim.forward, dir) / 2;

		print ("Angle: " + angle);

		if (angle >= maxShieldAngle && parrying)
			return true;

		return false;
	}

    private void OnEnable()
    {
        ServiceLocator.Locate <CameraMovement> ().Track (gameObject);
    }

    private void OnDisable()
    {
        ServiceLocator.Locate <CameraMovement> ().UnTrack (gameObject);
    }

    public void Die ()
	{
		dead = true;

		Destroy (GetComponent <Rigidbody> ());

		if (anim.GetBool ("Moving"))
			anim.SetBool ("Moving", false);
		
		bodyAnim.SetInteger ("playerClip", 4);

		Invoke ("ActivateFallingThroughFloor", 2.0f);

		GetComponentInChildren <Laser> ().gameObject.SetActive (false);

		Instantiate (oil, new Vector3 (transform.position.x, 0.1f, transform.position.z), Quaternion.Euler (new Vector3 (90.0f, 0.0f, 0.0f)));

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

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
