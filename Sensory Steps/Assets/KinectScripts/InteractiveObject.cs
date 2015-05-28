using UnityEngine;
using System.Collections;
/// <summary>
/// An object that just tells if the cursor is ontop of it and then does something
/// requires a box collider to see if cursor is ontop and or grabbing it
/// currently all z is set to 0.
/// This object only manages Interactions with Kinect.
/// Other scripts are needed to manage the inter-object behaviours.
/// </summary>

public class InteractiveObject : MonoBehaviour {
	//public variables
	public bool isDraggable;
	public Vector3 dragOffset;
	public float dragSpeed;

	InteractionManager manager;
	bool isTouched=false;
	bool isGrabbed= false;
	bool isLeftHand= false;
	Vector3 cursorCameraPosition = Vector3.zero;
	Vector3 cursorScreenPosition = Vector3.zero;
	Vector3 worldCursorScreenPosition= Vector3.zero;
	float depth;
	GameObject someObject;
	Collider box;
	Collider2D box2D;
	GameManager gManager;
	GameObject anObject;
	// Use this for initialization
	void Start () {

		//anObject=GameObject.Find("GameManager");
		if(manager==null)
		manager = InteractionManager.Instance;
		if (GetComponent<Collider> () == null) {

			box2D = GetComponent<Collider2D> ();
		} else
			box = GetComponent<Collider> ();
		depth = -Camera.main.transform.position.z  + transform.position.z;

		if (gManager == null)
			gManager = GameManager.Instance();
	}
	
	// Update is called once per frame

	void Update () 
	{	
		//Debug.Log (isGrabbed);
		if(manager==null)
			manager = InteractionManager.Instance;
		if (gManager == null)
			gManager = GameManager.Instance();


			if (manager.IsLeftHandPrimary ()) {
				// if the left hand is primary, check for left hand grip

				cursorCameraPosition = manager.GetLeftHandScreenPos ();
				cursorCameraPosition.z = 0;
				isLeftHand = true;

			} else if (manager.IsRightHandPrimary ()) {	
				cursorCameraPosition = manager.GetRightHandScreenPos ();
				cursorCameraPosition.z = 0;
				isLeftHand = false;
			
			}
			//translate the camera position returned by the Kinect into coordinates in the scene
			if (cursorCameraPosition != Vector3.zero) {
				cursorScreenPosition.x = (int)(cursorCameraPosition.x * Camera.main.pixelWidth);
				cursorScreenPosition.y = (int)(cursorCameraPosition.y * Camera.main.pixelHeight);
				cursorScreenPosition.z = depth;
				worldCursorScreenPosition = Camera.main.ScreenToWorldPoint (cursorScreenPosition);
				worldCursorScreenPosition.z = 0;
				//Debug.Log (worldCursorScreenPosition);

				//someObject is for debugging purposes only
				//if(someObject==null)
				//	someObject=GameObject.CreatePrimitive(PrimitiveType.Sphere);
				//else
				//someObject.transform.position=Camera.main.ScreenToWorldPoint(cursorScreenPosition);

			}


			//Debug.Log (gManager.ToString()+" "+gManager.isHandGrabbing());
			//check if the box is being touched and is grabbed or not.
			if (box != null) {
				if (box.bounds.Contains (worldCursorScreenPosition) && worldCursorScreenPosition != Vector3.zero && !gManager.isHandGrabbing () && box) {
					//Debug.Log (this.name+ " i am touch?"  + isTouched);
					//Debug.Log (this.name+ " i am grab?"  + isGrabbed);
					isTouched = true;
					if (manager.GetLastLeftHandEvent () == InteractionManager.HandEventType.Grip) {
						isGrabbed = true;
						isLeftHand = true;
						Debug.Log (gameObject.name + " I got Grabbed");
						if (isDraggable)
							gManager.handGrabs ();
					}
					if (manager.GetLastRightHandEvent () == InteractionManager.HandEventType.Grip) {
						isGrabbed = true;
						isLeftHand = false;
						Debug.Log (gameObject.name + " I got Grabbed");
						if (isDraggable)
							gManager.handGrabs ();
					}


				}
			}
			if (box2D != null) {
				//same as above, except with support for 2d collider
				if (box2D.bounds.Contains (worldCursorScreenPosition) && worldCursorScreenPosition != Vector3.zero && !gManager.isHandGrabbing ()) {
					isTouched = true;
					if (manager.GetLastLeftHandEvent () == InteractionManager.HandEventType.Grip) {
						isGrabbed = true;
						isLeftHand = true;
						Debug.Log (gameObject.name + " I got Grabbed");
						if (isDraggable)
							gManager.handGrabs ();
					}
					if (manager.GetLastRightHandEvent () == InteractionManager.HandEventType.Grip) {
						isGrabbed = true;
						isLeftHand = false;
						Debug.Log (gameObject.name + " I got Grabbed");
						if (isDraggable)
							gManager.handGrabs ();
					}
				}
			}
			/*if (gManager.isHandGrabbing ()) {
			if(manager.GetLastLeftHandEvent()== InteractionManager.HandEventType.Release||manager.GetLastRightHandEvent()== InteractionManager.HandEventType.Release)
				gManager.handLoose();
		}
*/
			if (isDraggable && isGrabbed) {
				//cursorCameraPosition = isLeftHand ? manager.GetLeftHandScreenPos() : manager.GetRightHandScreenPos();
				cursorCameraPosition = manager.GetCursorPosition ();
				// convert the normalized screen pos to 3D-world pos
				cursorScreenPosition.x = (int)(cursorCameraPosition.x * Camera.main.pixelWidth);
				cursorScreenPosition.y = (int)(cursorCameraPosition.y * Camera.main.pixelHeight);
				cursorScreenPosition.z = depth;
				worldCursorScreenPosition = Camera.main.ScreenToWorldPoint (cursorScreenPosition);
				worldCursorScreenPosition.z = 0;
				Vector3 newObjectPos = worldCursorScreenPosition - dragOffset;
				transform.position = Vector3.Lerp (transform.position, newObjectPos, dragSpeed * Time.deltaTime);


			}
		
			//set it to drop the box when a hand loosens, regardless of position.
			if (isGrabbed) {
				if (isLeftHand && manager.GetLastLeftHandEvent () == InteractionManager.HandEventType.Release) {

					isLeftHand = false;
					isGrabbed = false;
					if (isDraggable)
						gManager.handLoose ();
					//break;
				} else if (!isLeftHand && manager.GetLastRightHandEvent () == InteractionManager.HandEventType.Release) {
					isGrabbed = false;
					if (isDraggable)
						gManager.handLoose ();
				}

			}

		}


	public bool getIsTouched()
	{
		return isTouched;

	}

	public bool getIsGrabbed()
	{
		return isGrabbed;
		
	}

	public void setGrab()
	{
		isGrabbed = true;
		isTouched = true;
	}

	public Vector3 getCursorPosition()
	{
		return worldCursorScreenPosition;

	}

	public void OnDestroy()
	{
		gManager.handLoose ();

	}
}
