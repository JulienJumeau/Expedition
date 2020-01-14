using System.Collections;
using UnityEngine;

public sealed class PlayerAbilities : MonoBehaviour
{
	#region Variables declaration

	[Range(10, 20)] [SerializeField] private float _speed;
	[SerializeField] private LayerMask _layerMask;

	private Camera _camera;
	private CharacterController _characterController;
	private Light _flashlight;
	private Vector3 _motionForward, _motionStrafe, _direction;
	private RaycastHit _hitForward, _hitTopFront, _hitTopBack;
	private float _characterInitialHeight;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		#region Variables initialization

		_camera = Camera.main;
		_characterController = GetComponent<CharacterController>();
		_characterInitialHeight = _characterController.height;
		_flashlight = GameObject.FindWithTag("FlashLight").GetComponent<Light>();

		#endregion

		#region Event Subcription

		EventSubscription();

		#endregion
	}

	private void Update()
	{
		// Call movement method each frame
		Movement();

		// Check if the player is aiming a GameObject with a specific layermask ( raycast beside camera forward ) 
		if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitForward, 3f, _layerMask))
		{

		}
	}

	#endregion

	#region Custom Methods

	#region Events Subscription

	private void EventSubscription()
	{
		InputManager._current.OnDirectionInputPressed += PlayerAbilities_OnDirectionInputPressed;
		InputManager._current.OnCameraMove += PlayerAbilities_OnCameraInputPressed;
		InputManager._current.OnActionInputPressed += PlayerAbilities_OnActionButtonPressed;
	}

	#endregion

	#region Player Movements

	private void Movement()
	{
		// Reset the motion and increment it beside moves initialized in direcetion event
		Vector3 motion = Vector3.zero;
		motion += (_motionForward + _motionStrafe).normalized * _speed;

		// Check if the character is grounded, if not add the gravity
		if (_characterController.isGrounded)
		{
			motion.y = 0;
		}

		else
		{
			motion += this.transform.up * Physics.gravity.y;
		}

		// Call the native method Move from character controller and send all moves to it
		_characterController.Move(motion * Time.deltaTime);
		_motionForward = Vector3.zero;
		_motionStrafe = Vector3.zero;
	}

	// Method called when an input direction event is triggered, receive args for direction variable used in movement
	private void PlayerAbilities_OnDirectionInputPressed(object sender, InputManager.DirectionInputPressedEventArgs e)
	{
		_motionForward = this.transform.forward * e.directionVector.z;
		_motionStrafe = this.transform.right * e.directionVector.x;
		_direction = (_motionForward + _motionStrafe).normalized;
	}

	// Method called when a camera rotation event is triggered, directly rotate the character beside rotation args
	private void PlayerAbilities_OnCameraInputPressed(object sender, InputManager.CameraMoveEventArgs e)
	{
		this.transform.rotation = Quaternion.identity;
		this.transform.Rotate(new Vector3(0, e.rotationVector.y, 0));
	}

	#endregion

	#region Player Actions

	// Method called when an action event is triggered, received the desired action as args (enum)
	private void PlayerAbilities_OnActionButtonPressed(object sender, InputManager.ActionInputPressedEventArgs e)
	{
		switch (e.actionPressed)
		{
			case InputAction.Stand:
				Debug.Log(e.actionPressed);

				if (_hitTopFront.transform == null && _hitTopBack.transform == null)
				{
					StartCoroutine(CheckTopCollisionBeforeStand(_characterInitialHeight));
				}

				break;

			case InputAction.Crouch:
				Debug.Log(e.actionPressed);

				if (_characterController.isGrounded)
				{
					CrouchAndStand(_characterInitialHeight / 3);
				}

				break;

			case InputAction.Flashlight:
				_flashlight.enabled = !_flashlight.enabled;
				break;

			case InputAction.Use:
				Debug.Log(e.actionPressed);

				if (_hitForward.transform != null)
				{
					_hitForward.transform.GetComponent<MeshRenderer>().material.color = Color.green;
				}

				break;

			default:
				break;
		}
	}

	#region Crouch and Stand Action

	private void CrouchAndStand(float height)
	{
		float lastHeight = _characterController.height;
		_characterController.height = height;
		this.transform.position = new Vector3(this.transform.position.x, (_characterController.height - lastHeight) * 0.5f, this.transform.position.z);
	}

	private IEnumerator CheckTopCollisionBeforeStand(float height)
	{
		while (Physics.Raycast(_direction * _characterController.radius + _characterController.transform.position, _characterController.transform.up, out _hitTopFront, (height / 3) + 1f)
			|| Physics.Raycast(-_direction * _characterController.radius + _characterController.transform.position, _characterController.transform.up, out _hitTopBack, (height / 3) + 1f))
		{
			yield return new WaitForEndOfFrame();
		}

		CrouchAndStand(height);
	}

	#endregion

	#endregion

	#endregion
}
