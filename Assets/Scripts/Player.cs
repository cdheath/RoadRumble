
using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour {

	public float moveSpeed;
	public float terminalVelocity;
	public bool compTesting;
	public GUISkin skin;
	public GameObject explosionAnimation;
	public GameObject smokeOnlyEffect;

	private Quaternion startRot;
	private Vector3 startPos;
	private bool exploding = false;
	protected int score = 0;
	private float invincibilityTimer;
	private int invincibleLimit = 2;
	enum car_state{New, Dinged, Battered , Wrecked};
	car_state respawnState = car_state.Wrecked;
	car_state currentState;

	//binding bools to each button
	protected bool pressedButtonA = false;
	protected bool pressedButtonB = false;
	protected bool pressedButtonX = false;					// X button (WiiU GamePad).
	protected bool pressedButtonY = false;					// Y button (WiiU GamePad).
	protected bool pressedButtonLeft = false;					// Left button (WiiU GamePad).
	protected bool pressedButtonRight = false;				// Right button (WiiU GamePad).
	protected bool pressedButtonUp = false;					// Up button (WiiU GamePad).
	protected bool pressedButtonDown = false;					// Down button (WiiU GamePad).
	protected bool pressedButtonZR = false;					// Trigger ZR button (WiiU GamePad).
	protected bool pressedButtonZL = false;					// Trigger ZL button (WiiU GamePad).
	protected bool pressedButtonR = false;					// Trigger R button (WiiU GamePad).
	protected bool pressedButtonL = false;					// Trigger L button (WiiU GamePad).
	protected bool pressedButtonPlus = false;					// + button (WiiU GamePad).
	protected bool pressedButtonMinus = false;				// - button (WiiU GamePad).
	protected bool pressedButtonStickL = false;				// Stick left button (WiiU GamePad).
	protected bool pressedButtonStickR = false;				// Stick right button (WiiU GamePad).
	protected Vector2 lStickVector = new Vector2();			// left joystick direction vector
	protected Vector2 rStickVector = new Vector2();			// right joystick direction vector
	protected Vector2 touchPosition = new Vector2(-1f,-1f);	// x,y coords on gamePad for touch
	protected Vector3 gyro = new Vector3();

	void Start () 
	{
		startPos = transform.position;
		startRot = transform.rotation;
		currentState = car_state.New;
		invincibilityTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {

		//declaring the game pad and mapping it's controls to the predeclared variables
		WiiUGamePad gamePad = WiiUInput.GetGamePad ();
		
		pressedButtonA = gamePad.GetButton(WiiUGamePadButton.ButtonA);
		pressedButtonB = gamePad.GetButton(WiiUGamePadButton.ButtonB);
		pressedButtonX = gamePad.GetButton(WiiUGamePadButton.ButtonX);
		pressedButtonY = gamePad.GetButton(WiiUGamePadButton.ButtonY);
		pressedButtonLeft = gamePad.GetButton(WiiUGamePadButton.ButtonLeft);
		pressedButtonRight = gamePad.GetButton(WiiUGamePadButton.ButtonRight);
		pressedButtonUp = gamePad.GetButton(WiiUGamePadButton.ButtonUp);
		pressedButtonDown = gamePad.GetButton(WiiUGamePadButton.ButtonDown);
		pressedButtonZR = gamePad.GetButton(WiiUGamePadButton.ButtonZR);
		pressedButtonZL = gamePad.GetButton(WiiUGamePadButton.ButtonZL);
		pressedButtonR = gamePad.GetButton(WiiUGamePadButton.ButtonR);
		pressedButtonL = gamePad.GetButton(WiiUGamePadButton.ButtonL);
		pressedButtonPlus = gamePad.GetButton(WiiUGamePadButton.ButtonPlus);
		pressedButtonMinus = gamePad.GetButton(WiiUGamePadButton.ButtonMinus);
		pressedButtonStickL = gamePad.GetButton(WiiUGamePadButton.ButtonStickL);
		pressedButtonStickR = gamePad.GetButton(WiiUGamePadButton.ButtonStickR);
		lStickVector = gamePad.leftStick;
		rStickVector = gamePad.rightStick;
		touchPosition = (Input.touchCount > 0) ? Input.touches[0].position : new Vector2(-1f,-1f);
		gyro = gamePad.gyro;

		//Debug.Log ("x = " + gyro.x + " y = " + gyro.y + " z = " + gyro.z);

		//running the individual player controls
		if(!exploding)
		{
			processControls();
		}
		if(invincibilityTimer != 0)
		{
			invincibilityTimer -= Time.deltaTime;
		}

	}

	//Collision handler
	IEnumerator OnCollisionEnter(Collision collision)
	{
		Debug.Log ("Collision Detected");
		Debug.Log(collision.gameObject.tag);
		if(collision.gameObject.tag != "Road" && collision.gameObject.tag != "Intersection" && collision.gameObject.tag != "DrivableArea" && collision.gameObject.tag != "ScoreZone" && 
		   collision.gameObject.tag != "GameBoundary" && collision.gameObject.tag != "ScoreZone" && collision.gameObject.tag != "FrozenLake" && collision.gameObject.tag != "terrain" )
		{
			Debug.Log ("Entered Damage Clause");
			Debug.Log (string.Format("Invisibility Timer = {0}", invincibilityTimer));
			Debug.Log (string.Format("velocity = {0}", getVelocity()));
			if(getVelocity() >= terminalVelocity && invincibilityTimer <= 0)
			{	

				currentState++;
				Debug.Log (string.Format("Damage State = {0}", currentState));
				invincibilityTimer = invincibleLimit;
				if(currentState == car_state.Battered)
				{
					this.gameObject.GetComponentInChildren<ParticleSystem>().startColor = new Color(200, 200, 200, .5f);
					this.gameObject.GetComponentInChildren<ParticleSystem>().Play();
				}
				else if(currentState == car_state.Battered)
				{
					this.gameObject.GetComponentInChildren<ParticleSystem>().startColor = new Color(101, 101, 101, .5f);
					this.gameObject.GetComponentInChildren<ParticleSystem>().Play();
				}
				else if(collision.gameObject.tag == "GameBoundary" || currentState >= respawnState)
				{
					exploding = true;
					this.gameObject.GetComponentInChildren<ParticleSystem>().Stop();
					var explosionObject = Instantiate(explosionAnimation, this.transform.position, this.transform.rotation);
					//audio.Play();
					WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
					WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
					
					yield return new WaitForSeconds(2);
					this.transform.position = startPos;
					this.transform.rotation = startRot;
					this.rigidbody.velocity = Vector3.zero;
					this.rigidbody.angularVelocity = Vector3.zero;
					currentState = car_state.New;
					exploding = false;
					Destroy(explosionObject);
				}
			}
		}
	}

	//Boundary Exit Destroy
	IEnumerator OnTriggerExit(Collider collider)
	{
		Debug.Log ("Boundary Exit Occurred");
		if (collider.gameObject.tag == "GameBoundary") {
			exploding = true;
			
			var explosionObject = Instantiate(explosionAnimation, this.transform.position, this.transform.rotation);
			//audio.Play();
			WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
			WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
			
			yield return new WaitForSeconds(2);
			this.transform.position = startPos;
			this.transform.rotation = startRot;
			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.angularVelocity = Vector3.zero;
			currentState = car_state.New;
			exploding = false;
			Destroy(explosionObject);
				}
	}

	public void turnCar(float leftRight, float upDown)
	{
		Vector3 to = new Vector3(leftRight, 0, upDown);
		if(to != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(to);
		}
	}

	public void addPoints(int scoreToAdd)
	{
		score += scoreToAdd;

		string[] toSend = new string[2];
		toSend [0] = this.name;
		toSend [1] = this.score.ToString ();
		GameObject.Find ("GameMechanics").SendMessage ("updateScore", toSend);

		Debug.Log (gameObject.name + "'s score is now " + score);
	}

	public int getPoints()
	{
		return score;
	}

	//abstract methods to implement controls for individual players

	//method to process the car's control inputs
	public abstract void processControls();

	//method to return the car's current velocity
	public abstract float getVelocity();

}
