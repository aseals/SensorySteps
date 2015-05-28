using UnityEngine;

using System.Collections;


public class Mound : MonoBehaviour {
	public Material material;

	//always assign the plant to this
	public GameObject growth;

	public float displacement;

	MoundStatus status;
	// Use this for initialization
	public enum MoundStatus{
		planted,
		unplanted
	

	}

	void Start () {

		status = MoundStatus.unplanted;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerStay(Collider Other)
	{	
//		Debug.Log ("I AM " + Other.gameObject.name);
		if (Other.gameObject.name.Contains( "Seed")&&status==MoundStatus.unplanted) 
		{
			GameObject.Destroy(Other.gameObject);
			GameObject newplant;
			newplant= (GameObject)Instantiate (growth, new Vector3(transform.position.x, transform.position.y+displacement,transform.position.z), transform.rotation	                                   );
			Debug.Log("NICO NICO FUCK YOU");
			status= MoundStatus.planted;
			//newplant.transform.position= transform.position;
		}

	}

	public MoundStatus getMoundStatus()
	{
		return status;

	}
}
