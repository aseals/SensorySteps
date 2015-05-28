using UnityEngine;
using System.Collections;

public class CloudWand : MonoBehaviour {
	// Use this for initialization
	InteractiveObject interactions;
	bool downTime= false;
	// Use this for initialization
	Vector3 lastPosition;
	public float smoothFactor;
	void Start () 
	{	
		interactions = GetComponent<InteractiveObject> ();
		gameObject.GetComponent<ParticleSystem>().Stop ();
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		transform.position =Vector3.Lerp (lastPosition, interactions.getCursorPosition(),  smoothFactor * Time.deltaTime);
		lastPosition = transform.position;
	}
	
	void sparkle()
	{
		gameObject.GetComponent<ParticleSystem> ().Play ();
		Invoke ("stopSparkle", 1);
	}
	void stopSparkle()
	{
		gameObject.GetComponent<ParticleSystem> ().Stop ();
	}
	void OnDestroy()
	{
		Debug.Log (interactions.getCursorPosition());

	}
}
