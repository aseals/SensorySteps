using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractiveObject))]
public class Cloud : MonoBehaviour {



	Collider cloudCollider;
	InteractiveObject cloudInteractions;
	public Transform pointA;
	public Transform pointB;
	public float speed;
	float alongMove;
	int direction;

	void Avake()
	{
		cloudCollider = gameObject.GetComponent<Collider>();
		alongMove = 0f;
		direction = 1;
		cloudInteractions = GetComponent<InteractiveObject>();
		Debug.Log (gameObject.GetComponent<InteractiveObject>());
	}

	void Update()
	{	
	/*	alongMove += direction* speed * Time.deltaTime;
		gameObject.transform.position = Vector3.Lerp (pointA.position, pointB.position, alongMove);
		if (alongMove <= 0 || alongMove >= 1)
			direction = direction * -1;
*/
		if(cloudInteractions.getIsTouched())
		{
			gameObject.GetComponent<ParticleSystem>().Play();

		}
	}

	void OnTriggerStay(Collider other)
	{
		Debug.Log("cloudgothit");
		other.gameObject.SendMessage("sparkle", SendMessageOptions.DontRequireReceiver);
		//fire raindrop???
		gameObject.GetComponent<ParticleSystem>().Play();
	}
}
