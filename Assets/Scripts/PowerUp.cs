using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
	
	private float Size;
	[HideInInspector] public Vector3 corPos;
	private bool canScale = false;

	enum Type
	{
		laser,
		speed,
		shield,
		ammo
	}

	[SerializeField] private Type type;

    void Start ()
	{
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.position = new Vector3(transform.position.x, 1, transform.position.y);
        transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(PosCor());
    }

	void Update ()
	{
        if (canScale)
        {
            transform.Rotate(0, 50 * Time.deltaTime, 0);
            if (transform.localScale.x <= 1.2f)
            {
                Size += 22 * Time.deltaTime;
                transform.localScale = new Vector3(Size, Size, Size);
            }
        }
    }
    private void OnTriggerEnter (Collider col)
    {
        if (col.tag == "Player")
        {
			PlayerController playerController = col.GetComponent <PlayerController> ();

			switch (type)
			{
			case Type.laser:
				playerController.LaserPowerUp ();
				break;
			case Type.ammo:
				playerController.AmmoPowerUp ();
				break;
			case Type.shield:
				playerController.ShieldPowerUp ();
				break;
			case Type.speed:
				playerController.SpeedPowerUp ();
				break;
			}

			Destroy (gameObject);
        }
    }
    IEnumerator PosCor()
    {
        yield return new WaitForSeconds (0.5f);
        transform.position = new Vector3 (corPos.x, 1, corPos.z);
        canScale = true;
    }
}
