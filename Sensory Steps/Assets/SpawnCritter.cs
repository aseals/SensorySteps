using UnityEngine;
using System.Collections;

public class SpawnCritter : MonoBehaviour {

	// Use this for initialization
	public Vector3 Go;

	//set to the critter prefab
	GameManager gmanager;
	public GameObject critter;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if (gmanager == null) {
			gmanager= GameManager.Instance();}
	}
	void Spawn()
	{

		GameObject g =(GameObject)Instantiate (critter, transform.position, transform.rotation);
		if (g.GetComponent<Critter> () != null) {
			Critter c =g.GetComponent<Critter> ();
			c.walkDirection = Go;
			Debug.Log (gameObject.name + " "+ Go.ToString()+" "+ c.walkDirection.ToString());
		}
		gmanager.incrementPest ();
	}

}
