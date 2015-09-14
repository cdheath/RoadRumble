﻿using UnityEngine;
using System.Collections;

public class GameMechanics : MonoBehaviour {

	GameObject[] goalZones;
	int activeZoneIndex;
	public float gameTimeLimit = -1;
	float gameTimeLeft;
	public float autoTimeout = -1;
	float timeToTimeout = -1;
	GameObject tvDisplay;
	int scoreLimit = 1200;

	int player1Score = 0;
	int player2Score = 0;
	private bool pressedButton = false;

	public GameObject pickupSound;

	// Use this for initialization
	void Start () 
	{
		playMusicOnTVAndGamePad ();
		pressedButton = Input.anyKeyDown;

		tvDisplay = GameObject.Find ("TV Camera");

		goalZones = GameObject.FindGameObjectsWithTag ("ScoreZone");
		Debug.Log (goalZones.Length);
		activeZoneIndex = Random.Range (0, goalZones.Length);
		foreach(GameObject zone in goalZones)
		{
			zone.SetActive(false);
		}

		goalZones [activeZoneIndex].SetActive (true);
		goalZones [activeZoneIndex].particleSystem.Play ();

		timeToTimeout = autoTimeout;
		gameTimeLeft = gameTimeLimit;
		string gameMode = PlayerPrefs.GetString ("Mode");
		if(gameMode.CompareTo("Timed") == 0)
		{
			gameTimeLimit = 30;
			gameTimeLeft = gameTimeLimit;
		}

		int players = PlayerPrefs.GetInt ("Players");
		if(players == 1)
		{
			Debug.Log("looking for p2");
			GameObject player2 = GameObject.Find("Player 2");
			player2.SetActive(false);
		}


		GameObject[] lights = GameObject.FindGameObjectsWithTag("LightSource");
		//The night int functions as a bool, 0 = false, 1 = true
		int night = PlayerPrefs.GetInt("Night");
		int on_off;
		foreach(GameObject light in lights)
		{
			//if night is true, only disable some lights
			if(night == 1)
			{
				on_off = Random.Range(0,100);

				if(on_off%2 == 0)
				{
					light.SetActive(false);
				}
			}
			else
			{
				light.SetActive(false);
			}
		}
		//turn the sun off
		GameObject sun = GameObject.Find("Sunlight");
	//	sun.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
			if(timeToTimeout != -1)
			{
				timeToTimeout -= Time.deltaTime;
				if(timeToTimeout <= 0)
				{
					setNewGoal();
				}
			}

		if(gameTimeLimit != -1)
		{
			gameTimeLeft -= Time.deltaTime;
			if(gameTimeLeft <= 0)
			{
				victoryScreen();
			}

			tvDisplay.SendMessage("updateGameTime", gameTimeLeft);
			Debug.Log(gameTimeLeft);
		}
		else
		{
			if(player1Score >= scoreLimit)
			{
				victoryScreen();

			}
			else if(player2Score >= scoreLimit)
			{
				victoryScreen();

			}
		}
	}

	void setNewGoal()
	{
		Debug.Log("setting new goal");
		goalZones [activeZoneIndex].particleSystem.Stop ();
		goalZones [activeZoneIndex].SetActive (false);
		int newValue = Random.Range (0, goalZones.Length);
		while(newValue == activeZoneIndex)
		{
			newValue = Random.Range (0, goalZones.Length);
		}

		activeZoneIndex = newValue;
		goalZones [activeZoneIndex].SetActive (true);
		goalZones [activeZoneIndex].particleSystem.Play ();
		timeToTimeout = autoTimeout;
	}

	void updateScore(string[] receivedVal)
	{
		if(receivedVal[0].CompareTo("Player 1") == 0)
		{
			player1Score = int.Parse(receivedVal[1]);
		}
		else
		{
			player2Score = int.Parse(receivedVal[1]);
		}

		tvDisplay.SendMessage ("updateScore", receivedVal);
	}

	void victoryScreen()
	{
		tvDisplay.SendMessage ("triggerVictory");
		if(pressedButton)
		{
			Application.LoadLevel("MainMenu");
		}
	}

	void playMusicOnTVAndGamePad()
	{
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
	}
}
