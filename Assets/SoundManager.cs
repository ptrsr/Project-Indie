using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[FMODUnity.EventRef]

	private string death = "event:/RobotDeathSound";
	private string random = "event:/RandomRobotSounds";
	private string reload = "event:/ReloadSound";
	private string shooting = "event:/ShootingSound";

	FMOD.Studio.EventInstance deathSound;
	FMOD.Studio.EventInstance randomSound;
	FMOD.Studio.EventInstance reloadSound;
	FMOD.Studio.EventInstance shootingSound;

	public void PlayDeathSound ()
	{
		deathSound = FMODUnity.RuntimeManager.CreateInstance (death);
		deathSound.setVolume (4);
		deathSound.start ();
	}

	public void PlayRandomSound ()
	{
		randomSound = FMODUnity.RuntimeManager.CreateInstance (random);
		randomSound.start ();
	}

	public void PlayReloadSound ()
	{
		reloadSound = FMODUnity.RuntimeManager.CreateInstance (reload);
		reloadSound.start ();
	}

	public void PlayShootingSound ()
	{
		shootingSound = FMODUnity.RuntimeManager.CreateInstance (shooting);
		shootingSound.start ();
	}

	public void StopAllSounds ()
	{
		deathSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		randomSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		reloadSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		shootingSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
	}
}
