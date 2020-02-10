using UnityEngine.Audio;
using System;
using UnityEngine;

public class SonManager : MonoBehaviour
{
	public Sound[] _sounds;

	public static SonManager _instance;
	private void Awake()
	{
		//Pour éviter plusieurs instances du SonManager
		if (_instance == null)
			_instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}

		//Pour gestion du son à travers plusieurs scenes sans reset
		DontDestroyOnLoad(gameObject);

		foreach (Sound sound in _sounds)
		{
			sound._audioSource = gameObject.AddComponent<AudioSource>();
			sound._audioSource.clip = sound._audioClip;
			sound._audioSource.volume = sound._volume;
			sound._audioSource.loop = sound.loop;
		}
	}

	public void Play (string name)
	{
		Sound s = Array.Find(_sounds, sound => sound._name == name);

		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s._audioSource.Play();
	}
}
