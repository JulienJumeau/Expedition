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
	private Vector3 _motion, _motionForward, _motionStrafe, _direction;
	private RaycastHit _hitForward, _hitTopFront, _hitTopBack;
	private float _characterInitialHeight;
	private bool _isRunning;

	public static int _lightbulbNbr;
	[Range(1f, 5f)] [SerializeField] private int _lightbulbNbrMax = 2;
	private int _OilLevel; //Between 0 and 1


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

		_lightbulbNbr = 0;
		_lightbulbNbrMax = 2;
		_OilLevel = 0;

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
			PullObject();
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
		_motion = Vector3.zero;
		float currentSpeed = _isRunning ? _sprintSpeed : _speed;
		_motion += (_motionForward + _motionStrafe).normalized * currentSpeed;

		// Check if the character is grounded, if not add the gravity
		if (_characterController.isGrounded)
		{
			_motion.y = 0;
		}

		else
		{
			_motion += this.transform.up * Physics.gravity.y;
		}

		// Call the native method Move from character controller and send all moves to it
		_characterController.Move(_motion * Time.deltaTime);
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
					// Pick up Objects
					if (_hitForward.transform.gameObject.tag == "Lightbulb")
					{
						if (_lightbulbNbr < _lightbulbNbrMax)
						{
							_lightbulbNbr++;
							print("New Lightbulb picked up! You now have [" + _lightbulbNbr + "] Lightbulbs in your inventory.");
							Destroy(_hitForward.transform.gameObject);
						}
						else print("You reach the maximum number of Lightbulbs that you can carry in your inventory.");
					}
					if (_hitForward.transform.gameObject.tag == "Oil")
					{
						_OilLevel = 1;
						print("Your oil level is now full!");
						Destroy(_hitForward.transform.gameObject);
					}
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

		//this.transform.position += this.transform.forward * 8f;
		this.transform.position += _hitForward.transform.right * Mathf.Sign(heading.x) * 8f;

		yield return null;
		//yield return new WaitForSeconds(2);
		_isActionPlaying = false;
	}

	private void PullObject ()
	{
		if (_hitForward.transform.gameObject.tag == "Pullable") //&& (_hitForward.transform != null))
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				//Debug.Log("Motion = ( " + motion.x + " , " + motion.y + " , " + motion.z + " )");
				_hitForward.transform.Translate(new Vector3(_motion.x, 0, _motion.z) * Time.deltaTime);
			}

			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				_hitForward.transform.Translate(Vector3.zero);
			}
		}
	}

	#endregion

	#endregion

	#endregion
}
