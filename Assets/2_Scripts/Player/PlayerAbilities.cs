using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerAbilities : MonoBehaviour
{
	#region Variables declaration

	public static PlayerAbilities _current;
	public static bool _isActionPlaying;

	[Range(1, 10)] [SerializeField] private float _speed;
	[Range(5, 15)] [SerializeField] private float _sprintSpeed;
	[SerializeField] private LayerMask _layerMask;

	private Camera _camera;
	private CharacterController _characterController;
	private Light _flashlight;
	private Vector3 _motionForward, _motionStrafe, _direction;
	private RaycastHit _hitForward, _hitTopFront, _hitTopBack;
	private float _characterInitialHeight;
	private bool _isRunning;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		#region Variables initialization

		_camera = Camera.main;
		_characterController = GetComponent<CharacterController>();
		_characterInitialHeight = _characterController.height;
		_flashlight = GameObject.FindWithTag("FlashLight").GetComponent<Light>();
		_isRunning = false;

		#endregion
	}

	private void Start()
	{
		EventSubscription();
	}

	private void Update()
	{
		// Call movement method each frame
		if (!_isActionPlaying)
		{
			Movement();
		}

		// Check if the player is aiming a GameObject with a specific layermask ( raycast beside camera forward ) 
		if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitForward, 2f, _layerMask))
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
		float currentSpeed = _isRunning ? _sprintSpeed : _speed;
		motion += (_motionForward + _motionStrafe).normalized * currentSpeed;

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

			case InputAction.Walk:
				Debug.Log(e.actionPressed);
				_isRunning = false;

				break;

			case InputAction.Run:
				Debug.Log(e.actionPressed);
				_isRunning = true;

				break;


			case InputAction.Crouch:
				Debug.Log(e.actionPressed);

				if (_characterController.isGrounded)
				{
					CrouchAndStand(_characterInitialHeight / 6);
				}

				break;

			case InputAction.Flashlight:
				_flashlight.enabled = !_flashlight.enabled;
				break;

			case InputAction.Use:

				if (_hitForward.transform != null)
				{
					TriggerActionToExecute(_hitForward.transform.gameObject.layer);
				}

				break;

			case InputAction.Jump:

				if (_hitForward.transform != null)
				{
					TriggerMoveToExecute(_hitForward.transform.gameObject.layer);
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

	#region Action Button

	private void TriggerActionToExecute(int layer)
	{
		switch (layer)
		{
			case 10:
				Debug.Log("Swicth");
				_hitForward.transform.GetComponent<MeshRenderer>().material.color = Color.green;

				break;

			default:
				break;
		}
	}

	private void TriggerMoveToExecute(int layer)
	{
		Debug.Log("Jump");
		switch (layer)
		{
			case 13:

				if (!_isActionPlaying)
				{
					Debug.Log("Climb");
					StartCoroutine(Climbing());
				}

				break;

			case 15:

				if (!_isActionPlaying)
				{
					Debug.Log("Jump");
					StartCoroutine(Jumping());
				}

				break;

			default:
				break;
		}
	}

	private IEnumerator Climbing()
	{
		_isActionPlaying = true;

		Vector3 heading = (_hitForward.point - this.transform.position) * 1.1f;
		heading.y = 0;
		this.transform.position = new Vector3(this.transform.position.x + heading.x, _hitForward.transform.localScale.y + 0.73f, this.transform.position.z + heading.z);

		yield return null;

		//yield return new WaitForSeconds(2);
		_isActionPlaying = false;
	}

	private IEnumerator Jumping()
	{
		_isActionPlaying = true;

		Vector3 heading = _hitForward.point - this.transform.position;
		heading.y = 0;
		Debug.Log(heading);

		this.transform.position = new Vector3(this.transform.position.x + heading.x * _hitForward.transform.localScale.x * 1.5f, this.transform.position.y, this.transform.position.z + heading.z);
		//this.transform.position = this.transform.position + heading + _hitForward.transform.localScale;

		yield return new WaitForSeconds(2);
		_isActionPlaying = false;
	}

	#endregion

	#endregion

	#endregion
}
