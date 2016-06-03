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
			skin.button.fontSize = 125;
			if (GUI.Button(new Rect(page.width * 1/4,0, page.width, page.height*1/2), "Retro Road\nRumble"))
			{
				skin.button.fontSize = 65;
				currentMenu = menu.main;
				break;
			}
			skin.button.fontSize = 50;
			if (GUI.Button(new Rect(page.width * 1/4,page.height*3/4, page.width, page.height*1/4), "Tap to Continue")) 
			{
				skin.button.fontSize = 65;
				currentMenu = menu.main;
			}
			break;
		case menu.main:
			if (GUI.Button(new Rect(page.width * 1/4, page.height * 1/5 - 50, page.width, page.height * 1/5), "1 Player")) 
			{
			}
			if (GUI.Button(new Rect(page.width * 1/4, page.height * 2/5 - 50, page.width, page.height * 1/5), "2 Player")) 
			{
			}
			if (GUI.Button(new Rect(page.width * 1/4, page.height * 3/5 - 50, page.width, page.height * 1/5), "Instructions")) 
			{
			}
			if (GUI.Button(new Rect(page.width * 1/4, page.height * 4/5 - 50, page.width, page.height * 1/5), "Credits")) 
			{
			}
			break;
		case menu.highScores:
			break;
		case menu.gameModes:
			if (GUI.Button(new Rect(page.width * 1/4,page.height * 1/3, page.width, 100), "Score")) 
			{
			}
			if (GUI.Button(new Rect(page.width * 1/4,page.height * 2/3, page.width, 100), "Timed")) 
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
			GUI.DrawTexture(new Rect(page.width/2, 100, 750,400),loadingScene);
			GUI.Label(new Rect(100,50, 200, 50), "Loading...");
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
		if (currentMenu != menu.levels && currentMenu != menu.instructions) 
		{
			if (carPos > page.width + 250) {
					carPos = -250;
			}
			if (carPos % 10 == 0) {
					GUI.DrawTexture (new Rect (carPos, page.height - 52, 100, 50), carImg);
			} else {
					GUI.DrawTexture (new Rect (carPos, page.height - 50, 100, 50), carImg);
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
		
		GUI.Label(new Rect(page.width/2,25, 500, 125), "Select Limit", skin.GetStyle("Button"));

		int y = (int)(page.height/limits.Length)/2 + 75;
		int x = (int)(page.width * .25);
		foreach(int opt in limits)
		{
			if (GUI.Button(new Rect(x,y, page.width/2, 175), opt.ToString() + " " + units)) 
			{
			}
			if(x == (int)(page.width * .25))
			{
				x = (int)(page.width * .7);
			}
			else
			{
				y+=(int)(page.height/limits.Length)/2 + 100;
				x = (int)(page.width * .25);
			}
		}
	}
	
	public void toggleLevels()
	{
		currentMenu = menu.levels;
		int x = (int)(page.width * 1/8);
		int y = (int)(page.width * 1/10);
		GUI.Toggle(new Rect(page.width-100,20, 40, 50),nightOn, "Night", skin.GetStyle("Toggle"));
		GUI.Label(new Rect(page.width-170,30, 75, 75), "Time:", skin.GetStyle("Label"));
		
		skin.label.fontSize = 18;
		
		foreach(Texture level in levels)
		{
			if (GUI.Button(new Rect(x,y, page.width * 1/3, page.height * 1/3), level)) 
			{
				levelLoad = level;
			}
			GUI.Label(new Rect(x,y+300, page.width * 1/5 + 175, page.height * 1/5 + 125), level.name, skin.GetStyle("Label"));

			x+= (int)(page.width * 0.3);
			if(x > (Screen.width - 500))
			{
				x = (int)page.width * 1/8;
				y += (int)page.width * 1/3;
			}
		}
		skin.label.fontSize = 20;
		
	}
	
	public void toggleCredits()
	{
		float half = page.width/2;
		
		skin.label.fontSize = 35;
		GUI.Label(new Rect(half/2,50, page.width, 50), "Created by:");
		GUI.Label(new Rect(half/2,150, page.width, 50), "Music:");
		
		skin.label.fontSize = 20;
		
		GUI.Label(new Rect(half/2,page.height/6, half, 50), "Corey Heath");
		GUI.Label(new Rect(half + page.width/4,page.height/6, half, 50), "Tracey Heath");		
		GUI.Label(new Rect(half/2,page.height/6 + 100, half, 75), "A Breeze From Alabama - Scott Joplin (1902)".PadRight(56));
		GUI.Label(new Rect(half +  page.width/4,page.height/6 + 100, half, 75), "Chicken Reel - Joseph M Daly (1910)".PadRight(56));
		GUI.Label(new Rect(half/2,page.height/6 + 150, half, 75), "Fig Leaf Rag - Scott Joplin (1908)".PadRight(56));
		GUI.Label(new Rect(half + page.width/4,page.height/6 + 150, half, 75), "Hallowe'en - Arthur Manlowe (1911)".PadRight(56));
		GUI.Label(new Rect(half/2,page.height/6 + 200, half, 75), "Heliotrope Bouquet - Scott Joplin & Louis Chauvin (1907)".PadRight(56));
		GUI.Label(new Rect(half +  page.width/4,page.height/6 + 200, half, 75), "Maple Leaf Rag - Scott Joplin (1899)".PadRight(56));
		GUI.Label(new Rect(half/2,page.height/6 + 250, half, 75), "The St. Louis Rag - Thoms Million Turpin (1903)".PadRight(56));
		GUI.Label(new Rect(half +  page.width/4,page.height/6 + 250, half, 75), "Whistling Rufus - Frederick Allen Mills (1899)".PadRight(56));
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
		GUI.DrawTexture(new Rect(page.width/3, 100, 950,475),Instructional_Diagram);
		GUI.Label(new Rect(page.width/3 - 125,50, page.width-25, 50),"Drive your car to the green marker to score points.");
	}
}
