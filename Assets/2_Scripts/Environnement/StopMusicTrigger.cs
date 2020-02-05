using System;
using UnityEngine;

public class StopMusicTrigger : MonoBehaviour
{
	#region Variables declaration

	private BoxCollider _collider;
	public int _index;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		StopMusicTriggered();
		_collider.enabled = false;
	}

	#endregion

	#region Events

	public event EventHandler OnStopMusicTriggered;

	private void StopMusicTriggered() => OnStopMusicTriggered?.Invoke(this, EventArgs.Empty);

	#endregion
}
