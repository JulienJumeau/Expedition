using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerAbilities : MonoBehaviour
{
	#region Variables declaration

	public static PlayerAbilities _current;
	public static bool _isActionPlaying;
	public static bool _isDetected;

	[Range(1, 10)] [SerializeField] private float _walkSpeed = 5;
	[Range(1, 15)] [SerializeField] private float _sprintSpeed = 10;
	[Range(1, 5)] [SerializeField] private float _crouchSpeed = 3;
	[Range(1, 5)] [SerializeField] private float _pullObjectSpeed = 2;
	[Range(1, 5)] [SerializeField] private int _lightbulbNbrMax = 2;
	[SerializeField] private LayerMask _layerMask = 0;
	[SerializeField] private GameObject _lanternGO = null, _photoCameraGO = null;
	[SerializeField] private AnimationCurve _climbWallAnimationCurve = null, _climbBoxAnimationCurve = null, _jumpAnimationCurve = null;
	[SerializeField] public float _holdingBreathSecondsAllowed = 0, _HoldingBreathSoftCooldown = 0, _HoldingBreathHardCooldown = 0;
	[SerializeField] private AudioClip _audioClipHoldBreath, _audioClipGetBreathSoft, _audioClipGetBreathHard;
	[SerializeField] private GameObject _lanternLightGO = null, _fxFireLantern = null;
	[SerializeField] public float _lanternMinIntensity = 0, _lanternMaxIntensity = 0, _secondsOilLevelFullToEmpty = 1;

	private Camera _camera;
	private CharacterController _characterController;
	private Animator _animator;
	private Vector3 _motion, _motionForward, _motionStrafe, _direction, _positionBeforeHide;
	private RaycastHit _hitForward, _hitBackward, _hitDownFront, _hitDownBack;
	private bool _isCrouching, _isLanternOnScreen, _isReading, _isPulling;
	[HideInInspector] public bool _isHiding, _isHoldingBreath, _isHoldingBreathOnCooldown;
	private string[] _triggerAnimationNames;
	public static float _oilLevel;
	public static bool _isLanternInInventory;
	private float _oilLevelMax;
	private int _lightbulbNbr;
	private float _characterInitialHeight, _currentSpeed;
	private AudioSource _audioSource;
	private Light _lanternLight;
	private Material _lanternMaterial;

	[HideInInspector] public float _holdingBreathSeconds;

	#endregion

	#region Unity Methods - Awake/Start/Update

	private void Awake()
	{
		_camera = Camera.main;
		_characterController = GetComponent<CharacterController>();
		_animator = _camera.GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_lanternMaterial = _fxFireLantern.GetComponent<Renderer>().material;
		_triggerAnimationNames = new string[4] { "IsWalk", "IsRun", "IsJumping", "IsClimbing" };
		_characterInitialHeight = _characterController.height;
		_currentSpeed = _walkSpeed;
		_lightbulbNbr = 0;
		_lightbulbNbrMax = 2;
		_oilLevelMax = 1;
		_isDetected = _isReading = _isPulling = false;
		_lanternLight = _lanternLightGO.GetComponent<Light>();
		_lanternLight.intensity = 0;
		_fxFireLantern.transform.localPosition = new Vector3(_fxFireLantern.transform.localPosition.x, -0.4f, _fxFireLantern.transform.localPosition.z);
	}

	private void Start()
	{
		if (_isLanternInInventory)
		{
			_isLanternOnScreen = !_isLanternOnScreen;
			HoldItem(_lanternGO, _photoCameraGO);
		}

		EventSubscription();
	}

	private void Update()
	{
		if (!_isActionPlaying && !_isHiding)
		{
			Movement();
		}

		if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitForward, 3f, _layerMask) && !_isActionPlaying && !_isPulling)
		{
			HUDDisplay(new HUDDisplayEventArgs { isActive = true, layerDetected = _hitForward.transform.gameObject.layer, isSheet = false });
		}

		else
		{
			HUDDisplay(new HUDDisplayEventArgs { isActive = false, layerDetected = 0, isSheet = false });
		}

		OilLevelDecreasing();
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
			case InputAction.Walk:

				if (!_isHiding)
				{
					if (!_isCrouching)
					{
						_currentSpeed = _walkSpeed;
					}

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
					if (!_isCrouching)
					{
						_isCrouching = true;
						_currentSpeed = _crouchSpeed;
						CrouchAndStand(_characterInitialHeight / 6);
					}

					else if (!CheckTopCollisionBeforeStand2(_characterInitialHeight))
					{
						_isCrouching = false;
						_currentSpeed = _walkSpeed;
						CrouchAndStand(_characterInitialHeight);
					}
				}

				break;

			case InputAction.Lantern:

				if (!_isHiding && _isLanternInInventory)
				{
					_isLanternOnScreen = !_isLanternOnScreen;
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

					if (_hitForward.transform.gameObject.layer == 21 && !_isReading)
					{
						_isActionPlaying = !_isActionPlaying;
						_isReading = !_isReading;
						Sheet sheet = _hitForward.transform.parent.GetComponentInChildren<Sheet>();
						HUDDisplay(new HUDDisplayEventArgs { isActive = true, layerDetected = _hitForward.transform.gameObject.layer, isSheet = true, sheetID = sheet.sheetID });
						break;
					}
				}

				if (_isReading)
				{
					_isActionPlaying = !_isActionPlaying;
					_isReading = !_isReading;
					HUDDisplay(new HUDDisplayEventArgs { isActive = false, isSheet = true });
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
						_isPulling = true;
						PullObject();
					}
				}

				break;

			case InputAction.HoldBreath:

				if (!_isDetected)
				{
					if (_holdingBreathSeconds < _holdingBreathSecondsAllowed && !_isHoldingBreathOnCooldown)
					{
						if (!_isHoldingBreath)
						{
							_audioSource.clip = _audioClipHoldBreath;
							_audioSource.Play();
						}

						_isHoldingBreath = true;
						PostProcessManager._isPostProssessHoldBreath = true;

						_holdingBreathSeconds += Time.deltaTime;
					}

					else
					{
						if (!_isHoldingBreathOnCooldown)
						{
							StartCoroutine(HoldingBreathCooldown(_HoldingBreathHardCooldown));
						}
					}
				}

				break;

			case InputAction.StopHoldingBreath:

				if (!_isDetected)
				{
					if (!_isHoldingBreathOnCooldown)
					{
						StartCoroutine(HoldingBreathCooldown(_HoldingBreathSoftCooldown));
					}
				}

				break;

			default:
				ResetAllTriggerAnimation();
				_isPulling = false;
				break;
		}
	}

	private void CrouchAndStand(float height)
	{
		float lastHeight = _characterController.height;
		//this.transform.position = new Vector3(this.transform.position.x, (_characterController.height - lastHeight) * 0.5f, this.transform.position.z);
		_characterController.height = height;
	}

	private bool CheckTopCollisionBeforeStand2(float height)
	{
		return (Physics.Raycast(_direction * _characterController.radius + _characterController.transform.position, _characterController.transform.up, (height / 3) + 1f)
			|| Physics.Raycast(-_direction * _characterController.radius + _characterController.transform.position, _characterController.transform.up, (height / 3) + 1f));
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
			else print("You can't carry more lightbulbs!");
		}

		if (_hitForward.transform.gameObject.CompareTag("Oil"))
		{
			if (_oilLevel < _oilLevelMax)
			{
				_oilLevel = _oilLevelMax;
				_fxFireLantern.transform.localPosition = new Vector3(_fxFireLantern.transform.localPosition.x, -0.155f, _fxFireLantern.transform.localPosition.z);
				_lanternLight.intensity = _lanternMaxIntensity;
				Destroy(_hitForward.transform.gameObject);
			}
			else print("Lantern is already full!");
		}

		if (_hitForward.transform.gameObject.CompareTag("Lantern"))
		{
			if (!_isLanternInInventory)
			{
				_oilLevel = 0;
				_isLanternInInventory = true;
				_isLanternOnScreen = true;
				HoldItem(_lanternGO, _photoCameraGO);
				Destroy(_hitForward.transform.gameObject);
			}
			else print("You are already carrying a lantern!");
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

	private IEnumerator HoldingBreathCooldown(float cooldown)
	{
		_isHoldingBreathOnCooldown = true;
		_isHoldingBreath = false;
		PostProcessManager._isPostProssessHoldBreath = false;

		if (cooldown == _HoldingBreathSoftCooldown)
		{
			_audioSource.clip = _audioClipGetBreathSoft;
		}

		else
		{
			_audioSource.clip = _audioClipGetBreathHard;
		}

		_audioSource.Play();

		_holdingBreathSeconds = 0;
		yield return new WaitForSeconds(cooldown);
		_isHoldingBreathOnCooldown = false;
	}

	private void EventSubscription()
	{
		FindObjectOfType<InputManager>().OnDirectionInputPressed += PlayerAbilities_OnDirectionInputPressed;
		FindObjectOfType<InputManager>().OnCameraMove += PlayerAbilities_OnCameraInputPressed;
		FindObjectOfType<InputManager>().OnActionInputPressed += PlayerAbilities_OnActionButtonPressed;
	}

	private void OilLevelDecreasing()
	{
		Debug.Log(_oilLevel > 0);
		if (_oilLevel > 0f && _isLanternOnScreen && !_isReading)
		{
			_oilLevel = Mathf.Clamp(_oilLevel - Time.deltaTime / _secondsOilLevelFullToEmpty, 0, 1);
			_fxFireLantern.transform.localPosition = Vector3.Lerp(new Vector3(0, -0.24f, 0), new Vector3(0, -0.155f, 0), _oilLevel);
			_lanternMaterial.SetFloat("_EmissiveIntensity", Mathf.Lerp(5, 40, _oilLevel));
			_lanternLight.intensity = Mathf.Lerp(_lanternMinIntensity, _lanternMaxIntensity, _oilLevel);
			//Debug.Log("Oil lvl:" + _oilLevel + " s");
			//Debug.Log("lanternLight intensity:" + _lanternLight.intensity);
		}
	}

	#endregion

	#region Events

	public class HUDDisplayEventArgs : EventArgs
	{
		public bool isActive;
		public bool isSheet;
		public int layerDetected;
		public int sheetID;
	}

	public event EventHandler<HUDDisplayEventArgs> OnHUDDisplay;

	public void HUDDisplay(HUDDisplayEventArgs e) => OnHUDDisplay?.Invoke(this, e);

	#endregion
}
