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
	protected Vector2 touchPosition = new Vector2(-1f,-1f);	// x,y coords on gamePad for touch

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		touchPosition = (Input.touchCount > 0) ? Input.touches[0].position : new Vector2(-1f,-1f);

		Debug.LogWarning("touch is: " + touchPosition.ToString());
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
			
			GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);
			GUI.Label(new Rect(0,100, Screen.width, 50),winnerMsg);
			
			GUI.Label(new Rect(0,150, Screen.width/2, 50),"Player 1");
			GUI.Label(new Rect(0,200, Screen.width/2, 50),player1Score.ToString(), GUI.skin.GetStyle("number"));

			if(PlayerPrefs.GetInt("Players") == 2)
			{
				GUI.Label(new Rect(Screen.width/2,150, Screen.width/2, 50),"Player 2");
				GUI.Label(new Rect(Screen.width/2,200, Screen.width/2, 50),player2Score.ToString(), GUI.skin.GetStyle("number"));
			}
			
			GUI.Label(new Rect(0,300, Screen.width, 50),"Tap to continue...");
		}
		else if(paused)
		{
			
			GUI.DrawTexture(new Rect(10,50,Screen.width-20,Screen.height-100), popup_background);
			
			if(GUI.Button(new Rect(Screen.width/2 - 500, Screen.height/2-200, 500, 50),"Resume"))
			{
				paused = false;
			}
			
			if (GUI.Button(new Rect(Screen.width/2 - 500 ,Screen.height/2+200, 500, 50),"Return to level select"))
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
				GUI.Label (new Rect (Screen.width - 130, 30, 120, 40), "Player 2");
				
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

	void pauseGame()
	{
		if(paused)
		{
			paused = false;
		}
		else
		{
			paused = true;
		}
	}

}
