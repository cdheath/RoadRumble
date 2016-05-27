using UnityEngine;
using System.Collections;

public class Gamepad_HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture popup_background;
	float gameTimeLeft = -1;
	int player1Score = 0;
	int player2Score = 0;
	int tickerPosition;
	int tickerWidth;
	string tickerMsg;
	bool victory = false;
	bool tickerRunning = false;
	bool paused = false;
	int[] highScores;
	bool singlePlayer = false;
	protected Vector2 touchPosition = new Vector2(-1f,-1f);	// x,y coords on gamePad for touch
	GameObject[] aiCars = null;
	[HideInInspector]
	public GameObject player1 = null;
	[HideInInspector]
	public GameObject player2 = null;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		touchPosition = (Input.touchCount > 0) ? Input.touches[0].position : new Vector2(-1f,-1f);

//		Debug.LogWarning("touch is: " + touchPosition.ToString());
		if(victory && touchPosition.x > -1)
		{
			Application.LoadLevel("MainMenu");
		}
	
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
			if(singlePlayer)
			{
				winnerMsg = "Level High Scores";
				GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);
				GUI.Label(new Rect(0,100, Screen.width, 50),winnerMsg);

				rightSideMsg = "Top Score";
				rightSideScore = GetTopScoreString();
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
			
			GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);
			GUI.Label(new Rect(0,100, Screen.width, 50),winnerMsg);
			
			GUI.Label(new Rect(0,150, Screen.width/2, 50),"Player 1");
			GUI.Label(new Rect(0,200, Screen.width/2, 50),player1Score.ToString(), GUI.skin.GetStyle("number"));

			GUI.Label(new Rect(Screen.width/2,150, Screen.width/2, 50),rightSideMsg);
			GUI.Label(new Rect(Screen.width/2,200, Screen.width/2, 50),rightSideScore, GUI.skin.GetStyle("number"));
			
			GUI.Label(new Rect(0,300, Screen.width, 50),"Tap to continue...");

			if (GUI.Button(new Rect(0,0,Screen.width, Screen.height),""))
			{
				Application.LoadLevel("MainMenu");
			}
		}
		else if(paused)
		{
			skin.button.fontSize = 40;
			GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);
			
			if(GUI.Button(new Rect(Screen.width/4 - 100, Screen.height/4, 500, 50),"Resume"))
			{
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TV_HUD>().pauseGame();
				pauseGame();
			}
			
			if (GUI.Button(new Rect(Screen.width/4 - 100 ,Screen.height/4 + 100, 500, 50),"Return to Main Menu"))
			{
				Application.LoadLevel("MainMenu");
			}

		}
		else
		{
			if(gameTimeLeft != -1)
			{
				GUI.Label (new Rect (Screen.width/2, 30, 50, 40), ((int)gameTimeLeft).ToString(), GUI.skin.GetStyle("number"));
			}

			if(PlayerPrefs.GetInt("Players") == 2)
			{
				skin.label.fontSize = 15;

				Matrix4x4 matrixBackup = GUI.matrix;
				GUIUtility.RotateAroundPivot(90, new Vector2(Screen.width/2,Screen.height/2));
				//Note: Parameters below represent the screen while rotated
				GUI.Label (new Rect (Screen.height/2 - 60, Screen.width - 250, 100, 40),"Player 1", GUI.skin.GetStyle("label"));
				GUI.Label (new Rect (Screen.height/2 - 35, Screen.width - 225, 65, 40),player1Score.ToString(), GUI.skin.GetStyle("number"));
				GUI.matrix = matrixBackup;

				GUIUtility.RotateAroundPivot(-90, new Vector2(Screen.width/2,Screen.height/2));
				//Note: Parameters below represent the screen while rotated
				GUI.Label (new Rect ( Screen.width/2 - 245, Screen.width/2 + 175,100, 40),"Player 2", GUI.skin.GetStyle("label"));
				GUI.Label (new Rect ( Screen.width/2 - 220, Screen.width/2 + 200,65, 40),player2Score.ToString(), GUI.skin.GetStyle("number"));
				
				GUI.matrix = matrixBackup;
			}
			else
			{
				GUI.Label (new Rect (30, 30, 120, 40), "Player 1");
			//	GUI.Label (new Rect (Screen.width - 130, 30, 120, 40), "Player 2");
				
				GUI.Label (new Rect (40, 60, 100, 40), player1Score.ToString(), GUI.skin.GetStyle("number"));
			}

			if(tickerRunning)
			{
				tickerPosition -= 1;
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
	
	void triggerSinglePlayerVictory(int[] highScores)
	{
		this.highScores = highScores;
		victory = true;
		singlePlayer = true;
	}
	
	void runTicker(string[] args)
	{
		if(args.Length < 2)
		{
			Debug.LogError("Received ticker message with incorrect args, ignoring.");
			return;
		}
		tickerRunning = true;

		tickerPosition = Screen.width;
		tickerWidth = int.Parse(args[0]);
		tickerMsg = args[1];
	}

	void newsTicker()
	{
		if(PlayerPrefs.GetInt("Players") == 2)
		{
			Matrix4x4 matrixBackup = GUI.matrix;
			GUIUtility.RotateAroundPivot(90, new Vector2(Screen.width/2,Screen.height/2));
			//Note: Parameters below represent the screen while rotated
			GUI.Box(new Rect(tickerPosition, Screen.width-280, tickerWidth, 35), tickerMsg, skin.box);

			GUI.matrix = matrixBackup;
			
			GUIUtility.RotateAroundPivot(-90, new Vector2(Screen.width/2,Screen.height/2));
			//Note: Parameters below represent the screen while rotated
			GUI.Box(new Rect(tickerPosition, Screen.width-280, tickerWidth, 35), tickerMsg, skin.box);

			
			GUI.matrix = matrixBackup;
		}
		else
		{
			GUI.Box(new Rect(tickerPosition, Screen.height-50, tickerWidth, 35), tickerMsg, skin.box);
		}
	}

	public void pauseGame()
	{
		if(paused)
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
			Time.timeScale = 1;
			paused = false;
		}
		else
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
			Time.timeScale = 0;
			paused = true;
		}
	}

	string GetTopScoreString()
	{
		string highScoreString = "";
		if (this.highScores[0] > 0) 
		{
			highScoreString = highScores[0].ToString();
		}
		return highScoreString;
	}

}
