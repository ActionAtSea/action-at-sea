using UnityEngine;
using System.Collections;

public class Screenshotter : MonoBehaviour {
	public KeyCode screenshotKey = KeyCode.F12;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(screenshotKey)) {
			string fileName = System.DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")  + ".png";
			Debug.Log("Created Screenshot: " + fileName);
			Application.CaptureScreenshot(fileName);
	    }
	}
}
