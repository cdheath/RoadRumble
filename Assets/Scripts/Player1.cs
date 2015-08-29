﻿using UnityEngine;
using System.Collections;

public class Player1 : Player {

	float absLeftRight;
	float absUpDown;

	//method to process the car's control inputs
	public override void processControls()
	{

		
		float leftRight;
		float upDown;

		leftRight = lStickVector.y;
		upDown = lStickVector.x;

		//Controls for on PC testing
		////////////TO BE DELETED?////////////////
		if(compTesting)
		{
			if (Input.GetKey(KeyCode.D)) 
			{
				transform.Translate(new Vector3( moveSpeed,0, 0));

				//if(transform.localEulerAngles.y != 0)
				//{
					turnCar(0, 360);
				//}

			}
			if (Input.GetKey(KeyCode.S)) 
			{transform.Translate(new Vector3( moveSpeed, 0, 0));
				//this.transform.Translate(new Vector3( 0,0, -moveSpeed));//-

				//if(transform.localEulerAngles.y != 90)
				//{
					turnCar(90, 0);
				//}
			}
			if (Input.GetKey(KeyCode.A)) 
			{
				transform.Translate(new Vector3( moveSpeed, 0, 0));//-

				//if(transform.localEulerAngles.y != 180)
				//{
					turnCar(0, -90);
				//}
			}
			if (Input.GetKey(KeyCode.W)) 
			{transform.Translate(new Vector3( moveSpeed, 0, 0));
				//this.transform.Translate(new Vector3( 0, 0, moveSpeed));

				//if(transform.localEulerAngles.y != 270)
				//{
					turnCar(-90, 0);
				//}
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

	[GuiTarget(GuiTarget.Target.GamePad)]
	void OnGUI()
	{
		if(skin != null)
		{
			GUI.skin = skin;
		}

		Matrix4x4 matrixBackup = GUI.matrix;
		GUIUtility.RotateAroundPivot(90, new Vector2(Screen.width/2,Screen.height/2));
		GUI.Label (new Rect (200, 620, 1000, 40),score.ToString(), GUI.skin.GetStyle("number"));
		//GUI.Label (new Rect (Screen.width - 80, 60, 100, 40), player2Score.ToString());
		GUI.matrix = matrixBackup;
	}
}
