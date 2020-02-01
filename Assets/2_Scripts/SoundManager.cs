using System.Linq;
using UnityEngine;

public sealed class SoundManager : MonoBehaviour
{
	#region Variables declaration

	private readonly string[] _audioSourceName = new string[] { "Audio_Music", "Audio_Voice" };
	private AudioSource[] _audioSources;
	private MusicTrigger[] _musicGo;

	#endregion

	#region UnityMethods

	private void Awake()
	{
		_audioSources = new AudioSource[_audioSourceName.Length];

		for (int i = 0; i < _audioSourceName.Length; i++)
		{
			GameObject go = new GameObject(_audioSourceName[i]);
			go.transform.parent = this.gameObject.transform;
			_audioSources[i] = go.AddComponent<AudioSource>();
			_audioSources[i].loop = true;
		}
	}

	private void Start()
	{
		EventSubscription();
	}

	#endregion

	#region CustomMethods

	private void SoundManager_OnMusicTriggered(object sender, MusicTrigger.MusicTriggeredEventArgs e)
	{
		Debug.Log(e.id);
		_musicGo[e.id].OnMusicTriggered -= SoundManager_OnMusicTriggered;
		_audioSources[0].clip = e.musicTriggered;
		_audioSources[0].Play();
	}

	private void EventSubscription()
	{
		_musicGo = FindObjectsOfType<MusicTrigger>().OrderBy(m => m.transform.GetSiblingIndex()).ToArray();
		for (int i = 0; i < _musicGo.Length; i++)
		{
			_musicGo[i].OnMusicTriggered += SoundManager_OnMusicTriggered;
		}
	}

	#endregion
}
