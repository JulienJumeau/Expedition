using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
	#region Variables declaration

	private float _xAxisClamp = 0;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		FindObjectOfType<InputManager>().OnCameraMove += CameraController_OnCameraInputPressed;
	}

	#endregion

	#region Custom Methods

	private void CameraController_OnCameraInputPressed(object sender, InputManager.CameraMoveEventArgs e)
	{
		_xAxisClamp -= e.rotationVector.x;

		Vector3 rotation = this.transform.rotation.eulerAngles;
		rotation.x += -e.rotationVector.x;

		if (_xAxisClamp > 90)
		{
			_xAxisClamp = 90;
			rotation.x = 90;
		}

		else if (_xAxisClamp < -90)
		{
			_xAxisClamp = -90;
			rotation.x = 270;
		}

		this.transform.rotation = Quaternion.Euler(rotation);
	}

	#endregion
}
