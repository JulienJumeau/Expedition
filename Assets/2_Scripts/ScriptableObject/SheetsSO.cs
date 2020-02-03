using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sheets", order = 1)]
public class SheetsSO : ScriptableObject
{
	#region Variables

	public int _sheetID;
	public Texture _sheetSprite;
	public string _sheetText;

    #endregion
}
