using UnityEngine;
using System.Collections;

public class TV_HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture popup_background;
	float gameTimeLeft = -1;
	int player1Score = 0;
	int player2Score = 0;
	Rect page;
	float tickerPosition;
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
		page.width = 1920;
		page.height = 1080;
		page.x = 0;
		page.y = 0;
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
			//	GUI.DrawTexture(new Rect(10,50,page.width-20,page.height-100), popup_background);
			//	GUI.Label(new Rect(0,100, page.width, 50),winnerMsg);

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

			Rect menuRect = new Rect(page.width * 1/6,page.height * 1/6,page.width * 0.7f,page.height * 0.7f);
			GUI.DrawTexture(menuRect, popup_background);
			GUI.Label(new Rect(menuRect.xMin,menuRect.yMin + menuRect.height/6, menuRect.width, 50),winnerMsg, GUI.skin.GetStyle("ScoreLabels_TV"));
			
			GUI.Label(new Rect(menuRect.xMin,menuRect.yMin + menuRect.height/4, menuRect.width/2, 50),"Player 1", GUI.skin.GetStyle("ScoreLabels_TV"));
			GUI.Label(new Rect(menuRect.xMin,menuRect.yMin + menuRect.height* 1/3, menuRect.width/2, 50),player1Score.ToString(), GUI.skin.GetStyle("number_TV"));
	
			GUI.Label(new Rect(menuRect.xMin + menuRect.width/2,menuRect.yMin + menuRect.height/4, menuRect.width/2, 50),rightSideMsg, GUI.skin.GetStyle("ScoreLabels_TV"));
			GUI.Label(new Rect(menuRect.xMin + menuRect.width/2,menuRect.yMin + menuRect.height* 1/3, menuRect.width/2, 50),rightSideScore, GUI.skin.GetStyle("number_TV"));

			GUI.Label(new Rect(menuRect.xMin,menuRect.height* 3/4, menuRect.width, 50),"Tap to continue...", GUI.skin.GetStyle("ScoreLabels_TV"));
		}
		else if(paused)
		{
			skin.button.fontSize = 40;
			Rect menuRect = new Rect(page.width/4,page.height/4,page.width/2-20,page.height/2-100);
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
				GUI.Label (new Rect (page.width/2 - 50, 30, 100, 40), ((int)gameTimeLeft).ToString(), GUI.skin.GetStyle("number_TV"));
			}
			
			GUI.Label (new Rect (30, 30, 250, 40), "Player 1", GUI.skin.GetStyle("ScoreLabels_TV"));

			
			GUI.Label (new Rect (40, 100, 200, 40), player1Score.ToString(), GUI.skin.GetStyle("number_TV"));
			if(PlayerPrefs.GetInt("Players") == 2)
			{
				GUI.Label (new Rect (page.width - page.width * 0.15f, 30, 250, 40), "Player 2", GUI.skin.GetStyle("ScoreLabels_TV"));
				GUI.Label (new Rect (page.width - page.width/7, 100, 200, 40), player2Score.ToString(), GUI.skin.GetStyle("number_TV"));
			}
			
			if(tickerRunning)
			{
				tickerPosition -= 4;
				newsTicker();
				if(tickerPosition < (0 - (tickerWidth * 2.5f)))
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
		
		tickerPosition = page.width - 20;//int.Parse(args[0]);
		tickerWidth = int.Parse(args[0]);
		tickerMsg = args[1];
	}
	
	void newsTicker()
	{
		//Debug.Log (tickerWidth);
		GUI.Box(new Rect(tickerPosition, page.height - page.height/6, tickerWidth * 2.5f, 80), tickerMsg, GUI.skin.GetStyle("Ticker_TV"));
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
