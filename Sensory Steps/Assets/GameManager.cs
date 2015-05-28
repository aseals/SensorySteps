using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


	//the one and only game manager of this scene
	private static GameManager ourGod;
	public float timebetweencritterspawn;
	//the one and only interaction manager;
	private InteractionManager interactions;
	private GameObject[] spawners;
	public string diosMio;
	public int howmanypeststokill;
	public int maxCrittersOnScreen;
	private int pestsKilled;
	//assign raincloud to this
	public GameObject wateringMethod;
	private oneHand theHand;
	private bool gameOver;
	public enum LevelStatus{
		planting, watering, pestControl, Harvest
		
	}

	public LevelStatus currentLevel;

	//for starting the level, looks for all the mounds
	GameObject[] moundsInLevel;
	bool allPlanted;

	//for starting level2, watering
	GameObject[] cloudsInLevel;
	bool allWatered;
	// Use this for initialization
	void Start () {
		ourGod = this;
		theHand = new oneHand ();
		currentLevel = LevelStatus.planting;
		allPlanted = false;
		moundsInLevel =GameObject.FindGameObjectsWithTag("mound");
		spawners=GameObject.FindGameObjectsWithTag("spawn");
		Debug.Log (moundsInLevel.Length);
		deadPlants = 0;
		gameOver = false;
		aTime = 0;
		pestsKilled = 0;
	}



	int crittersOnScreen;
	float aTime;
	// Update is called once per frame
	void Update () {
		if(currentLevel== LevelStatus.planting){
		foreach (GameObject m in moundsInLevel) 
		{
				if(m.GetComponent<Mound>().getMoundStatus()==Mound.MoundStatus.planted)
				{
					allPlanted=true;
				}
				else
				{
					allPlanted=false;
					break;
				}
		}
		}
		if (currentLevel == LevelStatus.planting && allPlanted) 
		{
			LevelUpToPestControl();
		}


		//if all plants are dead
		if (deadPlants >= moundsInLevel.Length) {

			EndGame();
		}
		if(pestsKilled>=howmanypeststokill)
		{

			EndGame();
		}

		if (Time.time - aTime > timebetweencritterspawn&&crittersOnScreen<maxCrittersOnScreen&&currentLevel==LevelStatus.pestControl) 
		{
			int whichspawner;

			whichspawner=(int) Random.Range (0,spawners.Length);
			Debug.Log(whichspawner);
			if (whichspawner== spawners.Length)
				whichspawner--;
			spawners[whichspawner].SendMessage("Spawn", SendMessageOptions.DontRequireReceiver);
			aTime=Time.time;
		
		}

	}

	public static GameManager Instance()
	{
		return ourGod;

	}

	public override string ToString ()
	{
		return diosMio;
	}

	public void handGrabs()
	{
		//Debug.Log (theHand.isGrabbingSomething);
		theHand.isGrabbingSomething = true;

	}

	public void handLoose()
	{
		theHand.isGrabbingSomething = false;
		
	}

	public bool isHandGrabbing()
	{
		return theHand.isGrabbingSomething;

	}

	//clouds are bugged for this iteration, skipping this phase of the game.
	private void LevelUpToWatering()
	{
		foreach (GameObject m in moundsInLevel) 
		{
			Vector3 RainCloudPosition = new Vector3(m.transform.position.x, m.transform.position.y +10, m.transform.position.z );
			Instantiate (wateringMethod, RainCloudPosition, gameObject.transform.rotation);
		}

		currentLevel = LevelStatus.watering;
	}

	private void LevelUpToPestControl()
	{	

		currentLevel = LevelStatus.pestControl;
		displayStartOfLevelText=true;
		Debug.Log ("NICO NICO FUCK YOU");
		Invoke ("TurnOffText", 5f);
	}

	bool displayStartOfLevelText;
	void OnGUI()
	{
		GUIStyle style = new GUIStyle ();
		style.fontSize = 32;

		if(displayStartOfLevelText)
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2, 100, 100), new GUIContent("Throw the Pests Off Screen!"), style);

		if (currentLevel == LevelStatus.pestControl) {
		}
		
		if(gameOver)
		{	

			if(deadPlants>=moundsInLevel.Length)
				GUI.Label(new Rect(Screen.width/2-50, Screen.height/2, 100, 100), new GUIContent("Game Over"), style);
				else
				GUI.Label(new Rect(Screen.width/2-25, Screen.height/2, 50, 50), new GUIContent("You beat all the pests! You win!"), style);

			GUI.Box(new Rect(10,10,100,90), "");
			
			// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
			if(GUI.Button(new Rect(20,40,80,20), "Play Again")) {
				Application.LoadLevel(1);
			}



		}

	}

	void eraseAllSeeds()
	{
	
		GameObject[] allObjects = FindObjectsOfType<GameObject> ();
		//disable all objects other than the game object
		foreach (GameObject g in allObjects) {
			if(g.tag=="seed")
				Destroy(g);
		}

	}

	void TurnOffText()
	{
		displayStartOfLevelText = false;

	}

	int deadPlants;
	public void CountDeadPlant()
	{
		deadPlants++;

	}
	private void EndGame()
	{
		GameObject[] allObjects = FindObjectsOfType<GameObject> ();
		//disable all objects other than the game object
		foreach (GameObject g in allObjects) {
			if(g.tag!="GameManager"||g.tag!="MainCamera")
				g.SetActive(false);
		}

		gameOver = true;

	}

	public void killedAPest()
	{
		pestsKilled++;
	}

	public void incrementPest()
	{
		crittersOnScreen++;

	}

	public void decrementPest()
	{
		crittersOnScreen--;
		
	}


}



public class oneHand
{
	public bool isGrabbingSomething=false;


}

