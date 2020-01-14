using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(BoxCollider))]
public sealed class SfxTrigger : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] private AudioClip _sfx;
	private AudioSource _audioSource;
	private BoxCollider _collider;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider _)
	{
		_audioSource.PlayOneShot(_sfx);
		_collider.enabled = false;
	}

	#endregion
}
