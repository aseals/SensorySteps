using UnityEngine;
using System.Collections;
/// <summary>
/// translates the camera's view into the coordinates of the unity scene, then manipulates objects.
/// </summary>
public class GrabDropScript : MonoBehaviour 
{
	public GameObject[] draggableObjects;
	public float dragSpeed = 3.0f;
	public Material selectedObjectMaterial;
	
	public GUIText infoGuiText;
	
	private InteractionManager manager;
	private bool isLeftHandDrag;

	private GameObject draggedObject;
	private float draggedObjectDepth;
	private Vector3 draggedObjectOffset;
	private Material draggedObjectMaterial;
	

	void Update() 
	{
		// get the interaction manager instance
		if(manager == null)
		{
			manager = InteractionManager.Instance;
		}

		if(manager != null && manager.IsInteractionInited())
		{
			Vector3 screenNormalPos = Vector3.zero;
			Vector3 screenPixelPos = Vector3.zero;
			
			if(draggedObject == null)
			{
				// if there is a hand grip, select the underlying object and start dragging it.
				if(manager.IsLeftHandPrimary())
				{
					// if the left hand is primary, check for left hand grip
					if(manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Grip)
					{
						isLeftHandDrag = true;
						screenNormalPos = manager.GetLeftHandScreenPos();
					}
				}
				else if(manager.IsRightHandPrimary())
				{
					// if the right hand is primary, check for right hand grip
					if(manager.GetLastRightHandEvent() == InteractionManager.HandEventType.Grip)
					{
						isLeftHandDrag = false;
						screenNormalPos = manager.GetRightHandScreenPos();
					}
				}
				
				// check if there is an underlying object to be selected
				if(screenNormalPos != Vector3.zero)
				{
					// convert the normalized screen pos to pixel pos
					screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
					screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
					Ray ray = Camera.main.ScreenPointToRay(screenPixelPos);
					
					// check for underlying objects
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit))
					{
						foreach(GameObject obj in draggableObjects)
						{
							if(hit.collider.gameObject == obj)
							{
								// an object was hit by the ray. select it and start drgging
								draggedObject = obj;
								draggedObjectDepth = draggedObject.transform.position.z - Camera.main.transform.position.z;
								draggedObjectOffset = hit.point - draggedObject.transform.position;
								
								// set selection material
								draggedObjectMaterial = draggedObject.GetComponent<Renderer>().material;
								draggedObject.GetComponent<Renderer>().material = selectedObjectMaterial;
								break;
							}
						}
					}
				}
				
			}
			else
			{
				// continue dragging the object
				screenNormalPos = isLeftHandDrag ? manager.GetLeftHandScreenPos() : manager.GetRightHandScreenPos();
				
				// convert the normalized screen pos to 3D-world pos
				screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
				screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
				screenPixelPos.z = screenNormalPos.z+draggedObjectDepth  ;
				
				Vector3 newObjectPos = Camera.main.ScreenToWorldPoint(screenPixelPos) - draggedObjectOffset;
				draggedObject.transform.position = Vector3.Lerp(draggedObject.transform.position, newObjectPos, dragSpeed * Time.deltaTime);
				
				// check if the object (hand grip) was released
				//Debug.Log ( Camera.main.ScreenToWorldPoint(screenPixelPos));
				bool isReleased = isLeftHandDrag ? (manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Release) :
					(manager.GetLastRightHandEvent() == InteractionManager.HandEventType.Release);
				
				if(isReleased)
				{
					// restore the object's material and stop dragging the object
					draggedObject.GetComponent<Renderer>().material = draggedObjectMaterial;
					draggedObject = null;
				}
			}
			//Debug.Log (screenNormalPos);
		}

	}
	
	void OnGUI()
	{
		if(infoGuiText != null && manager != null && manager.IsInteractionInited())
		{
			string sInfo = string.Empty;
			
			long userID = manager.GetUserID();
			if(userID != 0)
			{
				if(draggedObject != null)
					sInfo = "Dragging the " + draggedObject.name + " around.";
				else
					sInfo = "Drag Something";
			}
			else
			{
				KinectManager kinectManager = KinectManager.Instance;

				if(kinectManager && kinectManager.IsInitialized())
				{
					sInfo = "Waiting for Users...";
				}
				else
				{
					sInfo = "Kinect is not initialized. Check the log for details.";
				}
			}
			
			infoGuiText.GetComponent<GUIText>().text = sInfo;
		}
	}
	
}
