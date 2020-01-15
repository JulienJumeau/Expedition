using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedUpItem : MonoBehaviour
{
	//Collect Ressources
	private void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			print(gameObject.name + " is picked up!");
			//Variable Lightbulb ++
			Destroy(gameObject);

		}
	}

}
