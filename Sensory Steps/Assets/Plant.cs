using UnityEngine;
using System.Collections;
[RequireComponent (typeof (Collider))]
public class Plant : MonoBehaviour {

	enum PlantStates{
		sprout,
		mid,
		flowering,
		fruitbearing, withered
			
			
}
	public float startsize;
	public float endsize;
	public float startPositionOffset;
	PlantStates thisPlantsState;
	public float growthSpeed;
	public float healthBarOffset;
	public int PlantHealth;
	float startHeight;
	float endHeight;
	GameManager gManager;

	float growth = 0;

	bool isBeingEaten;
	// Use this for initialization
	void Start () {
		thisPlantsState = PlantStates.sprout;
		transform.localScale = new Vector3 (startsize, startsize, 1);
		startHeight= transform.position.y - startPositionOffset;
		endHeight = transform.position.y;
		//transform.position.y = startHeight - startPositionOffset;
	}
	
	// Update is called once per frame
	void Update () {
		gManager = GameManager.Instance();
		if (thisPlantsState == PlantStates.sprout&&growth<1) {
			growth = growth+growthSpeed*Time.deltaTime;
			transform.localScale=Vector3.Lerp( new Vector3 (startsize, startsize, 1), new Vector3(endsize,endsize, 1), growth);
			transform.position=new Vector3(transform.position.x, Mathf.Lerp(startHeight, endHeight, growth), transform.position.z);
			if(growth>1)
			{	growth = 1;
				GetComponent<ParticleSystem>().Stop();
			}
		}

		if (PlantHealth == 0) 
		{
		
			Destroy(gameObject);
			//then send the gameManager a message about death
		}
	}

	void OnGUI()
	{
		if (gManager.currentLevel == GameManager.LevelStatus.pestControl) 
		{
			Vector3 guiPosition = Camera.main.WorldToScreenPoint(transform.position);
			guiPosition.y+= healthBarOffset;

			GUI.Label(new Rect(guiPosition.x, guiPosition.y, 15,15), new GUIContent(PlantHealth.ToString()));
		}

	}

	void TakeDamage()
	{

		PlantHealth--;
	}

	void OnDestroy()
	{
		gManager.CountDeadPlant ();

	}
}
