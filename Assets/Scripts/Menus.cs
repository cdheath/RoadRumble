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

	enum menu {gameModes,twoPlayer,main,options,highScores, title, levels, credits, victory, instructions, loading, intro, limits};
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

	private bool pressedButtonB = false;

	// Use this for initialization
	void Start () {
		page.width = Screen.width;
		page.height = Screen.height;
		page.x = 0;
		page.y = 0;

		carPos = 0;

	}
	
	// Update is called once per frame
	void Update () {

	}

	//constantly being called
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
				skin.button.fontSize = 125;
				if (GUI.Button(new Rect(0,0, page.width, page.height*3/4), "Retro Road\nRumble"))
				{
					skin.button.fontSize = 65;
					currentMenu = menu.main;
					break;
				}
				skin.button.fontSize = 50;
				if (GUI.Button(new Rect(0,page.height*3/4, page.width, page.height*1/4), "Tap to Continue")) 
				{
					skin.button.fontSize = 65;
					currentMenu = menu.main;
				}
			break;
			case menu.main:
				//toggleMain(true);
			if (GUI.Button(new Rect(0, page.height * 1/5 - 50, page.width, page.height * 1/5), "1 Player")) 
				{
					numPlayers = 1;
					gameMode="Timed";
					currentMenu = menu.limits;
				}
			if (GUI.Button(new Rect(0, page.height * 2/5 - 50, page.width, page.height * 1/5), "2 Player")) 
				{
					numPlayers = 2;
					currentMenu = menu.gameModes;
				}
			if (GUI.Button(new Rect(0, page.height * 3/5 - 50, page.width, page.height * 1/5), "Instructions")) 
				{
					currentMenu = menu.instructions;
				}
			if (GUI.Button(new Rect(0, page.height * 4/5 - 50, page.width, page.height * 1/5), "Credits")) 
				{
					currentMenu = menu.credits;
				}
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.title;
					toggleTitle(true);
				}
				break;
			case menu.highScores:
				break;
			case menu.gameModes:
				if (GUI.Button(new Rect(0,100, page.width, 100), "Score")) 
				{
					gameMode="Score";
					currentMenu = menu.limits;
				}
				if (GUI.Button(new Rect(0,300, page.width, 100), "Timed")) 
				{
					gameMode="Timed";
					currentMenu = menu.limits;
				}
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
				}
				break;
			case menu.limits:
				toggleVictoryConditions();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					if(numPlayers == 2)
					{
						currentMenu = menu.gameModes;
					}
					else
					{
						currentMenu=menu.main;
					}
				}
				break;
			case menu.levels:
				toggleLevels();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.limits;
				}
				break;
			case menu.loading:
				GUI.DrawTexture(new Rect(page.width/2-375, 100, 750,400),levelLoad);
				GUI.Label(new Rect(100,50, 200, 50), "Loading...");
				break;
			case menu.instructions:
				toggleInstructions();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
				}
				break;
			case menu.credits:
				toggleCredits();
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
				}
				break;
			default:
				break;
		}

		if(carPos > page.width + 250)
		{
			carPos = -250;
		}
		if(carPos % 10 == 0)
		{
			GUI.DrawTexture(new Rect(carPos,page.height - 52, 100, 50), carImg);
		}
		else
		{
			GUI.DrawTexture(new Rect(carPos,page.height - 50, 100, 50), carImg);
		}
		carPos+=2;
	}


	IEnumerator toggleTitle(bool visible)
	{
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
		
		GUI.Label(new Rect(page.width/2 - 250,25, 500, 125), "Select Limit", skin.GetStyle("Button"));

		int y = (int)(page.height/limits.Length)/2 + 75;
		int x = 100;
		foreach(int opt in limits)
		{
			if (GUI.Button(new Rect(x,y, page.width/2 - 100, 175), opt.ToString() + " " + units)) 
			{
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
			if(x == 100)
			{
				x = (int)page.width/2 + 50;
			}
			else
			{
				y+=(int)(page.height/limits.Length)/2 + 100;
				x = 100;
			}
		}
	}

	void toggleLevels()
	{
		currentMenu = menu.levels;
		int x = -150;
		int y = 75;
		nightOn = GUI.Toggle(new Rect(page.width-100,20, 40, 50),nightOn, "Night", skin.GetStyle("Toggle"));
		GUI.Label(new Rect(page.width-170,30, 75, 75), "Time:", skin.GetStyle("Label"));

		skin.label.fontSize = 18;

		foreach(Texture level in levels)
		{
			x+=200;
			if(x > (Screen.width - 150))
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
				loadGame();
			}
			GUI.Label(new Rect(x,y+125, 175, 125), level.name, skin.GetStyle("Label"));
		}
		skin.label.fontSize = 20;

	}

	void toggleCredits()
	{
		float half = page.width/2;
		float third = page.width/3; 

		skin.label.fontSize = 35;
		GUI.Label(new Rect(0,50, page.width, 50), "Created by:");
		//GUI.Label(new Rect(0,150, page.width, 50), "Models by:");
		GUI.Label(new Rect(0,150, page.width, 50), "Music:");

		skin.label.fontSize = 20;
	
		GUI.Label(new Rect(0,100, half, 50), "Corey Heath");
		GUI.Label(new Rect(half,100, half, 50), "Tracey Heath");

		//GUI.Label(new Rect(0,200, third, 50), "dude 1");
		//GUI.Label(new Rect(third,200, third, 50), "dude 2");
		//GUI.Label(new Rect(third*2,200, third, 50), "Tracey Heath");

		GUI.Label(new Rect(0,200, half, 75), "A Breeze From Alabama - Scott Joplin (1902)");
		GUI.Label(new Rect(half,200, half, 75), "Chicken Reel - Joseph M Daly (1910)");
		GUI.Label(new Rect(0,250, half, 75), "Fig Leaf Rag - Scott Joplin (1908)");
		GUI.Label(new Rect(half,250, half, 75), "Hallowe'en - Arthur Manlowe (1911)");
		GUI.Label(new Rect(0,300, half, 75), "Heliotrope Bouquet - Scott Joplin & Louis Chauvin (1907)");
		GUI.Label(new Rect(half,300, half, 75), "Maple Leaf Rag - Scott Joplin (1899)");
		GUI.Label(new Rect(0,350, half, 75), "The St. Louis Rag - Thoms Million Turpin (1903)");
		GUI.Label(new Rect(half,350, half, 75), "Whistling Rufus - Frederick Allen Mills (1899)");
	}

	void toggleInstructions()
	{
		/*GUI.Label(new Rect(25,50, page.width-25, 125), "Get your jalopy to the green drop off point, on the double." + 
		          "You see anyone else on the make, be sure they don't get there before you." +
		          "Mind you don't conk into anything or your car won't be worth a plugged nickel before long.");

		GUI.Label(new Rect(25,175, page.width-25, 100),"Now here's a real humdinger." + 
		          "Get a wiggle on that gamepad to really razz those rubes, but mind it doesn't grift you outta the running too.");
		GUI.Label(new Rect(25,250, page.width-25, 50),"Now you're on the trolley!");*/

		//GUI.DrawTexture(new Rect(page.width/2 - 275, 200, 550,275),Instructional_Diagram);
		GUI.DrawTexture(new Rect(page.width/2-475, 100, 950,475),Instructional_Diagram);
		GUI.Label(new Rect(25,50, page.width-25, 50),"Drive your car to the green marker to score points.");
	}

	void loadGame()
	{
		Debug.Log ("level is: " + levelChoice + " mode is: " + gameMode + " players = " + numPlayers);
		//input scene load here
		PlayerPrefs.SetString ("Mode", gameMode);
		PlayerPrefs.SetInt ("Players", numPlayers);
		PlayerPrefs.SetInt ("TimeLimit", maxTime);
		PlayerPrefs.SetInt ("ScoreLimit", maxScore);
		PlayerPrefs.SetString("Night", nightOn.ToString());
		Application.LoadLevel(levelChoice);
	}

}
