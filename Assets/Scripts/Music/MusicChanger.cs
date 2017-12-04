using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class MusicChanger : MonoBehaviour {
	
	[Header ("Game Objects")]
	[SerializeField] private GameController gameController;

	private Transform activeBullets;
	private Settings settings;

	private int currentTrackNumber = 0;

    StudioEventEmitter emitter;

	void Start ()
	{
		activeBullets = gameController.activeBullets.transform;
		settings = ServiceLocator.Locate <Settings> ();
	}

	//True when starting game, false when exiting game
	public void Initialize (bool status)
	{
		if (status)
		{
			if (settings.GetBool (Setting.graduallySpeedingUp))
			{
				GetComponents <StudioEventEmitter> () [0].enabled = false;
				GetComponents <StudioEventEmitter> () [1].enabled = true;

				emitter = GetComponents <StudioEventEmitter> () [1];
			}
			else
			{
				GetComponents <StudioEventEmitter> () [0].enabled = true;
				GetComponents <StudioEventEmitter> () [1].enabled = false;

				emitter = GetComponents <StudioEventEmitter> () [0];

				ChangeTrack (6);
			}
		}
		else
		{
			GetComponents <StudioEventEmitter> () [0].enabled = true;
			GetComponents <StudioEventEmitter> () [1].enabled = false;

			emitter = GetComponents <StudioEventEmitter> () [0];

			ChangeTrack (5);
		}
	}

	public void PauseMusic (bool status)
	{
		RuntimeManager.PauseAllEvents (status);
	}

    void Update ()
    {
		if (settings.GetBool (Setting.graduallySpeedingUp) && GetComponents <StudioEventEmitter> () [1].enabled)
		{
			UpdateSpeedUp ();
			return;
		}

		if (Input.GetKeyDown (KeyCode.Alpha0))
			ChangeTrack (0);
		if (Input.GetKeyDown (KeyCode.Alpha1))
			ChangeTrack (1);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			ChangeTrack (2);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			ChangeTrack (3);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			ChangeTrack (4);
    }

	void UpdateSpeedUp ()
	{
		//Higher speed = higher pitch (-100 - 100)
		float pitch = activeBullets.childCount;
		emitter.SetParameter ("Pitch", pitch * 2 / gameController.playerAmount);
	}

	public void NextTrack ()
	{
		if (settings.GetBool (Setting.graduallySpeedingUp))
			return;
		
		if (gameController.MatchPoint ())
		{
			if (currentTrackNumber != 0)
				ChangeTrack (0);
		}
		else
			RandomTrack ();
	}

	public void OutroTrack ()
	{
		ChangeTrack (4);
	}

	void RandomTrack ()
	{
		int random;

		do random = Random.Range (1, 4);
		while (currentTrackNumber == random);

		ChangeTrack (random);
	}

	void ChangeTrack (int trackNumber)
	{
		print ("Change to " + trackNumber);

		emitter.SetParameter ("MainTheme", 0);
		emitter.SetParameter ("ActionLevel1", 0);
		emitter.SetParameter ("ActionLevel2", 0);
		emitter.SetParameter ("ActionLevel3", 0);
		emitter.SetParameter ("Outro", 0);
		emitter.SetParameter ("Intro", 0);
		emitter.SetParameter ("Build Up", 0);

		switch (trackNumber)
		{
		case 0:
			emitter.SetParameter ("MainTheme", 1);
			break;
		case 1:
			emitter.SetParameter ("ActionLevel1", 1);
			break;
		case 2:
			emitter.SetParameter ("ActionLevel2", 1);
			break;
		case 3:
			emitter.SetParameter ("ActionLevel3", 1);
			break;
		case 4:
			emitter.SetParameter ("Outro", 1);
			break;
		case 5:
			emitter.SetParameter ("Intro", 1);
			break;
		case 6:
			emitter.SetParameter ("Build Up", 1);
			break;
		}

		currentTrackNumber = trackNumber;
	}
}
