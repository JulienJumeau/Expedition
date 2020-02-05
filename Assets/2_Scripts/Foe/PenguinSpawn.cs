using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinSpawn : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private GameObject _penguinToSpawn = null;
	private BoxCollider _collider;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		Debug.Log("Triggered");
		_penguinToSpawn.SetActive(true);
		_collider.enabled = false;
	}

	#endregion
}
