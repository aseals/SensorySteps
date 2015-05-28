using UnityEngine;
using System.Collections;

public class Critter : MonoBehaviour {

	//throws are calculated based on a combination of grab location, release location, and timing. the following variable describes how long it
	//it takes for the hand to hold the critter at a new location, for the throw to be recalculated on the new position
	public float normalizeThrowPositionRate;
//to give the throw an extra kick, we take the magnitude of the throw vector and raise to the power of the following variable.
	public float exponentialThrowScaling;
	public float walkspeed;
	public Vector3 walkDirection;
	GameManager gManager;
	float setYAxisLevel;
	InteractiveObject interactions;
	Vector3 lastposition;
	Vector3 originalGrabPosition;
	float howfaralong;
	Rigidbody forthrow;
	float throwTime;
	//describes what is happening
	//free = critter is moving on its own
	//grabbed= the player is holding it
	//throw3n= the player has grabbed it and let go
	enum critterStates{
		free,
		grabbed, 
		thrown

	}

	critterStates status;
	// Use this for initialization
	void Awake () {
		setYAxisLevel = transform.position.y;
		interactions = gameObject.GetComponent<InteractiveObject> ();
		status = critterStates.free;
		forthrow = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gManager == null) {
			gManager=GameManager.Instance();}
		if (status == critterStates.free) {

			transform.position += Vector3.Normalize(walkDirection)*walkspeed*Time.deltaTime;
		}
		//critter moves on its own and gets grabbe
		if (status == critterStates.free && interactions.getIsGrabbed()) {
			status=critterStates.grabbed;
			lastposition=transform.position;
			originalGrabPosition= lastposition;
			howfaralong=0;
		}
		//grabbed and is being held
		if (status==critterStates.grabbed&& interactions.getIsGrabbed()) {

			howfaralong+=normalizeThrowPositionRate*Time.deltaTime;
			lastposition = transform.position;


		}

		//grabbed and let go (thrown)
		if(!interactions.getIsGrabbed()&&status==critterStates.grabbed)
		{
			status=critterStates.thrown;
			interactions.isDraggable=false;
			forthrow.isKinematic=false;
			forthrow.useGravity=true;

			Debug.Log ((transform.position-lastposition)/(Time.fixedDeltaTime)+ "  "+ howfaralong);
			forthrow.AddForce(Vector3.Normalize((interactions.getCursorPosition()-lastposition)/(Time.fixedDeltaTime))*Mathf.Pow(Vector3.Magnitude((interactions.getCursorPosition()-lastposition)/(Time.fixedDeltaTime)), exponentialThrowScaling));
		

		}

		//thrown and fell back to its original level.
		if (status == critterStates.thrown) 
		{
			if(transform.position.y <=setYAxisLevel)
			{
				forthrow.isKinematic=true;
				forthrow.useGravity=false;
				transform.position=new Vector3(transform.position.x, setYAxisLevel, transform.position.z);
				status=critterStates.free;
				forthrow.velocity=Vector3.zero;
				interactions.isDraggable=true;

			}
		}

	}

	void OnTriggerEnter(Collider other)
	{

		if (other.gameObject.tag=="plant") {
			Debug.Log ("Chomp");
			other.gameObject.SendMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
			Destroy (gameObject);
		}
	}

	void OnDestroy()
	{
		gManager.decrementPest ();
	}
}
