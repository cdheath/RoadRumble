﻿using UnityEngine;
using System.Collections;

public class GameMechanics : MonoBehaviour {

	GameObject[] goalZones;
	int activeZoneIndex;
	public float gameTimeLimit = -1;
	public string levelName;
	float gameTimeLeft;
	public float autoTimeout = -1;
	float timeToTimeout = -1;
	GameObject tvDisplay;
	GameObject gamePadDisplay;
	GameObject[] lights;
	int scoreLimit = 1200;
	int players;
	int player1Score = 0;
	int player2Score = 0;
	bool victory;
	int currentHighScore = -1;
	public GameObject pickupSound;

	// Use this for initialization
	void Start () 
	{

		playMusicOnTVAndGamePad ();

		tvDisplay = GameObject.Find ("TV Camera");
		gamePadDisplay = GameObject.Find ("Gamepad Camera");

		goalZones = GameObject.FindGameObjectsWithTag ("ScoreZone");
		//Debug.Log (goalZones.Length);
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
			gameTimeLimit = PlayerPrefs.GetInt("TimeLimit") * 60;
			gameTimeLeft = gameTimeLimit;
		}
		else
		{
			scoreLimit = PlayerPrefs.GetInt("ScoreLimit");
		}

		players = PlayerPrefs.GetInt ("Players");
		if(players == 1)
		{
			//Debug.Log("looking for p2");
			GameObject player2 = GameObject.Find("Player 2");
			player2.SetActive(false);
		}


		lights = GameObject.FindGameObjectsWithTag("LightSource");

		bool night = PlayerPrefs.GetString("Night") == "True";
		int on_off;

		if (!night) {
			disableLightMap ();
						foreach (GameObject light in lights) {
								//if night is true, only disable some lights
//								if (night) {
//				on_off = Random.Range(0,100);

//				if(on_off%2 == 0)
//				{
//					light.SetActive(false);
//				}
//								} else {
										light.SetActive (false);
//								}
						}
				}
		//turn the sun off
		if(night)
		{
			GameObject sun = GameObject.Find("Directional light");
		  	sun.SetActive(false);
		}
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
			gamePadDisplay.SendMessage("updateGameTime", gameTimeLeft);
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
		//Debug.Log("setting new goal");
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
		gamePadDisplay.SendMessage ("updateScore", receivedVal);

	}

	void victoryScreen()
	{
		int highScore = UpdateHighScores (player1Score, levelName, PlayerPrefs.GetInt ("TimeLimit").ToString());
		victory = true;
		if (players == 1) 
		{
			tvDisplay.SendMessage ("triggerSinglePlayerVictory", highScore);
			gamePadDisplay.SendMessage ("triggerSinglePlayerVictory", highScore);
		} 
		else 
		{
			tvDisplay.SendMessage ("triggerMultiPlayerVictory");
			gamePadDisplay.SendMessage ("triggerMultiPlayerVictory");
		}
	}

	void playMusicOnTVAndGamePad()
	{
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
	}

	int UpdateHighScores(int newScore, string levelName, string timeLimit)
	{
		if (currentHighScore == -1) 
		{
			currentHighScore = PlayerPrefs.GetInt (string.Format ("TopScore_{0}", levelName + timeLimit));
		}
		try 
		{
			if (currentHighScore >= newScore) 
			{
				return currentHighScore;
			}
		} 
		catch (System.Exception e) {}	

		currentHighScore = newScore;
		PlayerPrefs.SetInt(string.Format("TopScore_{0}", levelName + timeLimit), newScore);

		using (WiiUSAVECommand cmd = WiiUSave.SaveCommand(WiiUSave.accountNo)) 
		{
			long freespace = 0;
			WiiUSave.FSStatus savestatus = cmd.GetFreeSpaceSize(out freespace, WiiUSave.FSRetFlag.All);
			var needspace = Mathf.Max(1024 * 1024, WiiUPlayerPrefsHelper.rawData.Length);

			if(savestatus == WiiUSave.FSStatus.OK)
			{
				PlayerPrefs.Save ();
				savestatus = cmd.FlushQuota(WiiUSave.FSRetFlag.None);
			}
			else
			{
				Debug.Log("Save failed");
			}
		}
			return newScore;
	}
	
	void disableLightMap()
	{
		if (levelName != "LOSPRADOS") 
		{
			LightmapSettings.lightmaps = new LightmapData[]{};
		}
	}
}
