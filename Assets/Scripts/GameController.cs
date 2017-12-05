using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header ("Game Objects")]
	[SerializeField] private Text countdown;
	[SerializeField] private Text winText;
	public LobbyController lobbyController;
	[SerializeField] private GameObject pausePanel;
	public GameObject activeBullets;
	[SerializeField] private GameObject musicManager;
	[SerializeField] private Transform scorePanel;
	public PPController ppController;
	[SerializeField] private GameObject winParticle;

	[HideInInspector] public Transform players;

	[HideInInspector] public int playerAmount;
	[HideInInspector] public bool gameStarted, gameFinished;
	private bool canActivatePlayerController;
	private float timeScale = 1.0f;
	private string playerStreak = "";
	private int playerStreakNumber = 0;
	private bool firstRound;

	private GameObject [] activePlayers;

	private Dictionary <string, int> victories;

	private Settings settings;

	void Start ()
	{
		settings = ServiceLocator.Locate <Settings> ();
	}

	public void SetupGame (Transform _players)
	{
		firstRound = true;

		ActivateMusic (true);

		players = _players;
		playerAmount = players.childCount;

		victories = new Dictionary <string, int> ();

		for (int i = 0; i < playerAmount; i++)
			victories.Add (players.GetChild (i).name, 0);
		
		winText.gameObject.SetActive (false);
		winText.GetComponent <Animator> ().enabled = true;

		for (int i = 0; i < playerAmount; i++)
		{
			PlayerController playerController = players.GetChild (i).GetComponent <PlayerController> ();
			scorePanel.GetChild ((int)playerController.playerNumber - 1).GetComponent <Text> ().text = playerController.playerColor + " player: 0";
		}

		InitializeGame (1.0f);
	}

	public void InitializeGame (float waitTime)
	{
		activePlayers = GameObject.FindGameObjectsWithTag ("Player");
		gameStarted = true;
		timeScale = 1.0f;
		StartCoroutine (StartCountdown (waitTime));
		ppController.ChangePP (PPController.PP.game);
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

			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Menu)) //Should be another button
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
			timeScale = 1.0f + (activeBullets.transform.childCount * 0.1f / (playerAmount / 1.5f));

			if (Time.timeScale != 0)
				Time.timeScale = timeScale;
		}

		if (!lobbyController.enabled)
		{
			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Settings) || InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Cancel))
			{
				lobbyController.enabled = true;
				ServiceLocator.Locate <Menu> ().SendCommand (1);
				ppController.ChangePP (PPController.PP.menu);
				lobbyController.settingsCanvas.SetActive (false);
			}
		}

		if (pausePanel.activeInHierarchy)
			musicManager.GetComponent <MusicChanger> ().PauseMusic (true);
	}

	//True to pause, false to unpause
	public void PauseGame (bool status)
	{
		if (status)
		{
			Time.timeScale = 0;
			pausePanel.SetActive (true);
			pausePanel.transform.GetChild (1).GetChild (0).GetChild (0).GetComponent <Button> ().Select ();

			foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player"))
				player.transform.parent = players;

			if (players.GetChild (0).GetComponent <PlayerController> ().enabled)
				canActivatePlayerController = true;
			else
				canActivatePlayerController = false;
			
			for (int i = 0; i < players.childCount; i++)
				players.GetChild (i).GetComponent <PlayerController> ().enabled = false;

			musicManager.GetComponent <MusicChanger> ().PauseMusic (true);
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

			musicManager.GetComponent <MusicChanger> ().PauseMusic (false);
		}
	}

	void CheckForVictory ()
	{
		if (playerAmount == 1)
			return;

		int alivePlayers = 0;
		for (int i = 0; i < activePlayers.Length; i++)
		{
			if (activePlayers [i] != null)
			{
				if (!activePlayers [i].GetComponent <PlayerController> ().dead)
					alivePlayers++;
			}
		}

		if (alivePlayers == 1)
		{
			foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player"))
				player.transform.parent = players;
			
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
		musicManager.GetComponent <MusicChanger> ().NextTrack ();

		gameStarted = false;

		Time.timeScale = 1;

		int bulletAmount = activeBullets.transform.childCount;

		foreach (GameObject bullet in GameObject.FindGameObjectsWithTag ("Bullet"))
			Destroy (bullet);

		PlayerController playerController = player.GetComponent <PlayerController> ();
		playerController.enabled = false;
		playerController.anim.SetBool ("Moving", false);

		Destroy (player.GetComponent <Rigidbody> ());

		victories [player.name]++;
		scorePanel.GetChild ((int)playerController.playerNumber - 1).GetComponent <Text> ().text = playerController.playerColor + " player: " + victories [player.name];

		if (playerStreak != playerController.playerColor)
		{
			playerStreak = playerController.playerColor;
			playerStreakNumber = 1;
		}
		else if (playerStreak == playerController.playerColor)
			playerStreakNumber++;

		if (!CheckForTotalVictory (player))
			playerController.bodyAnim.SetInteger ("playerClip", 3);

		yield return new WaitForSeconds (1.5f);

		if (CheckForTotalVictory (player))
		{
			if (!settings.GetBool (Setting.graduallySpeedingUp))
				musicManager.GetComponent <MusicChanger> ().OutroTrack ();

			victories = null;

			string [] victoryTexts = VictoryTexts (false, playerController, bulletAmount);
			winText.text = victoryTexts [Random.Range (0, victoryTexts.Length)];
			winText.transform.GetChild (0).gameObject.SetActive (true);
			winText.gameObject.SetActive (true);
			gameFinished = true;

			playerController.bodyAnim.SetInteger ("playerClip", 3);
			Instantiate (winParticle, player);

			Invoke ("LockText", 1.0f);
		}
		else
		{
			string [] victoryTexts = VictoryTexts (true, playerController, bulletAmount);
			winText.text = victoryTexts [Random.Range (0, victoryTexts.Length)];
			winText.gameObject.SetActive (true);
			lobbyController.NewRound ();
		}

		if (firstRound)
			firstRound = false;
	}

	string [] VictoryTexts (bool round, PlayerController player, int bulletAmount)
	{
		List <string> victoryTexts = new List <string> ();
		string playerColor = player.playerColor + " player";

		if (round)
		{
			//Round
			if (firstRound)
			{
				//First round
				victoryTexts.Add (playerColor + " is off to a good start!");
				victoryTexts.Add (playerColor + " has drawn first oil!");
			}
			else if (playerStreakNumber == 2)
			{
				//2 wins in a row
				victoryTexts.Add (playerColor + " is on a streak!");
				victoryTexts.Add (playerColor + " is double dipping!");
				victoryTexts.Add (playerColor + " is dominating!");
				victoryTexts.Add (playerColor + " is making it look easy!");
				victoryTexts.Add (playerColor + "! Somebody stop him!");
			}
			else if (playerStreakNumber > 2)
			{
				//3+ wins in a row
				victoryTexts.Add (playerColor + " is dominating!");
				victoryTexts.Add (playerColor + " is pimpin!");
				victoryTexts.Add (playerColor + " master race!");
				victoryTexts.Add (playerColor + " is running away with the game!");
				victoryTexts.Add (playerColor + " must be DDOSing the other players!");
			}
			else
			{
				//Default
				victoryTexts.Add (playerColor + " wins the round!");

				//Needs more stuff here
			}

			if (bulletAmount >= 15)
			{
				//At least 20 active bullets
				victoryTexts.Add (playerColor + " is the juke master!");
				victoryTexts.Add (playerColor + " is the juke king!");
				victoryTexts.Add (playerColor + "! Happy feet!");
			}
		}
		else
		{
			//Set
			//Default
			victoryTexts.Add (playerColor + " wins the set!");
			victoryTexts.Add (playerColor + " is victorious!");
			victoryTexts.Add (playerColor + " is CLEARLY the best robot!");
			victoryTexts.Add ("All hail " + playerColor + "!");
			victoryTexts.Add (playerColor + " is tasting the sweet taste of victory!");
		}

		return victoryTexts.ToArray ();
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

	public bool MatchPoint ()
	{
		foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player"))
			player.transform.parent = players;
		
		for (int i = 0; i < players.childCount; i++)
		{
			if (victories [players.GetChild (i).name] == Modifiers.pointsToVictory - 1)
				return true;
		}

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

		winText.transform.GetChild (0).gameObject.SetActive (false);

		gameStarted = false;
		gameFinished = false;

		lobbyController.RemoveAllPlayers ();

		transform.root.GetComponent <Menu> ().SendCommand (1);

		for (int i = 0; i < 4; i++)
			scorePanel.GetChild (i).GetComponent <Text> ().text = "";
	}

	public void BackToLobby ()
	{
		musicManager.GetComponent <MusicChanger> ().PauseMusic (false);
		ActivateMusic (false);

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
		musicManager.GetComponent <MusicChanger> ().PauseMusic (false);
		ActivateMusic (false);

		ResetGame ();

		lobbyController.BackToMainMeny ();
	}

	//True when starting game, false when exiting game
	void ActivateMusic (bool status)
	{
		musicManager.GetComponent <MusicChanger> ().Initialize (status);
	}
}
