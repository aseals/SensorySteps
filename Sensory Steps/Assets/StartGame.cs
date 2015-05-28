using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if (GUI.Button (new Rect (Screen.width / 2, Screen.height / 2, 50, 50), "Start Game")) {
			Application.LoadLevel(1);
		
		}
	}
}
