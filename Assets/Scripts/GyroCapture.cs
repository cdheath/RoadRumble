﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class GyroCapture : MonoBehaviour {
	private WiiUGamePad gamePad;
	private float gyro;
	public int bufferSize = 20;
	private float[] gyroBuffer;
	private int bufferPtr;
	public bool test;
	public string levelName;
	public float timeForEventReset = 30.0f;
	private float speed = 0.5f;
	private bool allowEvent = true;
	private float delayInterval = 10.0f;
	public GameObject haboob;
	public GUISkin skin;
	private string tickerMessage = "";
	private int tickerWidth;
	//bool eventHappening=false;
	int tickPosition;
	public AudioClip newsSound;
	GameObject tvDisplay;
	GameObject gamepadDisplay;

	#region LosPradosVariables
	private GameObject implosionCloud;
	ArrayList casinoNames = new ArrayList(new[] {"Gold Bar Casino", "Pilgrim Club", "Ras Roost","Mr Snrubs","Los Prados Club"});
	bool allowCasinoDestroy = true;
	GameObject[] debrisPlanes;
	#endregion

	#region CragsburyVariables
	ArrayList brokenBridgeIndex;
	GameObject[] brokenBridges;
	#endregion

	#region FloretVariables
	GameObject flowers;
	#endregion
	
	#region FigwoodVariables
	GameObject crazyAI;
	#endregion

	#region HorrorLakeVariables
	private GameObject frozenLake;
	private GameObject lake;
	private bool lakeFrozen = false;
	#endregion

	private bool disableHaboob = false;

	// Use this for initialization
	void Start () {
		bufferPtr = 0;
		LoadBuffer();
		
		if (levelName.Equals ("HORRORLAKE")) {
						frozenLake = GameObject.FindGameObjectWithTag ("FrozenLake");
						frozenLake.SetActive (false);
				}

		if(levelName.Equals("LOSPRADOS")){
			debrisPlanes = GameObject.FindGameObjectsWithTag("debris");
			foreach(GameObject obj in debrisPlanes)
			{
				obj.SetActive(false);
			}
		}

		if(levelName.Equals("CRAGSBURY")){
			brokenBridgeIndex = new ArrayList(new[] {0,1,2});
			brokenBridges = new GameObject[3];
			brokenBridges[0] = GameObject.FindGameObjectWithTag("brokenBridge0");
			brokenBridges[1] = GameObject.FindGameObjectWithTag("brokenBridge1");
			brokenBridges[2] = GameObject.FindGameObjectWithTag("brokenBridge2");

			foreach(var obj in brokenBridges)
			{
				obj.SetActive(false);
			}
		}
		
		if(levelName.Equals("FLORETVILLAGE")){
			flowers = GameObject.Find("Festival_Plants");
			flowers.SetActive(false);
		}
		
		if(levelName.Equals("FIGWOODSTUDIOS")){
			crazyAI = GameObject.Find("Crazy AI Car");
			crazyAI.SetActive(false);
		}

		//Get the HUD objects for the news ticker
		tvDisplay = GameObject.Find ("TV Camera");
		gamepadDisplay = GameObject.Find ("Gamepad Camera");
	}
	
	// Update is called once per frame
	void Update () {
		gamePad = WiiUInput.GetGamePad ();
		gyro = gamePad.gyro.magnitude;
		
		float sum = UpdateBuffer (gyro);
		
//		//Debug.Log ("gyro: " + gyro);
		//Debug.Log ("sum: " + sum);
		//Debug.Log ("event allowed" + allowEvent.ToString ());
		if (sum > (float)bufferSize/2  &&  allowEvent) 
		{

			GyroEvent();
			allowEvent = false;
			LoadBuffer();
		}

		if (test && Input.GetKey (KeyCode.M) && allowEvent) 
		{
			//Debug.Log ("Gyroevent Triggered by Test Key");
			GyroEvent();
			allowEvent = false;
			LoadBuffer();
		}
//add delay
		if (!allowEvent) 
		{
			if(delayInterval > 0)
			{
				delayInterval -= Time.deltaTime;
			}
			else
			{
				allowEvent = true;
				delayInterval = 1;
			}
		}
	}
	
	float UpdateBuffer(float gyroMag)
	{
		gyroBuffer[bufferPtr] = gyroMag;
		bufferPtr++;
		
		
		if (bufferPtr == bufferSize) 
		{
			bufferPtr = 0;
		}
		
		return BufferSum();
	}
	
	void LoadBuffer()
	{
		gyroBuffer = new float[bufferSize];
		
		for (int i = 0; i < bufferSize; i++)
		{
			gyroBuffer[i] = 0;
		}
	}
	
	float BufferSum()
	{
		float sum = 0f;
		for(int i = 0; i < bufferSize; i++)
		{
			sum += gyroBuffer[i];
		}
		
		return sum;
	}
	
	void GyroEvent()
	{
		//eventHappening = true;
		//tickPosition = Screen.currentResolution.width;
		audio.Play ();
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
		//Debug.Log ("Gyroevent Triggered");
		//Debug.Log ("levelName: " + levelName);
		switch (levelName) 
		{
		case "REGULARSVILLE": StopAllNonPlayerCars();
			tickerMessage = "Robots begin world take over by stopping Regularsville's self driving cars. Still worlds most boring town.";
			break;
		case "TEMPE": CreateHaboob();
			tickerMessage = "We've awakened the mummy's curse! No wait, it's just a dust storm.";
			break;
		case "HORRORLAKE": 
			tickerMessage = LakeEvent();
			break;
		case "LOSPRADOS": ImplodeRandomCasino();
			tickerMessage = "Los Prados casino torn down to make way for a store that sells designer mousepads! Residents create line for grand opening.";
			break;
		case "CRAGSBURY" : CollapseRandomBridge();
			tickerMessage = "Bridge designed by local elementary school collapses into the canyon.";
			break;
		case "FLORETVILLAGE" : flowers.SetActive(true);
			tickerMessage = "Village blocked to new traffic as Flower Festival begins.";
			StartCoroutine(ResetObjectBasedEvents(flowers));
			break;
		case "FIGWOODSTUDIOS" : crazyAI.SetActive(true);
			tickerMessage = "Crazed celeb begins vehicular rampage after craft services runs out of danishes. Stay tuned to find out who!";
			StartCoroutine(ResetObjectBasedEvents(crazyAI));
			break;
		case "MARIUSMESA": SwitchScoreZones();
			tickerMessage = "After ignoring mortals for millennia, the pantheon reemerges to interfere in driving contest.  Avoid the red zones!";
			break;
		}
		//Each character is ~11 pixels, so multiply length by 11 to get necessary ticker size
		tickerWidth = tickerMessage.Length * 11;

		
		string[] tickArgs = {tickerWidth.ToString(), tickerMessage};
		
		tvDisplay.SendMessage("runTicker", tickArgs);
		gamepadDisplay.SendMessage("runTicker", tickArgs);
	}

	void StopAllNonPlayerCars()
	{
		GameObject[] aiCars = GameObject.FindGameObjectsWithTag("aiCar");

		foreach (var car in aiCars) 
		{
			AI aiScript = car.GetComponent<AI>();
			aiScript.maxSpeedRange = 0;
			aiScript.StopAIVehicle();
		}
	}

	void CreateHaboob()
	{
		GameObject gamepadCamera = GameObject.FindGameObjectWithTag ("GamePadCamera");;
		if (!disableHaboob) 
		{						
			gamepadCamera.particleSystem.Play ();
			disableHaboob = true;
			StartCoroutine ("RollInFog");
		}		
	}

	IEnumerator RollInFog()
	{
		float startLevel = 0.0f;
		float endLevel = 0.8f;
		float speed = 1.0f / 3.0f;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime*speed) 
		{
			Color color = haboob.renderer.material.color;
			color.a =  Mathf.Lerp(startLevel, endLevel, t);
			haboob.renderer.material.color = color;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(timeForEventReset);
		
		StartCoroutine ("RollOutFog");
	}

	IEnumerator RollOutFog()
	{

		float startLevel = 0.8f;
		float endLevel = 0.0f;
		float speed = 1.0f / 3.0f;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime*speed) 
		{
			Color color = haboob.renderer.material.color;
			color.a =  Mathf.Lerp(startLevel, endLevel, t);
			haboob.renderer.material.color = color;
			yield return new WaitForEndOfFrame();
		}

		GameObject gamepadCamera = GameObject.FindGameObjectWithTag ("GamePadCamera");;
		gamepadCamera.particleSystem.Stop ();
		disableHaboob = false;
	}

	string LakeEvent()
	{
		string message = "";
		if (!lakeFrozen) 
		{
			FreezeLake();
			message = "Lake freezes over amidst severe cold snap!";
			lakeFrozen = true;
		} 
		else 
		{
			ThawLake();
			message = "Sudden heat wave causes lake to thaw!";
			lakeFrozen = false;
		}
		return message;
	}

	void FreezeLake()
	{
		lake = GameObject.FindGameObjectWithTag ("Lake");

		if (lake != null && lake.activeSelf == true) {
				lake.SetActive (false);
				}
			frozenLake.SetActive(true);
	}

	void ThawLake()
	{	
		if (frozenLake != null && frozenLake.activeSelf == true) {
			frozenLake.SetActive (false);
		}
		lake.SetActive(true);
	}

	string GetLevelName()
	{		
		if (test) { return levelName.ToUpper();}

		//Debug.Log (string.Format("Triggered event for level: {0}", PlayerPrefs.GetString("LevelChoice")));
		return PlayerPrefs.GetString("LevelChoice").ToUpper();
	}

	void ImplodeRandomCasino()
	{
		if (allowCasinoDestroy) 
		{
				string selectedCasino = SelectCasinoToDestroy ();
				switch (selectedCasino) {
				case "Gold Bar Casino":
						CasinoImplosion ("GB", "Gold Bar Casino", "gbExplosion", 5.0f);
						break;
				case "Pilgrim Club":
						CasinoImplosion ("PC", "Pilgrim Club", "pcExplosion", 5.0f);
						break;
				case "Ras Roost":
						CasinoImplosion ("RR", "Ra\'s Roost Hotel & Casino", "rrExplosion", 5.0f);
						break;
				case "Mr Snrubs":
						CasinoImplosion ("MS", "Mr Snrub\'s Casino", "msExplosion", 5.0f);
						break;
				case "Los Prados Club":
						CasinoImplosion ("LP", "Los Prados Club", "lpExplosion", 5.0f);
						break;
				}

				allowCasinoDestroy = false;
				StartCoroutine(DelayNextCasinoDestroy (3.0f));
		}
	}

	string SelectCasinoToDestroy()
	{
		var selectedCasinoName = "none";
		if (casinoNames.Count > 0) 
		{
			var selectedPos = Random.Range(0, casinoNames.Count-1);
			selectedCasinoName = casinoNames[selectedPos].ToString();
			casinoNames.RemoveAt(selectedPos);
		}
		return selectedCasinoName;
	}

	void CasinoImplosion(string prefix, string casinoName, string explosionTag, float explosionDelay)
	{
		implosionCloud = GameObject.Find (prefix +" Cloud");
		implosionCloud.particleSystem.Play ();
		PlayAllByTag (explosionTag);

		StartCoroutine (DelayCasinoDestroy(prefix, casinoName, explosionDelay));
	}

	IEnumerator DelayCasinoDestroy(string prefix, string casinoName, float seconds)
	{
		yield return new WaitForSeconds (seconds);
		var gbCasino = GameObject.Find (casinoName);
		//implosionCloud.audio.Play();
		WiiUAudio.EnableOutputForAudioSource(implosionCloud.audio, WiiUAudioOutputDevice.TV);
		WiiUAudio.EnableOutputForAudioSource(implosionCloud.audio, WiiUAudioOutputDevice.GamePad);
		Destroy (gbCasino);
		debrisPlanes.Where(t => t.name.Equals(prefix +" Debris")).Single().SetActive(true);
	}

	IEnumerator DelayNextCasinoDestroy(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		allowCasinoDestroy = true;
	}

	void PlayAllByTag(string tag)
	{
		var particleSysObjects = GameObject.FindGameObjectsWithTag (tag);
		foreach (var obj in particleSysObjects) {
			obj.particleSystem.Play ();
				}
	}

	void CollapseRandomBridge()
	{
		int indexPos = -1;

		if (brokenBridgeIndex.Count > 0) 
		{
			int selectedPos = Random.Range(0, brokenBridgeIndex.Count-1);
			indexPos = int.Parse(brokenBridgeIndex[selectedPos].ToString());
			brokenBridgeIndex.RemoveAt(selectedPos);
		}

		if (indexPos != -1) {
			var bridgeTag = string.Format("bridge{0}", indexPos);
			GameObject bridge = GameObject.FindGameObjectWithTag(bridgeTag);
			bridge.SetActive(false);
			brokenBridges[indexPos].SetActive(true);
				}
	}

	void SwitchScoreZones()
	{
		//Debug.Log ("Switch Score Zones");
		var scoreZones = GameObject.FindGameObjectsWithTag ("ScoreZone");
		foreach (var zone in scoreZones) 
		{
			try
			{
				zone.GetComponent<ScoreZone>().SetNegativeZonesTrue();
			}
			catch(System.Exception e)
			{//Debug.Log (e);
			}
		}
	}

	IEnumerator ResetObjectBasedEvents(GameObject gameObj)
	{
		yield return new WaitForSeconds (10); 
		gameObj.SetActive (false);
	}
}
