using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	private string death = "event:/RobotDeathSound";
	private string shooting = "event:/ShootingSound";
	private string explosion = "event:/ExplosionSound";
	private string parry = "event:/ParryHit";

	FMOD.Studio.EventInstance deathSound;
	FMOD.Studio.EventInstance shootingSound;
	FMOD.Studio.EventInstance explosionSound;
	FMOD.Studio.EventInstance parrySound;

	public void PlayDeathSound ()
	{
		deathSound = FMODUnity.RuntimeManager.CreateInstance (death);
		deathSound.setVolume (4);
		deathSound.start ();
	}

	public void PlayShootingSound ()
	{
		shootingSound = FMODUnity.RuntimeManager.CreateInstance (shooting);
		shootingSound.setVolume (1.5f);
		shootingSound.start ();
	}

	public void PlayExplosionSound ()
	{
		explosionSound = FMODUnity.RuntimeManager.CreateInstance (explosion);
		explosionSound.setVolume (2);
		explosionSound.start ();
	}

	public void PlayParrySound ()
	{
		parrySound = FMODUnity.RuntimeManager.CreateInstance (parry);
		parrySound.setVolume (1.5f);
		parrySound.start ();
	}

	public void StopAllSounds ()
	{
		deathSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		shootingSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		explosionSound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		parrySound.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		
	}
}
