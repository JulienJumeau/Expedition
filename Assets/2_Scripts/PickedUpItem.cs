using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedUpItem : MonoBehaviour
{
	public static int LightbulbNumber;
	[Range(1f, 5f)]
	public int LightbulbNumberMax = 2;
	public int OilLevel; //Between 0 and 1


	//if (_hitForward.transform != null)
	//{
	//	if (_hitForward.transform.tag == "Lightbulb")
	//	{
	//		if (LightbulbNumber < LightbulbNumberMax)
	//		{
	//			LightbulbNumber++;
	//			print("New Lightbulb picked up! You now have [" + LightbulbNumber + "] Lightbulbs in your inventory.");
	//			Destroy(gameObject);
	//		}
	//		else print("You reach the maximum number of Lightbulbs that you can carry in your inventory.");
	//	}
	//	if (_hitForward.transform.tag == "Oil")
	//	{
	//		OilLevel = 1;
	//		print("Your oil level is now full!");
	//		Destroy(gameObject);
	//	}
	//}

	/* Drag Objects
	if(Input.GetKey(KeyCode.E)
				_hitForward.transform.Translate(motion);
	if (Input.GetKeyUp(KeyCode.E)
	 * 
	 */

	private void Start()
	{
		LightbulbNumber = 0;
		OilLevel = 0;
	}
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (gameObject.tag == "Lightbulb")
			{
				if (LightbulbNumber < LightbulbNumberMax)
				{
					LightbulbNumber++;
					print("New Lightbulb picked up! You now have [" + LightbulbNumber + "] Lightbulbs in your inventory.");
					Destroy(gameObject);
				}
				else print("You reach the maximum number of Lightbulbs that you can carry in your inventory.");
			}
			if (gameObject.tag == "Oil")
			{
				OilLevel = 1;
				print("Your oil level is now full!");
				Destroy(gameObject);
			}
		}
	}

}
