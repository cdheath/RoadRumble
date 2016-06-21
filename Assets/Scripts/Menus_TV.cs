using UnityEngine;
using System.Collections;

public class Menus_TV : MonoBehaviour {
	public Texture screenToShow;
	public Texture back;
	public Texture Instructional_Diagram;
	public Texture popup_background;
	public Texture carImg;
	public Texture[] levels;
	public GUISkin skin;

	string gameMode;
	int[] maxMins = {1,2,3,4,5,6};
	int[] maxScores = {500,1000,1500,2000,2500,3000};
	Texture loadingScene;

	menu currentMenu = menu.title;
	int carPos;	
	Rect page;
	GameObject toChange;
	public bool nightOn = false;
	Texture levelLoad;

	// Use this for initialization
	void Start () {
		page.x = 0;
		page.y = 0;
/*		//Debug.Log ("TV1 Page Width: " + Screen.currentResolution.width + " Page Height: " + Screen.currentResolution.height);
		if (Screen.currentResolution.width != 854) 
		{
			page.width = Screen.currentResolution.width + Screen.currentResolution.width/4;
			page.height = Screen.currentResolution.height;

			PlayerPrefs.SetInt("tv_width", Screen.currentResolution.width);
			PlayerPrefs.SetInt("tv_height", Screen.currentResolution.height);
		}
		else
		{
			if(PlayerPrefs.HasKey("tv_width"))
			{
				page.width = PlayerPrefs.GetInt("tv_width");
				page.height = PlayerPrefs.GetInt ("tv_height");
			}
			else
			{
				page.width = 1280;
				page.height = 720;
				PlayerPrefs.SetInt("tv_width", 1280);
				PlayerPrefs.SetInt("tv_height", 720);
			}
		}
*/
		//Only looks correct on TV with hardcoded values
		page.width = 1920;
		page.height = 1080;
		carPos = 0;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//constantly being called
	[GuiTarget(GuiTarget.Target.TV)]
	void OnGUI()
	{ 		
		if(skin != null)
		{
			GUI.skin = skin;
		}
		
		switch(currentMenu)
		{
		case menu.title:
			//skin.button.fontSize = 125;
			if (GUI.Button(new Rect(0,page.height * 1/8, page.width, page.height*1/2), "Retro Road\nRumble", GUI.skin.GetStyle("Title_TV")))
			{
			//	skin.button.fontSize = 65;
				currentMenu = menu.main;
				break;
			}
			//skin.button.fontSize = 50;
			if (GUI.Button(new Rect(0, page.height*5/8, page.width, page.height*1/4), "Tap to Continue", GUI.skin.GetStyle("TapContinue_TV"))) 
			{
				//skin.button.fontSize = 65;
				currentMenu = menu.main;
			}
			break;
		case menu.main:
			if (GUI.Button(new Rect(0, page.height * 1/8, page.width, page.height * 1/6), "1 Player", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			if (GUI.Button(new Rect(0, page.height * 2/8, page.width, page.height * 1/6), "2 Player", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			if (GUI.Button(new Rect(0, page.height * 3/8, page.width, page.height * 1/6), "Instructions", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			if (GUI.Button(new Rect(0, page.height * 4/8, page.width, page.height * 1/6), "Credits", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			break;
		case menu.highScores:
			break;
		case menu.gameModes:
			if (GUI.Button(new Rect(0,page.height * 1/8, page.width, page.height * 1/6), "Score", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			if (GUI.Button(new Rect(0,page.height * 2/8, page.width, page.height * 1/6), "Timed", GUI.skin.GetStyle("Labels_TV"))) 
			{
			}
			break;
		case menu.limits:
			toggleVictoryConditions();
			break;
		case menu.levels:
			toggleLevels();
			break;
		case menu.loading:
			GUI.DrawTexture(new Rect(200, 150, page.width - 400 ,page.height - 300),loadingScene);
			GUI.Label(new Rect(0,50, page.width, page.height * 1/6), "Loading...", GUI.skin.GetStyle("Labels_TV"));
			break;
		case menu.instructions:
			toggleInstructions();
			break;
		case menu.credits:
			toggleCredits();
			break;
		default:
			break;
		}
		if (currentMenu != menu.levels && currentMenu != menu.instructions && currentMenu != menu.loading) 
		{
			if (carPos > page.width + 250) {
					carPos = -250;
			}
			if (carPos % 10 == 0) {
					GUI.DrawTexture (new Rect (carPos, page.height * 7/8, 100, 50), carImg);
			} else {
					GUI.DrawTexture (new Rect (carPos, page.height * 7/8, 100, 50), carImg);
			}
			carPos += 2;
		}
	}

	public void ChangeMenu(menu newMenu)
	{
		currentMenu = newMenu;
	}
	
	public void ChangeMenu(menu newMenu, Texture load)
	{
		currentMenu = newMenu;
		loadingScene = load;
	}
	
	public IEnumerator toggleTitle(bool visible)
	{
		toChange = GameObject.Find("Title");
		toChange.GetComponent<GUIText>().enabled = visible;
		toChange = GameObject.Find("Continue");
		toChange.GetComponent<GUIText>().enabled = visible;
		yield return new WaitForSeconds(1);
	}

	public void sendVictoryConditions(string mode, int[] mins, int[] scores)
	{
		gameMode = mode;
		maxMins = mins;
		maxScores = scores;
	}
	
	public void toggleVictoryConditions()
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
		
		GUI.Label(new Rect(0, page.height * 1/8, page.width, page.height * 1/6), "Select Limit", skin.GetStyle("LimitLabels_TV"));

		int y = (int)((page.height/limits.Length)/2 + (page.height * 1/8));
		int x = 0;
		bool leftSide = true;

		foreach(int opt in limits)
		{
			string styleType = leftSide ? "LimitButtons_TV_Left" : "LimitButtons_TV_Right";
			if (GUI.Button(new Rect(x,y, page.width * 0.42f, page.width * 1/6), opt.ToString() + " " + units, skin.GetStyle(styleType))) 
			{
			}
			if(x == 0)
			{
				x = (int)(page.width * 0.3f);
			}
			else
			{
				y+=(int)(page.height/limits.Length)/2;
				x = 0;
				leftSide = true;
			}
		}
	}
	
	public void toggleLevels()
	{
		currentMenu = menu.levels;
		int x = (int)(page.width * 1/12);
		int y = (int)(page.height * 1/9);
		GUI.Toggle(new Rect(page.width - 200,40, 40, 50),nightOn, "Night", skin.GetStyle("Toggle"));
		GUI.Label(new Rect(page.width - 280,50, 75, 75), "Time:", skin.GetStyle("Label"));
		
		//skin.label.fontSize = 18;
		
		foreach(Texture level in levels)
		{
			if (GUI.Button(new Rect(x,y, page.width * 1/4, page.height * 1/4), level)) 
			{
				levelLoad = level;
			}
			GUI.Label(new Rect(x,y + 300, page.width * 1/4, page.height * 1/5 + 125), level.name, skin.GetStyle("LevelLabels_TV"));

			x+= (int)(page.width * 0.21f);
			if(x > (page.width - 500))
			{
				x = (int)(page.width * 1/12);
				y += (int)page.width * 1/4;
			}
		}
		//skin.label.fontSize = 20;
		
	}
	
	public void toggleCredits()
	{
		float half = page.width/2;
		float y = page.height * 0.4f;
		//skin.label.fontSize = 35;
		GUI.Label(new Rect(0, page.height * 1/7, page.width, 50), "Created by:", skin.GetStyle("CreditHeaders_TV"));
		GUI.Label(new Rect(0,page.height * 0.3f, page.width, 50), "Music:", skin.GetStyle("CreditHeaders_TV"));
		
		//skin.label.fontSize = 20;
		
		GUI.Label(new Rect(0,page.height * 0.22f, page.width * 0.42f, page.height * 1/7), "Corey Heath", skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect((int)(page.width * 0.5f), page.height * 0.22f, page.width * 0.42f, page.height * 1/7), "Tracey Heath", skin.GetStyle("CreditLabels_TV"));		
		GUI.Label(new Rect(0,y, page.width, page.height * 1/7), "A Breeze From Alabama - Scott Joplin (1902)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 50, page.width, page.height * 1/7), "Chicken Reel - Joseph M Daly (1910)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 100, page.width, page.height * 1/7), "Fig Leaf Rag - Scott Joplin (1908)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 150, page.width, page.height * 1/7), "Hallowe'en - Arthur Manlowe (1911)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 200, page.width, page.height * 1/7), "Heliotrope Bouquet - Scott Joplin & Louis Chauvin (1907)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 250, page.width, page.height * 1/7), "Maple Leaf Rag - Scott Joplin (1899)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 300, page.width, page.height * 1/7), "The St. Louis Rag - Thoms Million Turpin (1903)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
		GUI.Label(new Rect(0,y + 350, page.width, page.height * 1/7), "Whistling Rufus - Frederick Allen Mills (1899)".PadRight(56), skin.GetStyle("CreditLabels_TV"));
	}
	
	public void toggleInstructions()
	{
		/*GUI.Label(new Rect(25,50, page.width-25, 125), "Get your jalopy to the green drop off point, on the double." + 
		          "You see anyone else on the make, be sure they don't get there before you." +
		          "Mind you don't conk into anything or your car won't be worth a plugged nickel before long.");

		GUI.Label(new Rect(25,175, page.width-25, 100),"Now here's a real humdinger." + 
		          "Get a wiggle on that gamepad to really razz those rubes, but mind it doesn't grift you outta the running too.");
		GUI.Label(new Rect(25,250, page.width-25, 50),"Now you're on the trolley!");*/
		
		//GUI.DrawTexture(new Rect(page.width/2 - 275, 200, 550,275),Instructional_Diagram);
		GUI.DrawTexture(new Rect(page.width * 1/8, page.height * 1/3, page.width - page.width * 2/8, page.height * 1/2),Instructional_Diagram);
		GUI.Label(new Rect(0,page.height * 1/10, page.width, page.height * 1/6),"Drive your car to the green marker to score points.", GUI.skin.GetStyle("Labels_TV"));
	}
}
