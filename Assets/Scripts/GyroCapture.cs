using UnityEngine;
using System.Collections;

public class GyroCapture : MonoBehaviour {
	private WiiUGamePad gamePad;
	private float gyro;
	public int bufferSize = 50;
	private float[] gyroBuffer;
	private int bufferPtr;
	public bool test;
	public string testLevelChoice;
	private float speed = 0.5f;
	private string levelName;
	private GameObject frozenLake;

	// Use this for initialization
	void Start () {
		bufferPtr = 0;
		LoadBuffer ();

		levelName = GetLevelName ();
		if (levelName.Equals ("HORRORLAKE")) {
			frozenLake = GameObject.FindGameObjectWithTag ("FrozenLake");
			frozenLake.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		gamePad = WiiUInput.GetGamePad ();
		gyro = gamePad.gyro.magnitude;
		
		float sum = UpdateBuffer (gyro);
		
		//Debug.Log (gyro);
		if (sum > (bufferSize * 1.0f) ) 
		{
			Debug.Log ("Gyroevent Triggered");
			GyroEvent();
		//	Debug.Log(sum);
		}

		if (test && Input.GetKey (KeyCode.M)) 
		{
			Debug.Log ("Gyroevent Triggered by Test Key");
			GyroEvent();
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
		switch (levelName) 
		{
		case "REGULARSVILLE": StopAllNonPlayerCars();
			break;
		case "TEMPE": CreateHaboob();
			break;
		case "HORRORLAKE": FreezeLake();
			break;
		}
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
		if (!RenderSettings.fog) {
						var gamepadCamera = GameObject.FindGameObjectWithTag ("GamePadCamera");
						gamepadCamera.particleSystem.Play ();
						RenderSettings.fog = true;
						StartCoroutine ("RollInFog");
				}
	}

	IEnumerator RollInFog()
	{
		float targetDensity = 0.02f;
		float currentDensity = 0.0f;

		while (currentDensity < targetDensity) 
		{
			currentDensity = currentDensity + 0.005f;
			RenderSettings.fogDensity = currentDensity;
			yield return new WaitForSeconds (speed);
		}

	}

	void FreezeLake()
	{
		var lake = GameObject.FindGameObjectWithTag ("Lake");

		if (lake != null && lake.activeSelf == true) {
				lake.SetActive (false);
				}
			frozenLake.SetActive(true);
	}

	string GetLevelName()
	{		
		if (test) { return testLevelChoice.ToUpper();}

		Debug.Log (string.Format("Triggered event for level: {0}", PlayerPrefs.GetString("LevelChoice")));
		return PlayerPrefs.GetString("LevelChoice").ToUpper();
	}
}
