using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
	#region Variables Declaration

	[SerializeField] private GameObject _hudInputGO;
	private TextMeshProUGUI _textComponent;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_textComponent = _hudInputGO.GetComponent<TextMeshProUGUI>();
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
		Debug.Log(e.isActive + " " + e.layerDetected);
		_textComponent.text = e.isActive ? ConvertLayerIndexToInputName(e.layerDetected) : "";
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
				text = "Left Click : Pull/Push \n Space : Climb";
				break;

			default:
				text = "";
				break;
		}

		return text;
	}
}
