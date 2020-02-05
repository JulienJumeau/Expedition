using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour
{
	#region Variables Declaration

	[Header("Game Objects")]
	[SerializeField] private GameObject _hudInputGO;
	[SerializeField] private GameObject _hudSheetGO;
	[SerializeField] private GameObject _hudCrosshairGO;
	[SerializeField] private GameObject _menuPauseGO;
	[SerializeField] private GameObject _menuChapterGO;
	[SerializeField] private SheetsSO[] _sheetSOList;

	[Header("Sounds")]
	[SerializeField] private AudioClip _audioClipMouseClick;
	[SerializeField] private AudioClip _audioClipHoverButton;
	[SerializeField] private AudioClip _audioClipPause;

	private TextMeshProUGUI _textComponent;
	private TextMeshProUGUI _textSheetComponent;
	private RawImage _sheetToRender;
	private AudioSource _audioSource;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_textComponent = _hudInputGO.GetComponent<TextMeshProUGUI>();
		_textSheetComponent = _hudSheetGO.GetComponentInChildren<TextMeshProUGUI>();
		_sheetToRender = _hudSheetGO.GetComponentInChildren<RawImage>();
		_audioSource = GetComponent<AudioSource>();
		EventSubscription();
	}

	#endregion

	private void EventSubscription()
	{
		FindObjectOfType<PlayerAbilities>().OnHUDDisplay += HudManager_OnHUDDisplay;
		FindObjectOfType<InputManager>().OnPause += HudManager_OnPause;
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

		_audioSource.clip = _audioClipPause;
		_audioSource.Play();
	}

	public void OnClickButton(string buttonName)
	{
		switch (buttonName)
		{
			case "Resume":
				InputManager._isPaused = false; 
				
				if (!PlayerAbilities._isReading)
				{
					PlayerAbilities._isActionPlaying = false;
				}

				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				break;
			case "Chapter":
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				_menuChapterGO.SetActive(!_menuChapterGO.activeSelf);
				break;
			case "Quit":
				Application.Quit();
				break;
			case "Back":
				_menuPauseGO.SetActive(!_menuPauseGO.activeSelf);
				_menuChapterGO.SetActive(!_menuChapterGO.activeSelf);
				break;
			default:
				break;
		}

		_audioSource.clip = _audioClipMouseClick;
		_audioSource.Play();
	}

	private void HudManager_OnHUDDisplay(object sender, PlayerAbilities.HUDDisplayEventArgs e)
	{
		_textComponent.enabled = e.isActive;
		_textComponent.text = e.isActive ? ConvertLayerIndexToInputName(e.layerDetected) : "";

		if (e.isSheet)
		{
			_hudSheetGO.SetActive(!_hudSheetGO.activeSelf);
			_textSheetComponent.text = _sheetSOList[e.sheetID]._sheetText.Replace("\\n", "\n");
			_sheetToRender.texture = _sheetSOList[e.sheetID]._sheetSprite;
		}
	}

	private string ConvertLayerIndexToInputName(int layerIndex)
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
				text = "E : Hide";
				break;
			case 17:
				text = "Hold Left Click : Pull/Push \n Space : Climb";
				break;
			case 21:
				text = "E: Readable";
				break;

			default:
				text = "";
				break;
		}

		return text;
	}
}
