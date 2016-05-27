using UnityEngine;
using System.Collections;

public class Player2 : Player {

	float absLeftRight;
	float absUpDown;

	//method to process the car's control inputs
	public override void processControls()
	{
		
		float leftRight;
		float upDown;
		
		leftRight = rStickVector.y;
		upDown = rStickVector.x;

		//Controls for on PC testing
		////////////TO BE DELETED?////////////////
		if(compTesting)
		{
			if (Input.GetKey(KeyCode.L)) 
			{
				transform.Translate(new Vector3( moveSpeed, 0, 0));
				turnCar(0, 360);
			}
			if (Input.GetKey(KeyCode.K)) 
			{
				transform.Translate(new Vector3( moveSpeed, 0, 0));
				turnCar(90, 0);
			}
			if (Input.GetKey(KeyCode.J)) 
			{
				transform.Translate(new Vector3( moveSpeed, 0, 0));
				turnCar(0, -90);
			}
			if (Input.GetKey(KeyCode.I)) 
			{
				transform.Translate(new Vector3( moveSpeed, 0, 0));
				turnCar(-90, 0);
			}

		}
		else
		{
			absUpDown = Mathf.Abs(upDown);
			absLeftRight = Mathf.Abs(leftRight);
			if(absUpDown > absLeftRight)
			{
				transform.Translate(new Vector3(absUpDown, 0, 0));// absLeftRight));
			}
			else
			{
				transform.Translate(new Vector3(absLeftRight, 0, 0));//absUpDown));
			}
			turnCar(-leftRight*360, upDown*360);

		}
	}

	//method to return the car's current velocity
	public override float getVelocity()
	{
		if(compTesting)
		{
			return moveSpeed;
		}
		else
		{
			if(absUpDown > absLeftRight)
			{
				return absUpDown;
			}
			else
			{
				return absLeftRight;
			}
		}
	}

	public void LockControls()
	{
		PauseControls();
	}
	
	public void UnlockControls()
	{
		UnpauseControls();
	}

	/*[GuiTarget(GuiTarget.Target.GamePad)]
	void OnGUI()
	{
		if(skin != null)
		{
			GUI.skin = skin;
		}
		Matrix4x4 matrixBackup = GUI.matrix;
		GUIUtility.RotateAroundPivot(-90, new Vector2(Screen.width/2,Screen.height/2));
		//Note: Parameters below represent the screen while rotated
		GUI.Label (new Rect ( Screen.width/2 - 225, Screen.width/2 + 200,50, 40),score.ToString(), GUI.skin.GetStyle("number"));

		//GUI.Label (new Rect (Screen.width - 80, 60, 100, 40), score.ToString());
		GUI.matrix = matrixBackup;
	}*/
}
