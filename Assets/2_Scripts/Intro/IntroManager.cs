using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
	#region Variables Declaration

	[SerializeField] private VideoPlayer _videoPlayer;
	[SerializeField] private GameObject _hudSkipIntroGO;

	#endregion

	#region Unity Methods

	private void Start()
	{
		StartCoroutine(LoadGameAfterIntro());
		StartCoroutine(PlayVideo());
	}

	private void Update()
	{
		if (Input.GetButtonDown("JumpClimb"))
		{
			if (!_hudSkipIntroGO.activeSelf)
			{
				_hudSkipIntroGO.SetActive(true);
			}

			else
			{
				SceneManager.LoadScene("SceneLoader");
			}
		}
	}

	#endregion

	private IEnumerator PlayVideo()
	{
		_videoPlayer.Prepare();

		while (!_videoPlayer.isPrepared)
		{
			yield return new WaitForSeconds(1);
		}

		_videoPlayer.Play();
	}

	private IEnumerator LoadGameAfterIntro()
	{
		yield return new WaitForSeconds((float)_videoPlayer.clip.length - 2);
		_videoPlayer.SetDirectAudioMute(0, true);
		SceneManager.LoadScene("SceneLoader");
	}
}
