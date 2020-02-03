using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour
{
	#region Variables Declaration

	[SerializeField] private GameObject _hudInputGO;
	[SerializeField] private GameObject _hudSheetGO;
	[SerializeField] private SheetsSO[] _sheetSOList;
	private TextMeshProUGUI _textComponent;
	private TextMeshProUGUI _textSheetComponent;
	private RawImage _sheetToRender;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_textComponent = _hudInputGO.GetComponent<TextMeshProUGUI>();
		_textSheetComponent = _hudSheetGO.GetComponentInChildren<TextMeshProUGUI>();
		_sheetToRender = _hudSheetGO.GetComponentInChildren<RawImage>();
		EventSubscription();
	}

	#endregion

	private void EventSubscription()
	{
		FindObjectOfType<PlayerAbilities>().OnHUDDisplay += HudManager_OnHUDDisplay;
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
