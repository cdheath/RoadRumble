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

			GUISkin newSkin = skin;
			newSkin.label.fontSize = 80;
			newSkin.GetStyle("number").fontSize = 80;
			GUI.skin = newSkin;
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
			Rect menuRect = new Rect(Screen.width/4,Screen.height/4,Screen.width/2-20,Screen.height/2-100);
			GUI.DrawTexture(menuRect, popup_background);

			if(GUI.Button(new Rect(menuRect.xMin + menuRect.width/4, menuRect.yMin + menuRect.height/5, 500, 50),"Resume"))
			{
//				GameObject.FindGameObjectWithTag("GamePadCamera").GetComponent<Gamepad_HUD>().pauseGame();
//				pauseGame();
			}
			
			if (GUI.Button(new Rect(menuRect.xMin + menuRect.width/4, menuRect.yMin + menuRect.height * 2/5, 500, 50),"Return to Main Menu"))
			{
//				Application.LoadLevel("MainMenu");
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
	//	//Debug.Log(newTime);
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
			//Debug.LogError("Received ticker message with incorrect args, ignoring.");
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
			Time.timeScale = 1;
			paused = false;
		}
		else
		{
			Time.timeScale = 0;
			paused = true;
		}
	}
}
