using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
	#region Unity Methods

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Start()
	{
		InputManager._current.OnCameraMove += CameraController_OnCameraInputPressed;
	}

	#endregion

	#region Custom Methods

	private void CameraController_OnCameraInputPressed(object sender, InputManager.CameraMoveEventArgs e)
	{
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate(-e.rotationVector.x, e.rotationVector.y, 0);
	}

	#endregion
}
