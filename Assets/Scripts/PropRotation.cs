using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRotation : MonoBehaviour {

	private GameController gameController;
	private LobbyController lobbyController;

	private float curRotation = 0.0f;
	private float rotationLimit = 0.0f;

	[SerializeField] private float rotationInterval = 5.0f;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();
		lobbyController = gameController.lobbyController;
		
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
		if (gameController.gameStarted || lobbyController.selectingPlayers)
		{
			if (curRotation + 45.0f < rotationLimit)
			{
				curRotation = rotationLimit;
				transform.rotation = Quaternion.Euler (new Vector3 (0.0f, curRotation, 0.0f));
			}
		
			rotationLimit += 45.0f;
		}

		Invoke ("ChangeRotationLimit", rotationInterval);
	}

	void OnTriggerStay (Collider col)
	{
		if (col.tag == "Player" && col.transform.parent != transform)
			col.transform.parent = transform;
	}

	void OnTriggerExit (Collider col)
	{
		if (col.tag == "Player")
			col.transform.parent = gameController.players;
	}
}
