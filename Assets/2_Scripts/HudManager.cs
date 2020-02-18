using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HudManager : MonoBehaviour
{
	#region Variables Declaration

	[Header("Game Objects")]
	[SerializeField] private GameObject _hudFadeOutGO;
	[SerializeField] private GameObject _hudVictoryGO;
	[SerializeField] private GameObject _hudCreditGO;
	[SerializeField] private GameObject _hudInputGO;
	[SerializeField] private GameObject _hudSheetGO;
	[SerializeField] private GameObject _hudCrosshairGO;
	[SerializeField] private GameObject _menuPauseGO;
	[SerializeField] private GameObject _menuPauseEndGO;
	[SerializeField] private GameObject _menuChapterEndGO;
	[SerializeField] private GameObject _menuChapterGO;
	[SerializeField] private Slider _sliderVolume;
	[SerializeField] private Slider _sliderGamma;
	[SerializeField] private SheetsSO[] _sheetSOList;

	[Header("Sounds")]
	[SerializeField] private AudioClip _audioClipMouseClick;
	[SerializeField] private AudioClip _audioClipHoverButton;
	[SerializeField] private AudioClip _audioClipPause;

	public static bool _isFading;
	public static bool _isTheEnd;
	public static float _gameVolume = 0.5f;
	public static float _gameGamma = 0;
	public static bool _isGameOver;
	private TextMeshProUGUI _textComponent;
	private TextMeshProUGUI _textComponentEnd;
	private TextMeshProUGUI _textSheetComponent;
	private RawImage _sheetToRender;
	private AudioSource _audioSource;
	private DisplayInputTrigger[] _displayInputTriggersGO;
	private bool _isTriggerHuInputdActive;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_isTheEnd = false;
		_isGameOver = false;
		_isFading = false;
		_isTriggerHuInputdActive = false;
		_textComponent = _hudInputGO.GetComponent<TextMeshProUGUI>();
		_textComponentEnd = _hudVictoryGO.GetComponentInChildren<TextMeshProUGUI>();
		_textSheetComponent = _hudSheetGO.GetComponentInChildren<TextMeshProUGUI>();
		_sheetToRender = _hudSheetGO.GetComponentInChildren<RawImage>();
		_audioSource = GetComponent<AudioSource>();
		_sliderVolume.value = _gameVolume;
		_sliderGamma.value = _gameGamma;
	}

	private void Start()
	{
		EventSubscription();

		if (PlayerAbilities._isTheBeginning)
		{
			StartCoroutine(Fade(_hudFadeOutGO, true, 3f, 9f));
		}

		else
		{
			StartCoroutine(Fade(_hudFadeOutGO, true, 3f, 0f));
		}
	}

	private void Update()
	{
		if (PlayerAbilities._isEndGame && !_isTheEnd)
		{
			_isTheEnd = true;
			StartCoroutine(Fade(_hudFadeOutGO, false, 2f, 13f));
			StartCoroutine(EndingHud(16f, true));
		}

		if (_isGameOver && !_isTheEnd)
		{
			_isTheEnd = true;
			_textComponentEnd.text = "YOU DIED";
			StartCoroutine(EndingHud(1.39f));
		}

		_gameVolume = _sliderVolume.value;
		_gameGamma = _sliderGamma.value;
	}

	#endregion

	private void EventSubscription()
	{
		FindObjectOfType<PlayerAbilities>().OnHUDDisplay += HudManager_OnHUDDisplay;
		FindObjectOfType<InputManager>().OnPause += HudManager_OnPause;

		_displayInputTriggersGO = FindObjectsOfType<DisplayInputTrigger>();

		for (int i = 0; i < _displayInputTriggersGO.Length; i++)
		{
			_displayInputTriggersGO[i].OnHUDDisplay += HudManager_OnHUDDisplay1;
		}
	}

	private void HudManager_OnHUDDisplay1(object sender, DisplayInputTrigger.HUDDisplayInputEventArgs e)
	{
		_isTriggerHuInputdActive = e.isActive;
		_textComponent.enabled = e.isActive;
		_textComponent.text = e.textInputToDisplay;
	}

	private void HudManager_OnPause(object sender, InputManager.PauseEventArgs e)
	{
		if (!PlayerAbilities._isReading)
		{
			PlayerAbilities._isActionPlaying = e.isPaused;
		}

		Cursor.visible = e.isPaused;
		Cursor.lockState = e.isPaused ? CursorLockMode.Confined : CursorLockMode.Locked;
		_menuChapterGO.SetActive(false);
		_hudCrosshairGO.SetActive(!e.isPaused);
		_menuPauseGO.SetActive(e.isPaused);
		Pause(new PauseEventArgs { isPaused = e.isPaused });

		//_audioSource.clip = _audioClipPause;
		//_audioSource.Play();
	}

	public static IEnumerator Fade(GameObject hudGoFade, bool fadeOut, float duration, float delayTime = 0)
	{
		_isFading = true;
		float elapsedTime = 0;
		Image imageToFade = hudGoFade.GetComponent<Image>();

		hudGoFade.SetActive(!hudGoFade.activeSelf);

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

		_isFading = false;
	}

	private IEnumerator EndingHud(float delay = 0, bool isCreditVisible = false)
	{
		yield return new WaitForSeconds(delay);
		PostProcessManager._isPostProssessAttack = PostProcessManager._isPostProssessFall = PostProcessManager._isPostProssessHoldBreath = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
		_hudVictoryGO.SetActive(true);
		_hudCreditGO.SetActive(isCreditVisible);
	}

	public void OnClickButton(string buttonName)
	{
		switch (buttonName)
		{
			case "Resume":
				InputManager._isPaused = false;
				Cursor.visible = false;
				Pause(new PauseEventArgs { isPaused = false });

				if (!PlayerAbilities._isReading)
				{
					PlayerAbilities._isActionPlaying = false;
				}

				_hudCrosshairGO.SetActive(!_hudCrosshairGO.activeSelf);
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				break;
			case "Chapter":
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				_menuChapterGO.SetActive(!_menuChapterGO.activeSelf);
				break;
			case "ChapterEnd":
				_menuPauseEndGO.SetActive(!_menuPauseEndGO.activeSelf);
				_menuChapterEndGO.SetActive(!_menuChapterEndGO.activeSelf);
				break;
			case "ChapterGameover":
				_hudVictoryGO.SetActive(!_hudVictoryGO.activeSelf);
				_menuChapterGO.SetActive(!_menuChapterGO.activeSelf);
				break;
			case "Quit":
				Application.Quit();
				break;
			case "Back":
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				_menuChapterGO.SetActive(!_menuChapterGO.activeSelf);
				break;
			case "BackEnd":
				_menuPauseEndGO.SetActive(!_menuPauseEndGO.activeSelf);
				_menuChapterEndGO.SetActive(!_menuChapterEndGO.activeSelf);
				break;
			default:
				break;
		}

		//_audioSource.clip = _audioClipMouseClick;
		//_audioSource.Play();
	}

	private void HudManager_OnHUDDisplay(object sender, PlayerAbilities.HUDDisplayEventArgs e)
	{
		if (!_isTheEnd && !_isTriggerHuInputdActive)
		{
			_textComponent.enabled = e.isActive;
			_textComponent.text = e.isActive ? ConvertLayerIndexToInputName(e.layerDetected, e.isHiding) : "";

			if (e.isSheet)
			{
				_hudSheetGO.SetActive(!_hudSheetGO.activeSelf);
				_textSheetComponent.text = _sheetSOList[e.sheetID]._sheetText.Replace("\\n", "\n");
				_sheetToRender.texture = _sheetSOList[e.sheetID]._sheetSprite;
			}
		}
	}

	private string ConvertLayerIndexToInputName(int layerIndex, bool isHiding)
	{
		string text;

		switch (layerIndex)
		{
			case 10:
				text = "E : Activate";
				break;
			case 11:
				text = "E : Pick Up";
				break;
			case 12:
				text = "Space : Climb";
				break;
			case 13:
				text = "Space : Jump";
				break;
			case 16:
				text = isHiding ? "E : Exit" : "E : Hide";
				break;
			case 17:
				text = "Hold Left Click : Pull/Push \n Space : Climb";
				break;
			case 21:
				text = "E: Read";
				break;

			default:
				text = "";
				break;
		}

		return text;
	}

	#region Events

	public class PauseEventArgs : EventArgs
	{
		public bool isPaused;
	}

	public event EventHandler<PauseEventArgs> OnPause;

	private void Pause(PauseEventArgs e) => OnPause?.Invoke(this, e);

	#endregion
}
