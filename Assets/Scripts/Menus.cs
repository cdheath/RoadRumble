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
	GUIStyle buttonStyle = new GUIStyle ();

	enum menu {gameModes,twoPlayer,main,options,highScores, title, levels, credits, victory, instructions};
	menu currentMenu = menu.title;

	string gameMode = "Timed";
	int numPlayers = 1;
	string levelChoice ="1";

	private bool pressedButtonB = false;
	//private Vector2 touchPosition = new Vector2(-1f,-1f);	// x,y coords on gamePad for touch

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
		GUI.DrawTexture (page, screenToShow);

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
					skin.button.fontSize = 75;
					currentMenu = menu.main;
					break;
				}
				skin.button.fontSize = 50;
				if (GUI.Button(new Rect(0,page.height*3/4, page.width, page.height*1/4), "Tap to Continue")) 
				{
					skin.button.fontSize = 75;
					currentMenu = menu.main;
				}
			break;
			case menu.main:
				//toggleMain(true);
				if (GUI.Button(new Rect(0, page.height * 1/5 - 50, page.width, 100), "1 Player")) 
				{
					numPlayers = 1;
					gameMode="Timed";
					currentMenu = menu.levels;
				}
				if (GUI.Button(new Rect(0, page.height * 2/5 - 50, page.width, 100), "2 Player")) 
				{
					numPlayers = 2;
					currentMenu = menu.gameModes;
				}
				if (GUI.Button(new Rect(0, page.height * 3/5 - 50, page.width, 100), "Instructions")) 
				{
					currentMenu = menu.instructions;
				}
				if (GUI.Button(new Rect(0, page.height * 4/5 - 50, page.width, 100), "Credits")) 
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
					currentMenu = menu.levels;
				}
				if (GUI.Button(new Rect(0,300, page.width, 100), "Timed")) 
				{
					gameMode="Timed";
					currentMenu = menu.levels;
				}
				if (GUI.Button(new Rect(5,5, 50, 50), back) || pressedButtonB) 
				{
					currentMenu = menu.main;
				}
				//toggleGameMode(true);
				break;
			case menu.levels:
				toggleLevels();
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

		//rect(x, y, width, height)
		//GUI.DrawTexture (page, screenToShow);
		//GUI.DrawTexture (new Rect (50, selectionIndex, 40, 50), selector);
		//drawMain ();
		//GUI.SelectionGrid (menuBox, 0, menuItems, 2);
	}


	IEnumerator toggleTitle(bool visible)
	{
		toChange = GameObject.Find("Title");
		toChange.GetComponent<GUIText>().enabled = visible;
		toChange = GameObject.Find("Continue");
		toChange.GetComponent<GUIText>().enabled = visible;
		yield return new WaitForSeconds(1);
	}

	void toggleLevels()
	{
		currentMenu = menu.levels;
		int x = -100;
		int y = 50;
		foreach(Texture level in levels)
		{
			x+=150;
			if(x > (Screen.width - 50))
			{
				x = 50;
				y += 200;
			}
			//GUI.DrawTexture (new Rect (x, y, 100, 100), level);
			if (GUI.Button(new Rect(x,y, 100, 100), level)) 
			{
				levelChoice = level.name;
				loadGame();
			}
		}
	}

	void toggleCredits()
	{
		//609 x 962
		//GUI.Box(new Rect(10,50,page.width-20,page.height-100), "");
		float half = page.width/2;
		float third = page.width/3; 

		skin.label.fontSize = 35;
		GUI.Label(new Rect(0,50, page.width, 50), "Concept, Design, and Scripts by:");
		GUI.Label(new Rect(0,150, page.width, 50), "Models by:");
		GUI.Label(new Rect(0,275, page.width, 50), "Music:");

		skin.label.fontSize = 20;
	
		GUI.Label(new Rect(0,100, half, 50), "Corey Heath");
		GUI.Label(new Rect(half,100, half, 50), "Tracey Heath");

		GUI.Label(new Rect(0,200, third, 50), "dude 1");
		GUI.Label(new Rect(third,200, third, 50), "dude 2");
		GUI.Label(new Rect(third*2,200, third, 50), "Tracey Heath");

		GUI.Label(new Rect(0,325, half, 75), "A Breeze From Alabama - Scott Joplin (1902)");
		GUI.Label(new Rect(half,325, half, 75), "Chicken Reel - Joseph M Daly (1910)");
		GUI.Label(new Rect(0,375, half, 75), "Fig Leaf Rag - Scott Joplin (1908)");
		GUI.Label(new Rect(half,375, half, 75), "Hallowe'en - Arthur Manlowe (1911)");
		GUI.Label(new Rect(0,425, half, 75), "Heliotrope Bouquet - Scott Joplin & Louis Chauvin (1907)");
		GUI.Label(new Rect(half,425, half, 75), "Maple Leaf Rag - Scott Joplin (1899)");
		GUI.Label(new Rect(0,475, half, 75), "The St. Louis Rag - Thoms Million Turpin (1903)");
		GUI.Label(new Rect(half,475, half, 75), "Whistling Rufus - Frederick Allen Mills (1899)");
	}

	void toggleInstructions()
	{	
		//GUI.Box(new Rect(10,50,page.width-20,page.height-100), "");
		GUI.DrawTexture (new Rect(5,20,page.width-10,page.height-20), popup_background);

		GUI.Label(new Rect(25,50, page.width-50, 150), "Get behind the wheel of your jalopy and take it to the green drop off point on the double." + 
		          "You see anyone else on the make and you be sure they don't get there before you." +
		          "And mind you don't conk into anything too much or your car won't be worth a plugged nickel before long.");

		GUI.Label(new Rect(25,175, page.width-50, 100),"Now here's the real humdinger." + 
		          "Get a wiggle on that gamepad to really razz those rubes, but mind it doesn't grift you outta the running too.");
		GUI.Label(new Rect(25,250, page.width-50, 50),"Now you're on the trolley!");

		GUI.DrawTexture(new Rect(page.width/2 - 275, 300, 550,275),Instructional_Diagram);

	}

	void loadGame()
	{
		Debug.Log ("level is: " + levelChoice + " mode is: " + gameMode + " players = " + numPlayers);
		//input scene load here
		PlayerPrefs.SetString ("Mode", gameMode);
		PlayerPrefs.SetInt ("Players", numPlayers);
		Application.LoadLevel(levelChoice);
	}

}
