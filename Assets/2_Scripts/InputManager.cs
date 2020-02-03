using System;
using UnityEngine;

public enum InputAction
{
	Idle,
	Use,
	Lantern,
	Crouch,
	Walk,
	Run,
	Jump,
	Pull,
	PhotoCamera,
	HoldBreath,
	StopHoldingBreath
}

public sealed class InputManager : MonoBehaviour
{
	#region Variables declaration

	[Range(1, 10)] [SerializeField] private float _initialCameraSensitivity = 3;
	private float _cameraSensitivity;
	private float _verticalRot, _horizontalRot;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		#region Variables initialization

		_cameraSensitivity = _initialCameraSensitivity;
		_verticalRot = _horizontalRot = 0f;

		#endregion
	}

	private void Update()
	{
		#region MovementInput

		if (!PlayerAbilities._isActionPlaying)
		{
			float strafeInput = Input.GetAxisRaw("Strafing"), forwardInput = Input.GetAxisRaw("MoveForward");
			_horizontalRot = Input.GetAxisRaw("Rotation Camera X") * _cameraSensitivity;
			_verticalRot = Input.GetAxisRaw("Rotation Camera Y") * _cameraSensitivity;

			if (strafeInput != 0 || forwardInput != 0)
			{
				DirectionInputPressed(new DirectionInputPressedEventArgs() { directionVector = new Vector3(strafeInput, 0, forwardInput) });

				if (Input.GetButton("Running"))
				{
					ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Run });
				}

				else if (Input.GetButton("Pulling"))
				{
					_cameraSensitivity = 0;
					ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Pull });
				}

				else
				{
					_cameraSensitivity = _initialCameraSensitivity;
					ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Walk });
				}
			}

			else
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Idle });
			}

			if (_horizontalRot != 0 || _verticalRot != 0)
			{
				CameraMove(new CameraMoveEventArgs() { rotationVector = new Vector3(_verticalRot, _horizontalRot, 0) });
			}

			#endregion

			#region ActionInput


			if (Input.GetButtonDown("Lantern"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Lantern });
			}

			if (Input.GetButtonDown("Crouching"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Crouch });
			}

			if (Input.GetButtonDown("JumpClimb"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Jump });
			}

			if (Input.GetButtonDown("PhotoCamera"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.PhotoCamera });
			}

			if (Input.GetButton("HoldBreath"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.HoldBreath });
			}

			if (Input.GetButtonUp("HoldBreath"))
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.StopHoldingBreath });
			}

			if (!Input.anyKey)
			{
				ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Idle });
			}
		}

		if (Input.GetButtonDown("ActionButton"))
		{
			ActionInputPressed(new ActionInputPressedEventArgs { actionPressed = InputAction.Use });
		}

		#endregion
	}

	#endregion

	#region Events

	// Event to trigger when an direction input is pressed

	public class DirectionInputPressedEventArgs : EventArgs
	{
		public Vector3 directionVector;
	}

	public event EventHandler<DirectionInputPressedEventArgs> OnDirectionInputPressed;

	private void DirectionInputPressed(DirectionInputPressedEventArgs e) => OnDirectionInputPressed?.Invoke(this, e);

	// Event to trigger when an action input is pressed

	public class ActionInputPressedEventArgs : EventArgs
	{
		public InputAction actionPressed;
	}

	public event EventHandler<ActionInputPressedEventArgs> OnActionInputPressed;

	private void ActionInputPressed(ActionInputPressedEventArgs e) => OnActionInputPressed?.Invoke(this, e);

	// Event to trigger when the mouse/joysticks is/are moved

	public class CameraMoveEventArgs : EventArgs
	{
		public Vector3 rotationVector;
	}

	public event EventHandler<CameraMoveEventArgs> OnCameraMove;

	private void CameraMove(CameraMoveEventArgs e) => OnCameraMove?.Invoke(this, e);

	#endregion
}