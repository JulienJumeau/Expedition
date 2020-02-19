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
	[SerializeField] private GameObject _hudFadeGO;
	[SerializeField] private bool _isMainMenuVideo;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		if (!_isMainMenuVideo)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void Start()
	{
		if (!_isMainMenuVideo)
		{
			StartCoroutine(Fade(_hudFadeGO, true, 3f, 2f));
			StartCoroutine(LoadGameAfterIntro());
		}

		else
		{
			StartCoroutine(Fade(_hudFadeGO, true, 3f, 2f));
			_videoPlayer.isLooping = true;
		}

		StartCoroutine(PlayVideo());
	}

	private void Update()
	{
		if (Input.GetButtonDown("JumpClimb") && !_isMainMenuVideo)
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

	public void OnClick() => StartCoroutine(VideoStop());

	private IEnumerator VideoStop()
	{
		StartCoroutine(HudManager.Fade(_hudFadeGO, false, 2f));
		yield return new WaitForSeconds(2);
	}

	private IEnumerator LoadGameAfterIntro()
	{
		yield return new WaitForSeconds((float)_videoPlayer.clip.length - 2);
		_videoPlayer.SetDirectAudioMute(0, true);
		SceneManager.LoadScene("SceneLoader");
	}

	private IEnumerator SkipIntro(float duration)
	{
		yield return StartCoroutine(FadeVideoTexture(duration));
		SceneManager.LoadScene("SceneLoader");
	}

	private IEnumerator FadeVideoTexture(float durattion)
	{
		float elapsedTime = 0;

		while (elapsedTime <= 2)
		{
			elapsedTime += Time.deltaTime;
			float alphaColor = Mathf.Lerp(1, 0, elapsedTime / 2);
			float musicVolume = alphaColor;
			_rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, alphaColor);
			_videoPlayer.SetDirectAudioVolume(0, musicVolume);
			yield return null;
		}
	}

	private static IEnumerator Fade(GameObject hudGoFade, bool fadeOut, float duration, float delayTime = 0)
	{
		float elapsedTime = 0;
		Image imageToFade = hudGoFade.GetComponent<Image>();

		yield return new WaitForSeconds(delayTime);

		while (elapsedTime <= duration)
		{
			elapsedTime += Time.deltaTime;
			float alphaColor = fadeOut ? Mathf.Lerp(1, 0, elapsedTime / duration) : Mathf.Lerp(0, 1, elapsedTime / duration);
			imageToFade.color = new Color(0, 0, 0, alphaColor);
			yield return null;
		}

		if (fadeOut)
		{
			hudGoFade.SetActive(!hudGoFade.activeSelf);
		}
	}
}
