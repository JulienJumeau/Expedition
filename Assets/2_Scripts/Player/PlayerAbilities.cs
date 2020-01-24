using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerAbilities : MonoBehaviour
{
	#region Variables declaration

	public static PlayerAbilities _current;
	public static bool _isActionPlaying;

	[Range(1, 10)] [SerializeField] private float _walkSpeed = 5;
	[Range(5, 15)] [SerializeField] private float _sprintSpeed = 10;
	[Range(1, 5)] [SerializeField] private float _crouchSpeed = 3;
	[Range(1, 5)] [SerializeField] private float _pullObjectSpeed = 2;
	[Range(1, 5)] [SerializeField] private int _lightbulbNbrMax = 2;
	[SerializeField] private LayerMask _layerMask = 0;
	[SerializeField] private GameObject _lanternGO = null, _photoCameraGO = null;
	[SerializeField] private AnimationCurve _climbWallAnimationCurve = null, _climbBoxAnimationCurve = null, _jumpAnimationCurve = null;

	private Camera _camera;
	private CharacterController _characterController;
	private Animator _animator;
	private Vector3 _motion, _motionForward, _motionStrafe, _direction, _positionBeforeHide;
	private RaycastHit _hitForward, _hitBackward, _hitTopFront, _hitTopBack, _hitDownFront, _hitDownBack;
	private bool _isCrouching;
	[HideInInspector] public bool _isHiding;
	private string[] _triggerAnimationNames;
	private int _lightbulbNbr;
	private int _oilLevel, _oilLevelMax;
	private float _characterInitialHeight, _currentSpeed;

	#endregion

	#region Unity Methods - Awake/Start/Update

	private void Awake()
	{
		_camera = Camera.main;
		_characterController = GetComponent<CharacterController>();
		_animator = _camera.GetComponent<Animator>();
		_triggerAnimationNames = new string[4] { "IsWalk", "IsRun", "IsJumping", "IsClimbing" };
		_characterInitialHeight = _characterController.height;
		_currentSpeed = _walkSpeed;
		_lightbulbNbr = 0;
		_lightbulbNbrMax = 2;
		_oilLevel = 0;
		_oilLevelMax = 3;
	}

	private void Start()
	{
		EventSubscription();
	}

	private void Update()
	{
		if (!_isActionPlaying && !_isHiding)
		{
			Movement();
		}

		if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitForward, 3f, _layerMask))
		{
			HUDDisplay(new HUDDisplayEventArgs { isActive = true, layerDetected = _hitForward.transform.gameObject.layer });
		}

		else
		{
			HUDDisplay(new HUDDisplayEventArgs { isActive = false, layerDetected = 0 });
		}
	}

	#endregion

	#region Player Movements - InputMoves/Camera EventsArgs + Move Method

	private void PlayerAbilities_OnCameraInputPressed(object sender, InputManager.CameraMoveEventArgs e)
	{
		Vector3 rotation = this.transform.rotation.eulerAngles;
		rotation.y += e.rotationVector.y;
		this.transform.rotation = Quaternion.Euler(rotation);
	}

	private void PlayerAbilities_OnDirectionInputPressed(object sender, InputManager.DirectionInputPressedEventArgs e)
	{
		_motionForward = this.transform.forward * e.directionVector.z;
		_motionStrafe = this.transform.right * e.directionVector.x;
		_direction = (_motionForward + _motionStrafe).normalized;
	}

	private void Movement()
	{
		_motion = Vector3.zero;
		_motion += (_motionForward + _motionStrafe).normalized * _currentSpeed;

		if (_characterController.isGrounded)
		{
			_motion.y = 0;
		}

		else
		{
			_motion += this.transform.up * Physics.gravity.y;
		}

		_characterController.Move(_motion * Time.deltaTime);
		_motionForward = Vector3.zero;
		_motionStrafe = Vector3.zero;
	}

	#endregion

	#region Player Actions - InputActions EventsArgs + Crouch/Climb/Jump/HoldItem/CollectItems/PullObject Methods + Event Subs

	private void PlayerAbilities_OnActionButtonPressed(object sender, InputManager.ActionInputPressedEventArgs e)
	{
		switch (e.actionPressed)
		{
			case InputAction.Stand:

				if (_hitTopFront.transform == null && _hitTopBack.transform == null && !_isHiding)
				{
					StartCoroutine(CheckTopCollisionBeforeStand(_characterInitialHeight));
				}

				break;

			case InputAction.Walk:

				if (!_isHiding)
				{
					_currentSpeed = _walkSpeed;
					ResetAllTriggerAnimation();
					_animator.SetBool(_triggerAnimationNames[0], true);
				}

				break;

			case InputAction.Run:

				if (!_isCrouching && !_isHiding)
				{
					_currentSpeed = _sprintSpeed;
					ResetAllTriggerAnimation();
					_animator.SetBool(_triggerAnimationNames[1], true);
				}

				break;

			case InputAction.Crouch:

				if (!_isHiding)
				{
					_currentSpeed = _crouchSpeed;

					if (_characterController.isGrounded)
					{
						_isCrouching = true;
						CrouchAndStand(_characterInitialHeight / 6);
					}
				}

				break;

			case InputAction.Lantern:

				if (!_isHiding)
				{
					HoldItem(_lanternGO, _photoCameraGO);
				}

				break;

			case InputAction.PhotoCamera:

				if (!_isHiding)
				{
					HoldItem(_photoCameraGO, _lanternGO);
				}

				break;

			case InputAction.Use:

				if (_hitForward.transform != null)
				{
					if (_hitForward.transform.gameObject.layer == 10 && !_isHiding)
					{
						_hitForward.transform.GetComponent<MeshRenderer>().material.color = Color.green;
					}

					if (_hitForward.transform.gameObject.layer == 11 && !_isHiding)
					{
						CollectItems();
					}

					if (_hitForward.transform.gameObject.layer == 16)
					{
						if (!_isHiding)
						{
							Hide();
						}

						else
						{
							GetOut();
						}
					}
				}

				break;

			case InputAction.Jump:

				if (_hitForward.transform != null && !_isActionPlaying && !_isHiding)
				{
					if (_hitForward.transform.gameObject.layer == 12 && _hitForward.distance < 1.1f)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 1, _climbWallAnimationCurve));
					}

					if (_hitForward.transform.gameObject.layer == 17)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 0.5f, _climbBoxAnimationCurve));
					}

					if (_hitForward.transform.gameObject.layer == 13)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 2, _jumpAnimationCurve));
					}
				}

				break;

			case InputAction.Pull:

				if (!_isHiding)
				{
					if (_hitForward.transform != null && _hitForward.transform.gameObject.CompareTag("Pullable") && CheckCollisionBeforePull(_characterInitialHeight) && _hitForward.distance < 2.7f)
					{
						_currentSpeed = _pullObjectSpeed;
						PullObject();
					}

					else
					{
						_currentSpeed = _walkSpeed;
					}
				}

				break;

			default:
				ResetAllTriggerAnimation();
				break;
		}
	}

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
		_isCrouching = false;
	}

	private IEnumerator AnimateMove(Vector3 origin, Vector3 targetPosition, float duration, AnimationCurve animationCurve)
	{
		_isActionPlaying = true;
		_characterController.enabled = false;
		ResetAllTriggerAnimation();

		float elapsedTime = 0, percent, curvePercent;

		while (elapsedTime <= duration)
		{

			elapsedTime += Time.deltaTime;
			percent = Mathf.Clamp01(elapsedTime / duration);
			curvePercent = animationCurve.Evaluate(percent);
			this.transform.position = Vector3.Lerp(origin, targetPosition, curvePercent);
			yield return null;
		}

		yield return null;
		_isActionPlaying = false;
		_characterController.enabled = true;
	}

	private void HoldItem(GameObject goToHandle, GameObject goToHide, bool mustDropItem = false)
	{
		if (!mustDropItem)
		{
			goToHandle.SetActive(!goToHandle.activeSelf);
			goToHide.SetActive(false);
		}

		else
		{
			goToHandle.SetActive(false);
			goToHide.SetActive(false);
		}
	}

	private void CollectItems()
	{
		if (_hitForward.transform.gameObject.CompareTag("Lightbulb"))
		{
			if (_lightbulbNbr < _lightbulbNbrMax)
			{
				_lightbulbNbr++;
				Destroy(_hitForward.transform.gameObject);
			}
		}

		if (_hitForward.transform.gameObject.CompareTag("Oil"))
		{
			if (_oilLevel < _oilLevelMax)
			{
				_oilLevel++;
				Destroy(_hitForward.transform.gameObject);
			}
		}
	}

	private void PullObject()
	{
		_hitForward.transform.Translate(new Vector3(_motion.x, 0, _motion.z) * Time.deltaTime);
	}

	private bool CheckCollisionBeforePull(float height)
	{
		Physics.Raycast((_direction * _characterController.radius) + _characterController.transform.position, -_characterController.transform.up, out _hitDownFront, height);
		Physics.Raycast((-_direction * _characterController.radius) + _characterController.transform.position, -_characterController.transform.up, out _hitDownBack, height);
		Physics.Raycast((_direction * _characterController.radius) + _characterController.transform.position, -_characterController.transform.forward, out _hitBackward, _characterController.radius);

		return _hitDownFront.transform != null && _hitDownBack.transform != null &&
			_hitDownFront.transform.gameObject.layer == 9 && _hitDownBack.transform.gameObject.layer == 9 &&
			(_hitBackward.transform == null || _hitBackward.transform == this.transform);
	}

	private void ResetAllTriggerAnimation()
	{
		for (int i = 0; i < _triggerAnimationNames.Length; i++)
		{
			_animator.ResetTrigger(_triggerAnimationNames[i]);
		}
	}

	private void Hide()
	{
		HoldItem(_lanternGO, _photoCameraGO, true);
		_isActionPlaying = true;
		_characterController.enabled = false;
		_isHiding = true;
		ResetAllTriggerAnimation();

		_positionBeforeHide = this.transform.position;
		this.transform.position = _hitForward.transform.parent.GetChild(0).transform.position;
		Quaternion rotation = Quaternion.LookRotation(-_hitForward.transform.right, Vector3.up);
		this.transform.rotation = rotation;
		_isActionPlaying = false;
	}

	private void GetOut()
	{
		this.transform.position = _positionBeforeHide;
		_isHiding = false;
		_characterController.enabled = true;
	}

	private void EventSubscription()
	{
		FindObjectOfType<InputManager>().OnDirectionInputPressed += PlayerAbilities_OnDirectionInputPressed;
		FindObjectOfType<InputManager>().OnCameraMove += PlayerAbilities_OnCameraInputPressed;
		FindObjectOfType<InputManager>().OnActionInputPressed += PlayerAbilities_OnActionButtonPressed;
	}

	#endregion

	#region Events

	public class HUDDisplayEventArgs : EventArgs
	{
		public bool isActive;
		public int layerDetected;
	}

	public event EventHandler<HUDDisplayEventArgs> OnHUDDisplay;

	public void HUDDisplay(HUDDisplayEventArgs e) => OnHUDDisplay?.Invoke(this, e);

	#endregion
}
