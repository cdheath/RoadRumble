using UnityEngine;
using System.Collections;

public class ScoreZone : MonoBehaviour {
	GameObject gameMechanics;
	// Use this for initialization
	void Start () 
	{
		gameMechanics = GameObject.Find ("GameMechanics");
		
	}

	IEnumerator OnTriggerEnter(Collider collider)
	{
		if(this.enabled)
		{
			if(collider.gameObject.tag == "Player")
			{
				audio.Play ();
				yield return new WaitForSeconds(1);
				//add to score
				//trigger reset
				collider.gameObject.SendMessage("addPoints", 100);
				gameMechanics.SendMessage("setNewGoal");
			}
		}
	}
}
