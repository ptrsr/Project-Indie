using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehaviour : MonoBehaviour {

	private GameObject [] players;
	private int workerAmount;
	private GameController gameController;

	//True when activating, false when deactivating
	public void ActivateWorkers (bool status)
	{
		if (status)
		{
			players = GameObject.FindGameObjectsWithTag ("Player");
			gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();
			workerAmount = transform.childCount;

			for (int i = 0; i < workerAmount; i++)
				transform.GetChild (i).GetComponent <BoxThrowing> ().currentFollow = players [Random.Range (0, players.Length)].transform;

			if (gameController.settings.GetBool (Setting.powerUps))
				StartCoroutine (ThrowBox ());
		}
		else
		{
			players = null;
			StopAllCoroutines ();
		}
	}

	IEnumerator ThrowBox ()
	{
		yield return new WaitForSeconds (Random.Range (1.5f, 7));

		if (gameController.gameStarted)
			transform.GetChild (Random.Range (0, workerAmount)).GetComponent <BoxThrowing> ().ThrowPickup ();

		StartCoroutine (ThrowBox ());
	}

	void Update ()
	{
		if (players == null)
			return;

		for (int i = 0; i < workerAmount; i++)
		{
			Transform worker = transform.GetChild (i);
			BoxThrowing workerComponent = worker.GetComponent <BoxThrowing> ();

			if (workerComponent.currentFollow != null)
			{
				worker.GetChild (1).LookAt (workerComponent.currentFollow);
				worker.rotation = Quaternion.Euler (new Vector3 (worker.rotation.x, worker.GetChild (1).rotation.eulerAngles.y, worker.rotation.z));
			}
		}
	}
}
