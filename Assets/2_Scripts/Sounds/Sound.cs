using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string _name;
	public AudioClip _audioClip;

	[Range(0f, 1f)]
	public float _volume;

	public bool loop;

	[HideInInspector]
	public AudioSource _audioSource;

}
