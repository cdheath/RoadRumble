using UnityEngine;
using System.Collections;


public class AI : MonoBehaviour {

	enum turnOption {Right, Left, Straight, UTurn};

	public float maxSpeedRange;
	float moveSpeed;
	public float turnSpeed;
	float reverseTime = 1;
	bool reversing = false;
	bool turning = false;
	string currentRoadDir;
	turnOption turnDir;
	Vector3 startPos;
	Quaternion startRot;
	Vector3 prevPos;
	int counter;
	bool allowStopMoving;
	public GameObject explosionAnimation;

	bool checkAICarMoving = true;

	public GameObject passRay;
	public GameObject driverRay;

	// Use this for initialization
	void Start () {
		startPos = this.transform.position;
		startRot = this.transform.rotation;
		moveSpeed = Random.Range(.1f, maxSpeedRange);
		Vector3 down = new Vector3(0,-90,0);
		RaycastHit leftHit;
		Vector3 leftWheelRaySource = driverRay.transform.position;
		Physics.Raycast(leftWheelRaySource ,down,out leftHit);

		currentRoadDir = leftHit.collider.gameObject.name;
		turnDir = turnOption.Straight;
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 front;

		if (checkAICarMoving) {
			DestroyIfNotMoving();
				}

		switch (currentRoadDir)
		{  
			case "EB-Road":
					front = new Vector3(90, 0, 0);
					break;
			case "WB-Road":
					front = new Vector3(-90, 0, 0);
					break;
			case "SB-Road":
					front = new Vector3(0, 0, 90);
					break;
			default:
					front = new Vector3(0,0,-90);
					break;
		}

		RaycastHit carDetector;
		if(Physics.Raycast(transform.position, front, out carDetector, moveSpeed*10 + 5) )
		{

			if(carDetector.collider.gameObject.tag.CompareTo("aiCar") == 0 || carDetector.collider.gameObject.tag.CompareTo("player") == 0)
			{
				//Debug.Log("car ahead");
			}
		}

		Vector3 down = new Vector3(0,-90,0);
		RaycastHit leftHit;
		RaycastHit rightHit;
		Vector3 leftWheelRaySource = driverRay.transform.position;
		Vector3 rightWheelRaySource = passRay.transform.position;
		Physics.Raycast(leftWheelRaySource ,down,out leftHit);
		Physics.Raycast(rightWheelRaySource ,down,out rightHit);

		if(!reversing)
		{
			transform.Translate(new Vector3( moveSpeed, 0, 0));
		}
		else
		{
			transform.Translate(new Vector3( -moveSpeed, 0, 0));
			reverseTime -= Time.deltaTime;
			if(reverseTime < 0)
			{
				try{
					reversing = false;
					reverseTime = 1;
					if ((leftHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0 && rightHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0) && (leftHit.collider.gameObject.name.CompareTo("Intersection") != 0 && rightHit.collider.gameObject.name.CompareTo("Intersection") != 0))
					{
						moveSpeed = 0;
					}
				}catch(System.Exception ex)
				{
					//Debug.Log(string.Format("Exception Occurred: {0}", ex.Message));
				}
			}
			
		}
		try{
			if(leftHit.collider.gameObject.tag.CompareTo("Intersection") == 0 || rightHit.collider.gameObject.tag.CompareTo("Intersection") == 0)
			{
				if(!turning)
				{
					int newTurnDir;
					if(leftHit.collider.gameObject.name == "4Way-Intersection")
					{
						newTurnDir = Random.Range(0,4);
						if(newTurnDir == 0)
						{
							turnDir=turnOption.Straight;
						}
						else if(newTurnDir == 1)
						{
							turnDir=turnOption.Left;
						}
						else if(newTurnDir == 2)
						{
							turnDir=turnOption.Right;
						}
						else
						{
							turnDir=turnOption.UTurn;
						}
					}
					else
					{
						turnDir = turnOption.UTurn;
					}
				}
				turning = true;
				
			}
			else if(leftHit.collider.gameObject.name.CompareTo(currentRoadDir) == 0 && rightHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0)
			{
				//shift to the left
				Vector3 pos = transform.position;

				if(currentRoadDir == "NB-Road")
				{
					pos.x = Mathf.MoveTowards(pos.x, transform.position.x - moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else if(currentRoadDir == "SB-Road")
				{
					pos.x = Mathf.MoveTowards(pos.x, transform.position.x + moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else if(currentRoadDir == "EB-Road")
				{
					pos.z = Mathf.MoveTowards(pos.z, transform.position.z + moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else
				{
					pos.z = Mathf.MoveTowards(pos.z, transform.position.z - moveSpeed, moveSpeed*10 * Time.deltaTime);
					
				}
				transform.position = pos;
				
			}
			else if (leftHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0 && rightHit.collider.gameObject.name.CompareTo(currentRoadDir) == 0)
			{
				Vector3 pos = transform.position;

				if(currentRoadDir == "SB-Road")
				{
				pos.x = Mathf.MoveTowards(pos.x, transform.position.x - moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else if(currentRoadDir == "NB-Road")
				{
					pos.x = Mathf.MoveTowards(pos.x, transform.position.x + moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else if(currentRoadDir == "WB-Road")
				{
					pos.z = Mathf.MoveTowards(pos.z, transform.position.z + moveSpeed, moveSpeed*10 * Time.deltaTime);
				}
				else
				{
					pos.z = Mathf.MoveTowards(pos.z, transform.position.z - moveSpeed, moveSpeed*10 * Time.deltaTime);

				}
				transform.position = pos;
			}
			else if ((leftHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0 && rightHit.collider.gameObject.name.CompareTo(currentRoadDir) != 0) && (leftHit.collider.gameObject.name.CompareTo("Intersection") != 0 && rightHit.collider.gameObject.name.CompareTo("Intersection") != 0))
			{
				reversing = true;
			}
			else{
				turning = false;

			}
		}
		catch(System.Exception ex)
		{
			Respawn();
			//Debug.Log(string.Format("Exception Occurred: {0}", ex.Message));
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

	void OnCollision(Collision collision)
	{
		moveSpeed = 0;
	}

	void OnTriggerEnter(Collider collision)
	{
		if(turnDir == turnOption.Right && currentRoadDir.CompareTo("EB-Road") == 0)
		{
			if(collision.gameObject.name == "SW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(90,0);
				currentRoadDir = "SB-Road";
				turnDir = turnOption.Straight;
			}
		}
		else if(turnDir == turnOption.Left && currentRoadDir.CompareTo("EB-Road") == 0)
		{
			if(collision.gameObject.name == "SE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(-90,0);
				currentRoadDir = "NB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.UTurn && currentRoadDir.CompareTo("EB-Road") == 0)
		{
			if(collision.gameObject.name == "SW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(-90,0);
				transform.Translate(new Vector3( 2, 0, 0));
				turnCar(0,-90);
				currentRoadDir = "WB-Road";
				turnDir = turnOption.Straight;


			}
		}



		//////////////
		if(turnDir == turnOption.Right && currentRoadDir.CompareTo("SB-Road") == 0)
		{
			if(collision.gameObject.name == "NW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,-90);
				currentRoadDir = "WB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.Left && currentRoadDir.CompareTo("SB-Road") == 0)
		{
			if(collision.gameObject.name == "SW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,90);
				currentRoadDir = "EB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.UTurn && currentRoadDir.CompareTo("SB-Road") == 0)
		{
			if(collision.gameObject.name == "NW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,90);
				transform.Translate(new Vector3( 2, 0, 0));
				turnCar(-90,0);
				currentRoadDir = "NB-Road";
				turnDir = turnOption.Straight;


			}
		}



		///////////
		if(turnDir == turnOption.Right && currentRoadDir.CompareTo("WB-Road") == 0)
		{
			if(collision.gameObject.name == "NE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(-90,0);
				currentRoadDir = "NB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.Left && currentRoadDir.CompareTo("WB-Road") == 0)
		{
			if(collision.gameObject.name == "NW-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(90,0);
				currentRoadDir = "SB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.UTurn && currentRoadDir.CompareTo("WB-Road") == 0)
		{
			if(collision.gameObject.name == "NE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(90,0);

				transform.Translate(new Vector3( 2, 0, 0));
				turnCar(0,90);
				currentRoadDir = "EB-Road";
				turnDir = turnOption.Straight;


			}
		}


		/////////
		if(turnDir == turnOption.Right && currentRoadDir.CompareTo("NB-Road") == 0)
		{
			if(collision.gameObject.name == "SE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,90);
				currentRoadDir = "EB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.Left && currentRoadDir.CompareTo("NB-Road") == 0)
		{
			if(collision.gameObject.name == "NE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,-90);
				currentRoadDir = "WB-Road";
				turnDir = turnOption.Straight;

			}
		}
		else if(turnDir == turnOption.UTurn && currentRoadDir.CompareTo("NB-Road") == 0)
		{
			if(collision.gameObject.name == "SE-Corner")
			{
				transform.Translate(new Vector3( 1, 0, 0));
				turnCar(0,-90);
				transform.Translate(new Vector3( 2, 0, 0));
				turnCar(90,0);
				currentRoadDir = "SB-Road";
				turnDir = turnOption.Straight;

				
			}
		}

	}

	public void StopAIVehicle()
	{
		checkAICarMoving = false;
		moveSpeed = 0;
	}

	public void StartAIVehicle()
	{
		checkAICarMoving = true;
		moveSpeed = Random.Range(.1f, maxSpeedRange);
	}

	public void DestroyIfNotMoving()
	{
		if (this.transform.position == prevPos) 
		{
			counter++;
			if(counter > 25)
			{
				counter = 0;
				Respawn();
			}
			else
			{
				counter++;
			}
		}

		prevPos = this.transform.position;
		checkAICarMoving = true;
//		return null;
	}

	public void SetAIMovingCheckTrue()
	{
		checkAICarMoving = true;
	}

	public void SetAIMovingCheckFalse()
	{
		checkAICarMoving = false;
	}

	void Respawn()
	{
		//Debug.Log ("Respawning AI Car");
		StartCoroutine(Explosion());
		this.transform.position = startPos;
		this.transform.rotation = startRot;
		moveSpeed = Random.Range(.1f, maxSpeedRange);
		Vector3 down = new Vector3(0,-90,0);
		RaycastHit leftHit;
		Vector3 leftWheelRaySource = driverRay.transform.position;
		Physics.Raycast(leftWheelRaySource ,down,out leftHit);
		
		currentRoadDir = leftHit.collider.gameObject.name;
		turnDir = turnOption.Straight;

	}

	IEnumerator Explosion()
	{
		var explosionObject = Instantiate(explosionAnimation, this.transform.position, this.transform.rotation);
		yield return new WaitForSeconds(1.0f);
		Destroy(explosionObject); 
	}
}
