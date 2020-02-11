using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
	#region Variables Declaration

	[SerializeField] private VideoPlayer _videoPlayer;
	[SerializeField] private RawImage _rawImage;
	[SerializeField] private GameObject _hudSkipIntroGO;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

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
				_hudSkipIntroGO.SetActive(false);
				StartCoroutine(SkipIntro(2f));
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

	private IEnumerator SkipIntro(float duration)
	{
		float elapsedTime = 0;

		while (elapsedTime <= duration)
		{
			elapsedTime += Time.deltaTime;
			float alphaColor = Mathf.Lerp(1, 0, elapsedTime / duration);
			float musicVolume = alphaColor;
			_rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, alphaColor);
			_videoPlayer.SetDirectAudioVolume(0, musicVolume);
			yield return null;
		}

		SceneManager.LoadScene("SceneLoader");
	}
}
