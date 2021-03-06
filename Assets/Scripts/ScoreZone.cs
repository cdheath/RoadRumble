﻿using UnityEngine;
using System.Collections;

public class ScoreZone : MonoBehaviour {
	public Material negativeZoneMat;
	public Material negativeZoneParticleMat;
	GameObject gameMechanics;
	bool SetNegativeZones = false;
	// Use this for initialization
	void Start () 
	{
		gameMechanics = GameObject.Find ("GameMechanics");
	}

	void Update ()
	{
		if (SetNegativeZones) 
		{
			gameObject.renderer.material = negativeZoneMat;
			gameObject.GetComponent<ParticleSystemRenderer>().material = negativeZoneParticleMat;
		}
	}

	IEnumerator OnTriggerEnter(Collider collider)
	{
		if(this.enabled)
		{
			if(collider.gameObject.tag == "Player")
			{
				audio.Play ();
				WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
				WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
				yield return new WaitForSeconds(0.5f);
				//add to score
				//trigger reset
				if(SetNegativeZones)
				{
					collider.gameObject.SendMessage("addPoints", -100);
				}
				else
				{
					collider.gameObject.SendMessage("addPoints", 100);
				}
				gameMechanics.SendMessage("setNewGoal");
			}
		}
	}

	public void SetNegativeZonesTrue()
	{
		SetNegativeZones = true;
		StartCoroutine (NegativeScoreZoneTimer());
	}

	public void SetNegativeZonesFalse()
	{
		SetNegativeZones = false;
	}

	IEnumerator NegativeScoreZoneTimer()
	{
		yield return new WaitForSeconds (10); 
		if (SetNegativeZones) 
		{
			gameMechanics.SendMessage("setNewGoal");
		}
	}
}
