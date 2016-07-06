using UnityEngine;
using System.Collections;

public class LogoScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(Fade (1.0f, 0.0f, 5.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Fade(float startLevel, float endLevel, float duration)
	{
		float speed = 1.0f / duration;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime*speed) 
		{
			Color color = guiTexture.color;
			color.a =  Mathf.Lerp(startLevel, endLevel, t);
			guiTexture.color = color;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds (1.0f);
		Application.LoadLevel("MainMenu");
	}
}
