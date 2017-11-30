using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header ("Game Objects")]
	[SerializeField] private Text countdown;
	[SerializeField] private Text winText;
	[SerializeField] private LobbyController lobbyController;
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject activeBullets;
	[SerializeField] private GameObject musicManager;

	private Transform players;

	[HideInInspector] public int playerAmount;
	[HideInInspector] public bool gameStarted, gameFinished;
	private bool canActivatePlayerController;
	public float timeScale = 1.0f;

	public Dictionary <string, int> victories;

	private Settings settings;

	void Start ()
	{
		settings = ServiceLocator.Locate <Settings> ();
	}

	public void SetupGame (Transform _players)
	{
		musicManager.SetActive (true);

		players = _players;
		playerAmount = players.childCount;

		victories = new Dictionary <string, int> ();

		for (int i = 0; i < playerAmount; i++)
			victories.Add (players.GetChild (i).name, 0);
		
		winText.gameObject.SetActive (false);
		winText.GetComponent <Animator> ().enabled = true;

		InitializeGame (1.0f);
	}

	public void InitializeGame (float waitTime)
	{
		gameStarted = true;
		timeScale = 1.0f;
		StartCoroutine (StartCountdown (waitTime));
	}

	IEnumerator StartCountdown (float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		for (int i = 3; i > 0; i--)
		{
			countdown.text = i.ToString ();
			yield return new WaitForSeconds (0.5f);
		}

		countdown.text = "GO";

		if (gameStarted)
		{
		for (int i = 0; i < players.childCount; i++)
			players.GetChild (i).GetComponent <PlayerController> ().enabled = true;
		}

		yield return new WaitForSeconds (1.0f);

		countdown.text = "";
	}

	void Update ()
	{
		if (gameStarted)
		{
			CheckForVictory ();

			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Cancel)) //Should be another button
			{
				if (pausePanel.activeInHierarchy)
					PauseGame (false);
				else
					PauseGame (true);
			}
		}

		if (gameFinished && InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Submit))
			BackToLobby ();

		if (gameStarted && settings.GetBool (Setting.graduallySpeedingUp) && Time.timeScale < 50.0f)
		{
			timeScale = 1.0f + (activeBullets.transform.childCount * 0.1f);

			if (Time.timeScale != 0)
				Time.timeScale = timeScale;
		}

		if (!lobbyController.enabled)
		{
			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Settings) || InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Cancel))
			{
				lobbyController.enabled = true;
				ServiceLocator.Locate <Menu> ().SendCommand (1);
			}
		}
	}

	//True to pause, false to unpause
	public void PauseGame (bool status)
	{
		if (status)
		{
			Time.timeScale = 0;
			pausePanel.SetActive (true);
			pausePanel.transform.GetChild (0).GetChild (0).GetComponent <Button> ().Select ();

			if (players.GetChild (0).GetComponent <PlayerController> ().enabled)
				canActivatePlayerController = true;
			else
				canActivatePlayerController = false;
			
			for (int i = 0; i < players.childCount; i++)
				players.GetChild (i).GetComponent <PlayerController> ().enabled = false;

			musicManager.SetActive (false);
		}
		else
		{
			Time.timeScale = 1;
			pausePanel.SetActive (false);

			if (canActivatePlayerController)
			{
				for (int i = 0; i < players.childCount; i++)
					players.GetChild (i).GetComponent <PlayerController> ().enabled = true;
			}

			musicManager.SetActive (true);
		}
	}

	void CheckForVictory ()
	{
		if (playerAmount == 1)
			return;

		int alivePlayers = 0;
		for (int i = 0; i < players.childCount; i++)
		{
			if (!players.GetChild (i).GetComponent <PlayerController> ().dead)
				alivePlayers++;
		}

		if (alivePlayers == 1)
		{
			for (int i = 0; i < players.childCount; i++)
			{
				//Destroy the cooldown circles
				Destroy (players.GetChild (i).GetChild (2).gameObject);

				if (!players.GetChild (i).GetComponent <PlayerController> ().dead)
					StartCoroutine (Victory (players.GetChild (i)));
			}
		}
	}

	IEnumerator Victory (Transform player)
	{
		gameStarted = false;

		Time.timeScale = 1;

		foreach (GameObject bullet in GameObject.FindGameObjectsWithTag ("Bullet"))
			Destroy (bullet);

		PlayerController playerController = player.GetComponent <PlayerController> ();
		playerController.enabled = false;
		playerController.bodyAnim.SetInteger ("playerClip", 3);
		playerController.anim.SetBool ("Moving", false);

		Destroy (player.GetComponent <Rigidbody> ());

		victories [player.name]++;

		string playerColorName = playerController.playerColor;

		yield return new WaitForSeconds (1.5f);

		if (CheckForTotalVictory (player))
		{
			victories = null;
			winText.text = playerColorName + " player wins the set!";
			winText.gameObject.SetActive (true);
			gameFinished = true;

			Invoke ("LockText", 1.0f);

			print (playerColorName + " player wins the set!");
		}
		else
		{
			winText.text = playerColorName + " player wins this round!";
			winText.gameObject.SetActive (true);

			print (playerColorName + " player has " + victories [player.name] + " wins!");

			lobbyController.NewRound ();
		}
	}

	void LockText ()
	{
		winText.GetComponent <Animator> ().enabled = false;
		winText.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

	bool CheckForTotalVictory (Transform player)
	{
		if (victories [player.name] == Modifiers.pointsToVictory)
			return true;
		return false;
	}

	void ResetGame ()
	{
		foreach (GameObject bullet in GameObject.FindGameObjectsWithTag ("Bullet"))
			Destroy (bullet);
		
		victories = null;

		Time.timeScale = 1;

		StopAllCoroutines ();
		countdown.text = "";

		gameStarted = false;
		gameFinished = false;

		lobbyController.RemoveAllPlayers ();

		transform.root.GetComponent <Menu> ().SendCommand (1);
	}

	public void BackToLobby ()
	{
		musicManager.SetActive (false);

		ResetGame ();

		lobbyController.ResetLobby (true, true);

		lobbyController.BackToLobby ();
	}

	public void Restart ()
	{
		winText.gameObject.SetActive (false);
	}

	public void BackToMainMenu ()
	{
		musicManager.SetActive (false);

		ResetGame ();

		lobbyController.BackToMainMeny ();
	}
}
