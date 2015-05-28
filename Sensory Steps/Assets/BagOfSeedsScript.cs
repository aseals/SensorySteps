using UnityEngine;
using System.Collections;
[RequireComponent(typeof(InteractiveObject))]
public class BagOfSeedsScript : MonoBehaviour {
	//put seeds in here

	public GameObject thisIsABagOf;
	GameManager gManager;
	InteractiveObject interactions;
	bool downTime= false;
	float someTime;
	// Use this for initialization

	void Start () 
	{	
		interactions = GetComponent<InteractiveObject> ();
		//Debug.Log (interactions);
			}
	
	// Update is called once per frame
	void Update () {
		if (gManager == null) {
			gManager=GameManager.Instance();
		}
		//Debug.Log (interactions.getIsGrabbed());
		if (gManager.currentLevel != GameManager.LevelStatus.planting) {
			gameObject.SetActive(false);
		}

		Debug.Log (this.name + " " + interactions.getIsGrabbed ());
	if (interactions.getIsGrabbed()&&!downTime&&!gManager.isHandGrabbing()) 
		{	
			//Debug.Log("I have seeds");
			GameObject newObject;
			newObject= (GameObject)(Instantiate(thisIsABagOf,interactions.getCursorPosition(), Quaternion.identity));

			newObject.SendMessage("SetGrabbed", SendMessageOptions.DontRequireReceiver);
			downTime= true;
			someTime= Time.time;
		}
		if (!interactions.getIsGrabbed()&&downTime&&(Time.time-someTime)>1) 
		
		{
			downTime= false;
		}
	}
}
