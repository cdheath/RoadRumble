using UnityEngine;
using System.Collections;

public class TV_HUD : MonoBehaviour {

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

	int[] highScores;
	bool singlePlayer = false;
	bool paused = false;

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
	
	}
	[GuiTarget(GuiTarget.Target.TV)]
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
		}
		else if(paused)
		{
			skin.button.fontSize = 40;
			GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);

			if(GUI.Button(new Rect(Screen.width/4 - 100, Screen.height/4, 500, 50),"Resume"))
			{
				GameObject.FindGameObjectWithTag("GamePadCamera").GetComponent<Gamepad_HUD>().pauseGame();
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
			
			GUI.Label (new Rect (30, 30, 120, 40), "Player 1");

			
			GUI.Label (new Rect (40, 60, 100, 40), player1Score.ToString(), GUI.skin.GetStyle("number"));
			if(PlayerPrefs.GetInt("Players") == 2)
			{
				GUI.Label (new Rect (Screen.width - 130, 30, 120, 40), "Player 2");
				GUI.Label (new Rect (Screen.width - 80, 60, 100, 40), player2Score.ToString(), GUI.skin.GetStyle("number"));
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
	//	Debug.Log(newTime);
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
		
		tickerPosition = Screen.width;//int.Parse(args[0]);
		tickerWidth = int.Parse(args[0]);
		tickerMsg = args[1];
	}
	
	void newsTicker()
	{
		GUI.Box(new Rect(tickerPosition, Screen.height-50, tickerWidth, 35), tickerMsg, skin.box);
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

	public void pauseGame()
	{
		if(paused)
		{
			if(player1 != null)
			{
				player1.GetComponent<Player1>().ToggleControlLock();
			}
			
			if(player2 != null)
			{
				player2.GetComponent<Player2>().ToggleControlLock();
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
				player1.GetComponent<Player1>().ToggleControlLock();
			}

			if(player2 != null)
			{
				player2.GetComponent<Player2>().ToggleControlLock();
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
}
