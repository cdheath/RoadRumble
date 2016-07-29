using UnityEngine;
using System.Collections;

public class Gamepad_HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture popup_background;
	float gameTimeLeft = -1;
	int player1Score = 0;
	int player2Score = 0;
	float tickerPosition;
	int tickerWidth;
	string tickerMsg;
	bool victory = false;
	bool tickerRunning = false;
	bool paused = false;
	int highScore;
	bool singlePlayer = false;
	protected Vector2 touchPosition = new Vector2(-1f,-1f);	// x,y coords on gamePad for touch
	GameObject[] aiCars = null;
	[HideInInspector]
	public GameObject player1 = null;
	[HideInInspector]
	public GameObject player2 = null;
	Rect page;
	// Use this for initialization
	void Start () 
	{
		page.width = 854;
		page.height = 480;
		page.x = 0;
		page.y = 0;
	}
	
	// Update is called once per frame
	void Update () {
		touchPosition = (Input.touchCount > 0) ? Input.touches[0].position : new Vector2(-1f,-1f);

//		//Debug.LogWarning("touch is: " + touchPosition.ToString());
/*		if(victory && touchPosition.x > -1)
		{
			unpauseGame();
			Application.LoadLevel("MainMenu");
		}
	*/
	}
	[GuiTarget(GuiTarget.Target.GamePad)]
	void OnGUI()
	{
		if(skin != null)
		{
			GUI.skin = skin;
		}

		if(victory)
		{
			string winnerMsg;
			string rightSideMsg;
			string rightSideScore;
			pauseGame(false);
			if(singlePlayer)
			{
				winnerMsg = "Level High Scores";
				GUI.DrawTexture(new Rect(10,50,page.width-20,page.height-100), popup_background);
				GUI.Label(new Rect(0,100, page.width, 50),winnerMsg);

				rightSideMsg = "Top Score";
				rightSideScore = highScore.ToString();
			}
			else 
			{
				if(player1Score > player2Score)
				{
					winnerMsg = "Player 1 Wins!";
				}
				else if(player1Score < player2Score)
				{
					winnerMsg = "Player 2 Wins!";
				}
				else
				{
					winnerMsg = "Tie!";
				}

				rightSideMsg = "Player 2";
				rightSideScore = player2Score.ToString();
			}
			
			GUI.DrawTexture(new Rect(10,50,page.width-20, page.height-100), popup_background);
			GUI.Label(new Rect(0,100, page.width, 50),winnerMsg);
			
			GUI.Label(new Rect(0,150, page.width/2, 50),"Player 1");
			GUI.Label(new Rect(0,200, page.width/2, 50),player1Score.ToString(), GUI.skin.GetStyle("number"));

			GUI.Label(new Rect(page.width/2,150, page.width/2, 50),rightSideMsg);
			GUI.Label(new Rect(page.width/2,200, page.width/2, 50),rightSideScore, GUI.skin.GetStyle("number"));
			
			GUI.Label(new Rect(0,300, page.width, 50),"Tap to continue...");

			if (GUI.Button(new Rect(10,50,page.width-20,page.height-100), ""))
			{
				unpauseGame(false);
				Application.LoadLevel("MainMenu");
			}
		}
		else if(paused)
		{
			skin.button.fontSize = 40;
			GUI.DrawTexture(new Rect(10,50,page.width-20, page.height-100), popup_background);
			
			if(GUI.Button(new Rect(page.width/3 - 100, page.height/4, 500, 50),"Resume"))
			{
				unpauseGame(true);
			}
			
			if (GUI.Button(new Rect(page.width/3 - 100 ,page.height/4 + 100, 500, 50),"Return to Main Menu"))
			{
				unpauseGame(true);
				Application.LoadLevel("MainMenu");
			}
		}
		else
		{
			if(gameTimeLeft != -1)
			{
				GUI.Label (new Rect (page.width/2, 30, 50, 40), ((int)gameTimeLeft).ToString(), GUI.skin.GetStyle("number"));
			}

			if(PlayerPrefs.GetInt("Players") == 2)
			{
				skin.label.fontSize = 15;

				Matrix4x4 matrixBackup = GUI.matrix;
				GUIUtility.RotateAroundPivot(90, new Vector2(page.width/2,page.height/2));
				//Note: Parameters below represent the screen while rotated
				GUI.Label (new Rect (page.height/2 - 60, page.width - 250, 100, 40),"Player 1", GUI.skin.GetStyle("Label"));
				GUI.Label (new Rect (page.height/2 - 35, page.width - 225, 65, 40),player1Score.ToString(), GUI.skin.GetStyle("number"));
				GUI.matrix = matrixBackup;

				GUIUtility.RotateAroundPivot(-90, new Vector2(page.width/2,page.height/2));
				//Note: Parameters below represent the screen while rotated
				GUI.Label (new Rect ( page.height/2 - 60, page.width/2 + 170,100, 40),"Player 2", GUI.skin.GetStyle("Label"));
				GUI.Label (new Rect ( page.height/2 - 35, page.width/2 + 200,65, 40),player2Score.ToString(), GUI.skin.GetStyle("number"));
				
				GUI.matrix = matrixBackup;
			}
			else
			{
				GUI.Label (new Rect (30, 30, 120, 40), "Player 1");
			//	GUI.Label (new Rect (Screen.currentResolution.width - 130, 30, 120, 40), "Player 2");
				
				GUI.Label (new Rect (40, 60, 100, 40), player1Score.ToString(), GUI.skin.GetStyle("number"));
			}

			if(tickerRunning)
			{
				tickerPosition -= 2;
				newsTicker();
				if(tickerPosition < (0 - tickerWidth))
				{
					tickerRunning = false;
				}
			}

		}

	}

	void updateGameTime(float newTime)
	{
		gameTimeLeft = newTime;
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
	}

	void triggerVictory()
	{
		victory = true;
	}

	void triggerMultiPlayerVictory()
	{
		victory = true;
	}
	
	void triggerSinglePlayerVictory(int highScore)
	{
		this.highScore = highScore;
		victory = true;
		singlePlayer = true;
	}
	
	void runTicker(string[] args)
	{
		if(args.Length < 2)
		{
			//Debug.LogError("Received ticker message with incorrect args, ignoring.");
			return;
		}
		tickerRunning = true;

		tickerPosition = page.width - 20;
		tickerWidth = int.Parse(args[0]);
		tickerMsg = args[1];
	}

	void newsTicker()
	{
		if(PlayerPrefs.GetInt("Players") == 2)
		{
			Matrix4x4 matrixBackup = GUI.matrix;
			GUIUtility.RotateAroundPivot(90, new Vector2(page.width/2,page.height/2));
			//Note: Parameters below represent the screen while rotated
			GUI.Box(new Rect(tickerPosition, page.width-280, tickerWidth, 35), tickerMsg, skin.box);

			GUI.matrix = matrixBackup;
			
			GUIUtility.RotateAroundPivot(-90, new Vector2(page.width/2, page.height/2));
			//Note: Parameters below represent the screen while rotated
			GUI.Box(new Rect(tickerPosition, page.width-280, tickerWidth, 35), tickerMsg, skin.box);

			
			GUI.matrix = matrixBackup;
		}
		else
		{
			GUI.Box(new Rect(tickerPosition, page.height-50, tickerWidth, 35), tickerMsg, skin.box);
		}
	}

	public void pauseGame(bool stopTime)
	{
		if(player1 == null)
		{
			player1 = GameObject.Find("Player 1");
		}
		
		if(player2 == null && !singlePlayer)
		{
			player2 = GameObject.Find("Player 2");
		}
		
		if(player1 != null)
		{
			player1.GetComponent<Player1>().LockControls();
		}
		
		if(player2 != null)
		{
			player2.GetComponent<Player2>().LockControls();
		}
		
		if(aiCars == null)
		{
			aiCars = GameObject.FindGameObjectsWithTag("aiCar");
		}
		
		foreach(GameObject car in aiCars)
		{
			car.GetComponent<AI>().StopAIVehicle();
		}
		if (stopTime) 
		{
			Time.timeScale = 0;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TV_HUD>().pauseGame();
		}

		paused = true;

	}

	public void unpauseGame(bool startTime)
	{
		if(player1 != null)
		{
			player1.GetComponent<Player1>().UnlockControls();
		}
		
		if(player2 != null)
		{
			player2.GetComponent<Player2>().UnlockControls();
		}
		
		foreach(GameObject car in aiCars)
		{
			car.GetComponent<AI>().StartAIVehicle();
		}

		paused = false;

		if (startTime) 
		{
			Time.timeScale = 1;
			GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<TV_HUD> ().unpauseGame ();
		}
	}
}
