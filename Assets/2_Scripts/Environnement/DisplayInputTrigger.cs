using System;
using UnityEngine;

public sealed class DisplayInputTrigger : MonoBehaviour
{
	#region Variables Declaration

	[SerializeField] private string _textInputToDisplay;

	#endregion

	#region Unity Methods

	private void OnTriggerEnter(Collider other)
	{
		HUDDisplay(new HUDDisplayInputEventArgs { isActive = true, textInputToDisplay = _textInputToDisplay });
	}

	private void OnTriggerExit(Collider other)
	{
		HUDDisplay(new HUDDisplayInputEventArgs { isActive = false, textInputToDisplay = "" });
	}

	#endregion

	public class HUDDisplayInputEventArgs : EventArgs
	{
		public bool isActive;
		public string textInputToDisplay;
	}

	public event EventHandler<HUDDisplayInputEventArgs> OnHUDDisplay;

	public void HUDDisplay(HUDDisplayInputEventArgs e) => OnHUDDisplay?.Invoke(this, e);
}
