using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class MusicTrigger : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private AudioClip _music = null;
	private BoxCollider _collider;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		MusicTriggered(new MusicTriggeredEventArgs { musicTriggered = _music, id = this.transform.GetSiblingIndex() });
		_collider.enabled = false;
	}

	#endregion

	#region Events

	public class MusicTriggeredEventArgs : EventArgs
	{
		public AudioClip musicTriggered;
		public int id;
	}

	public event EventHandler<MusicTriggeredEventArgs> OnMusicTriggered;

	private void MusicTriggered(MusicTriggeredEventArgs e) => OnMusicTriggered?.Invoke(this, e);

	#endregion
}
