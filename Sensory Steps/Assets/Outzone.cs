using UnityEngine;
using System.Collections;

public class Outzone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	GameManager gmanager;
	// Update is called once per frame
	void Update () {
	if (gmanager == null) {
			gmanager=GameManager.Instance();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Destroy(other.gameObject);
		gmanager.killedAPest ();
	}
}
