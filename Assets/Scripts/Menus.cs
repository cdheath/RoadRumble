using UnityEngine;
using System.Collections;

public class Menus : MonoBehaviour {

	public Texture screenToShow;
	public Texture back;
	public Texture Instructional_Diagram;
	public Texture popup_background;
	public Texture carImg;
	public Texture[] levels;

	int carPos;

	Rect page;
	public GUISkin skin;
	GameObject toChange;

//	enum menu {gameModes,twoPlayer,main,options,highScores, title, levels, credits, victory, instructions, loading, intro, limits};
	menu currentMenu = menu.title;

	int[] maxMins = {1,2,3,4,5,6};
	int[] maxScores = {500,1000,1500,2000,2500,3000};

	string gameMode = "Timed";
	int numPlayers = 1;
	int maxScore = 500;
	int maxTime = 1;
	string levelChoice ="1";
	bool nightOn = false;
	Texture levelLoad;
	Menus_TV tvMenu;

	private bool pressedButtonB = false;

	// Use this for initialization
	void Start () {
		//hard coded to gamepad res
		page.width = 854;
		page.height = 480;
		page.x = 0;
		page.y = 0;

		carPos = 0;
		//Debug.Log ("GamePad Page Width: " + Screen.currentResolution.width + " Page Height: " + Screen.currentResolution.height);
		tvMenu = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Menus_TV> ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	//constantly being called
	[GuiTarget(GuiTarget.Target.GamePad)]
	void OnGUI()
	{ 
		//GUI.DrawTexture (page, screenToShow);

		WiiUGamePad gamePad = WiiUInput.GetGamePad ();

		pressedButtonB = gamePad.GetButton(WiiUGamePadButton.ButtonB);
		//touchPosition = (Input.touchCount > 0) ? Input.touches[0].position : new Vector2(-1f,-1f);

		if(skin != null)
		{
			GUI.skin = skin;
		}

		switch(currentMenu)
		{
			case menu.title:
				//skin.button.fontSize = 120;
				//skin.button.fontSize = 60;
			if (GUI.Button(new Rect(0,0, page.width, page.height*1/2), "Retro Road\nRumble", GUI.skin.GetStyle("Title_GamePad")))
				{
				//	skin.button.fontSize = 65;
					currentMenu = menu.main;
					tvMenu.ChangeMenu(menu.main);
					break;
				}
				//skin.button.fontSize = 50;
			if (GUI.Button(new Rect(0,(float)(page.height * 2/5), page.width, page.height*3/8), "Tap to Continue", GUI.skin.GetStyle("TapContinue_GamePad"))) 
				{
				//	skin.button.fontSize = 65;
					currentMenu = menu.main;
					tvMenu.ChangeMenu(menu.main);
				}
			break;
			case menu.main:
				//toggleMain(true);
			if (GUI.Button(new Rect(0, page.height * 1/8, page.width, page.height * 1/6), "1 Player")) 
				{
					numPlayers = 1;
					gameMode="Timed";
					currentMenu = menu.limits;
					tvMenu.ChangeMenu(menu.limits);
				}
			if (GUI.Button(new Rect(0, page.height * 2/8, page.width, page.height * 1/6), "2 Player")) 
				{
					numPlayers = 2;
					currentMenu = menu.gameModes;
					tvMenu.ChangeMenu(menu.gameModes);
				}
			if (GUI.Button(new Rect(0, page.height * 3/8, page.width, page.height * 1/6), "Instructions")) 
				{
					currentMenu = menu.instructions;
					tvMenu.ChangeMenu(menu.instructions);
				}
			if (GUI.Button(new Rect(0, page.height * 4/8, page.width, page.height * 1/6), "Credits")) 
				{
					currentMenu = menu.credits;
					tvMenu.ChangeMenu(menu.credits);
				}
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.title;
					tvMenu.ChangeMenu(menu.title);
					toggleTitle(true);
				}
				break;
			case menu.highScores:
				break;
			case menu.gameModes:
			if (GUI.Button(new Rect(0, page.height * 1/8, page.width, page.height * 1/6), "Score")) 
				{
					gameMode="Score";
					currentMenu = menu.limits;
					tvMenu.ChangeMenu(menu.limits);
				}
			if (GUI.Button(new Rect(0, page.height * 2/8, page.width, page.height * 1/6), "Timed")) 
				{
					gameMode="Timed";
					currentMenu = menu.limits;
					tvMenu.ChangeMenu(menu.limits);
				}
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
					tvMenu.ChangeMenu(menu.main);
				}
				break;
			case menu.limits:
				toggleVictoryConditions();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					if(numPlayers == 2)
					{
						currentMenu = menu.gameModes;
						tvMenu.ChangeMenu(menu.gameModes);
					}
					else
					{
						currentMenu=menu.main;
						tvMenu.ChangeMenu(menu.main);
					}
				}
				break;
			case menu.levels:
				toggleLevels();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.limits;
					tvMenu.ChangeMenu(menu.limits);
				}
				break;
			case menu.loading:
				GUI.DrawTexture(new Rect(page.width/7, 100, 550,300),levelLoad);
				GUI.Label(new Rect(100,50, 200, 50), "Loading...");
				break;
			case menu.instructions:
				toggleInstructions();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
					tvMenu.ChangeMenu(menu.main);
				}
				break;
			case menu.credits:
				toggleCredits();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
					tvMenu.ChangeMenu(menu.main);
				}
				break;
			default:
				break;
		}

		if (currentMenu != menu.levels && currentMenu != menu.instructions && currentMenu != menu.credits && currentMenu != menu.none && currentMenu != menu.loading) 
		{
			if (carPos > page.width + 250) {
					carPos = -250;
			}
			if (carPos % 10 == 0) {
					GUI.DrawTexture (new Rect (carPos, page.height * 1 / 3 + 227, 100, 50), carImg);
			} else {
					GUI.DrawTexture (new Rect (carPos, page.height * 1 / 3 + 225, 100, 50), carImg);
			}
			carPos += 2;
		}
	}


	IEnumerator toggleTitle(bool visible)
	{
		tvMenu.toggleTitle (visible);
		toChange = GameObject.Find("Title");
		toChange.GetComponent<GUIText>().enabled = visible;
		toChange = GameObject.Find("Continue");
		toChange.GetComponent<GUIText>().enabled = visible;
		yield return new WaitForSeconds(1);
	}

	void toggleVictoryConditions()
	{
		int[] limits;
		string units;

		if(gameMode == "Timed")
		{
			limits = maxMins;
			units = "Minutes";
		}
		else
		{
			limits = maxScores;
			units = "Points";
		}

		tvMenu.sendVictoryConditions (gameMode, maxMins, maxScores);
		tvMenu.ChangeMenu(menu.limits);

		GUI.Label(new Rect(0, page.height * 1/8, page.width, 125), "Select Limit", skin.GetStyle("Button"));

		int y = (int)((page.height/limits.Length)/2 + (page.height * 1/8));
		int x = 0;
		//GUIStyle limitButtonStyle = new GUIStyle ("button");
		//limitButtonStyle.fontSize = 40;
		bool leftSide = true;
		foreach(int opt in limits)
		{

			string styleType = leftSide ? "LimitButtons_GamePad" : "LimitButtons_GamePad_Right";
			if (GUI.Button(new Rect(x,y, (page.width * 0.4f), page.height * 1/6), opt.ToString() + " " + units, skin.GetStyle(styleType))) 
			{
				//skin.button.fontSize = 30;
				if(gameMode == "Timed")
				{
					maxTime = opt;
				}
				else
				{
					maxScore = opt;
				}
				currentMenu = menu.levels;
			}
			if(x == 0)
			{
				x = (int)(page.width * 0.6f);
				leftSide = false;
			}
			else
			{
				y+=(int)(page.height/limits.Length)/2;
				x = 0;
				leftSide = true;
			}
		}
	}

	void toggleLevels()
	{
		currentMenu = menu.levels;
		tvMenu.ChangeMenu(menu.levels);
		//tvMenu.toggleLevels();

		int x = -150;
		int y = 75;
		nightOn = GUI.Toggle(new Rect(page.width * 1/2 + 300,20, 40, 50),nightOn, "Night", skin.GetStyle("Toggle"));
		tvMenu.nightOn = nightOn;
		GUI.Label(new Rect(page.width * 1/2 + 200,30, 75, 75), "Time:", skin.GetStyle("Label"));

		//skin.label.fontSize = 18;

		foreach(Texture level in levels)
		{
			x+=200;
			if(x > (page.width - 150))
			{
				x = 50;
				y += 175;
			}
			//GUI.DrawTexture (new Rect (x, y, 100, 100), level);
			if (GUI.Button(new Rect(x,y, 175, 175), level)) 
			{
				levelChoice = level.name;
				levelLoad = level;
				currentMenu = menu.loading;
				tvMenu.ChangeMenu(menu.loading, levelLoad);
				loadGame();
			}
			GUI.Label(new Rect(x,y+125, 175, 125), level.name, skin.GetStyle("LevelLabels_GamePad"));
		}
		//skin.label.fontSize = 20;

	}

	void toggleCredits()
	{
		//tvMenu.toggleCredits();

		float half = page.width/2;
//		float third = page.width/3; 

		//skin.label.fontSize = 35;
		GUI.Label(new Rect(0,50, page.width, page.width * 1/6), "Created by:",skin.GetStyle("CreditHeaders"));
		//GUI.Label(new Rect(0,150, page.width, 50), "Models by:");
		GUI.Label(new Rect(0,page.width * 0.17f, page.width, page.width * 1/6), "Music:",skin.GetStyle("CreditHeaders"));

		//skin.label.fontSize = 20;
	
		GUI.Label(new Rect(0,page.width * 1/8, half, 50), "Corey Heath", skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect((int)(page.width * 0.5f),page.width * 1/8, half, 50), "Tracey Heath", skin.GetStyle("CreditLabels"));

		//GUI.Label(new Rect(0,200, third, 50), "dude 1");
		//GUI.Label(new Rect(third,200, third, 50), "dude 2");
		//GUI.Label(new Rect(third*2,200, third, 50), "Tracey Heath");

		GUI.Label(new Rect(0,200, page.width, 75), "A Breeze From Alabama - Scott Joplin (1902)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,225, page.width, 75), "Chicken Reel - Joseph M Daly (1910)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,250, page.width, 75), "Fig Leaf Rag - Scott Joplin (1908)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,275, page.width, 75), "Hallowe'en - Arthur Manlowe (1911)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,300, page.width, 75), "Heliotrope Bouquet - Scott Joplin & Louis Chauvin (1907)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,325, page.width, 75), "Maple Leaf Rag - Scott Joplin (1899)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,350, page.width, 75), "The St. Louis Rag - Thoms Million Turpin (1903)".PadRight(56), skin.GetStyle("CreditLabels"));
		GUI.Label(new Rect(0,375, page.width, 75), "Whistling Rufus - Frederick Allen Mills (1899)".PadRight(56), skin.GetStyle("CreditLabels"));
	}

	void toggleInstructions()
	{
		//tvMenu.toggleInstructions();
		/*GUI.Label(new Rect(25,50, page.width-25, 125), "Get your jalopy to the green drop off point, on the double." + 
		          "You see anyone else on the make, be sure they don't get there before you." +
		          "Mind you don't conk into anything or your car won't be worth a plugged nickel before long.");

		GUI.Label(new Rect(25,175, page.width-25, 100),"Now here's a real humdinger." + 
		          "Get a wiggle on that gamepad to really razz those rubes, but mind it doesn't grift you outta the running too.");
		GUI.Label(new Rect(25,250, page.width-25, 50),"Now you're on the trolley!");*/

		//GUI.DrawTexture(new Rect(page.width/2 - 275, 200, 550,275),Instructional_Diagram);
		GUI.DrawTexture(new Rect(40, 65, page.width - 80, page.height - 75),Instructional_Diagram);
		GUI.Label(new Rect(0,25, page.width, 50),"Drive your car to the green marker to score points.");
	}

	void loadGame()
	{
		//Debug.Log ("level is: " + levelChoice + " mode is: " + gameMode + " players = " + numPlayers);
		//input scene load here
		PlayerPrefs.SetString ("Mode", gameMode);
		PlayerPrefs.SetInt ("Players", numPlayers);
		PlayerPrefs.SetInt ("TimeLimit", maxTime);
		PlayerPrefs.SetInt ("ScoreLimit", maxScore);
		PlayerPrefs.SetString("Night", nightOn.ToString());
		//Application.LoadLevel(levelChoice);
		Application.LoadLevelAsync (levelChoice);
	}

}
