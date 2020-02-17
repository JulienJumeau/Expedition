using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerAbilities : MonoBehaviour
{
	#region Variables declaration

	public static PlayerAbilities _current;
	public static bool _isActionPlaying;
	public static bool _isDetected;
	public static bool _isReading;
	public static bool _isPlayerInjured;
	public static bool _mustStandUp;
	public static bool _isTheBeginning = true;
	public static bool _isPulling;

	[Range(1, 10)] [SerializeField] private float _walkSpeed = 5;
	[Range(1, 15)] [SerializeField] private float _sprintSpeed = 10;
	[Range(1, 5)] [SerializeField] private float _crouchSpeed = 3;
	[Range(1, 5)] [SerializeField] private float _pullObjectSpeed = 2;
	[Range(1, 5)] [SerializeField] private int _lightbulbNbrMax = 2;
	[SerializeField] private LayerMask _layerMask = 0;
	[SerializeField] private GameObject _lanternGO = null, _photoCameraGO = null;
	[SerializeField] private AnimationCurve _climbWallAnimationCurve = null, _climbBoxAnimationCurve = null, _jumpAnimationCurve = null;
	[SerializeField] public float _holdingBreathSecondsAllowed = 0, _HoldingBreathSoftCooldown = 0, _HoldingBreathHardCooldown = 0;
	[SerializeField] private GameObject _lanternLightGO = null, _fxFireLantern = null;
	[SerializeField] public float _lanternMinIntensity = 0, _lanternMaxIntensity = 0, _secondsOilLevelFullToEmpty = 1;
	[SerializeField] private float timeToRecoverLife = 0;

	[Header("Audio Sources")]
	[SerializeField] private AudioSource _audioSourceMovement = null;
	[SerializeField] private AudioSource _audioSourcePlayerSounds = null;
	[SerializeField] private AudioSource _audioSourceLoots = null;
	[SerializeField] private AudioSource _audioSourceMusicDetected = null;

	[Header("Sounds")]
	[SerializeField] private AudioClip[] _audioClipWalk;
	[SerializeField] private AudioClip[] _audioClipRun;
	[SerializeField] private AudioClip[] _audioClipCrouch;
	[SerializeField] private AudioClip[] _audioClipLimp;
	[SerializeField] private AudioClip _audioGameBeginning;
	[SerializeField] private AudioClip _audioClipDetected;
	[SerializeField] private AudioClip _audioClipRunTooLong;
	[SerializeField] private AudioClip _audioClipClimb;
	[SerializeField] private AudioClip _audioClipDying;
	[SerializeField] private AudioClip _audioClipTakingDamage;
	[SerializeField] private AudioClip _audioClipHeartbeat;
	[SerializeField] private AudioClip _audioClipHoldBreath;
	[SerializeField] private AudioClip _audioClipGetBreathSoft;
	[SerializeField] private AudioClip _audioClipGetBreathHard;
	[SerializeField] private AudioClip _audioClipLootOil;
	[SerializeField] private AudioClip _audioClipLantern;
	[SerializeField] private AudioClip _audioClipReadSheet;
	[SerializeField] private AudioClip _audioClipMoveBox;
	[SerializeField] private AudioClip _audioClipMonster;
	private AudioSource _beginWindGO;

	public static bool _isEndGame;
	private Camera _camera;
	private CharacterController _characterController;
	private Animator _animator;
	private Vector3 _motion, _motionForward, _motionStrafe, _direction, _positionBeforeHide;
	private RaycastHit _hitForward, _hitBackward, _hitDownFront, _hitDownBack;
	private bool _isCrouching, _isLanternOnScreen, _isLanternOnScreenBeforeHiding;
	[HideInInspector] public bool _isHiding, _isHoldingBreath, _isHoldingBreathOnCooldown;
	private bool _isRunning;
	private string[] _triggerAnimationNames;
	public static float _oilLevel;
	public static bool _isLanternInInventory, _isDying;
	private float _oilLevelMax;
	private int _lightbulbNbr;
	private float _characterInitialHeight, _currentSpeed;
	private Light _lanternLight;
	private Material _lanternMaterial;
	//private bool _isDetectedMusicMustBePlayed;
	private int _currentReadingSheetIndex;
	private bool _canOnlyLimp, _isPlayingTakingHitAnimSound, _isPlayingDyingAnimSound;
	private float _timerHitRegen;

	[HideInInspector] public float _holdingBreathSeconds;

	#endregion

	#region Unity Methods - Awake/Start/Update

	private void Awake()
	{
		_camera = Camera.main;
		_characterController = GetComponent<CharacterController>();
		_animator = _camera.GetComponent<Animator>();
		_lanternMaterial = _fxFireLantern.GetComponent<Renderer>().material;
		_triggerAnimationNames = new string[8] { "IsWalk", "IsRun", "IsJumping", "IsDraging", "IsClimbing", "IsOpening", "IsLimping", "IsDead" };
		_characterInitialHeight = _characterController.height;
		_isActionPlaying = true;
		_currentSpeed = _walkSpeed;
		_lightbulbNbr = 0;
		_lightbulbNbrMax = 2;
		_oilLevelMax = 1;
		_isDetected = _isReading = _isPulling = _isDying = false;
		_lanternLight = _lanternLightGO.GetComponent<Light>();
		_lanternLight.intensity = 0;
		_currentReadingSheetIndex = 0;
		_canOnlyLimp = false;
		_isEndGame = false;
		_isPlayingTakingHitAnimSound = false;
		_isPlayingDyingAnimSound = false;
		//_isDetectedMusicMustBePlayed = false;
		_fxFireLantern.transform.localPosition = new Vector3(_fxFireLantern.transform.localPosition.x, -0.4f, _fxFireLantern.transform.localPosition.z);
		_timerHitRegen = 0;
	}

	private void Start()
	{
		if (_isTheBeginning)
		{
			_audioSourceMovement.PlayOneShot(_audioGameBeginning);
			StartCoroutine(WaitBeforeBeginningAnimation(8.5f));
			StartCoroutine(WaitBeforeBeginMoves(12f));
		}

		else
		{
			this.transform.GetComponentInChildren<AudioSource>().enabled = true;
			StartCoroutine(WaitBeforeBeginMoves(3f));
			GameObject.FindGameObjectWithTag("WindSound").GetComponent<AudioSource>().Play();
		}

		if (_isLanternInInventory)
		{
			_isLanternOnScreen = !_isLanternOnScreen;
			HoldItem(_lanternGO, _photoCameraGO);
		}

		EventSubscription();
	}

	private void Update()
	{
		if (!_isEndGame)
		{

			if (!_isActionPlaying && !_isHiding)
			{
				Movement();
			}

			if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hitForward, 2f, _layerMask) && !_isActionPlaying && !_isPulling)
			{
				if ((_hitForward.transform.gameObject.layer == 12 || _hitForward.transform.gameObject.layer == 17) && _hitForward.distance < 1.1f || (_hitForward.transform.gameObject.layer != 12 && _hitForward.transform.gameObject.layer != 17))
				{
					HUDDisplay(new HUDDisplayEventArgs { isActive = true, layerDetected = _hitForward.transform.gameObject.layer, isSheet = false, isHiding = _isHiding });
				}

				else
				{
					HUDDisplay(new HUDDisplayEventArgs { isActive = false, layerDetected = 0, isSheet = false });
				}
			}

			else
			{
				HUDDisplay(new HUDDisplayEventArgs { isActive = false, layerDetected = 0, isSheet = false });
			}

			if (_isPlayerInjured && !_canOnlyLimp)
			{
				_canOnlyLimp = true;
				HoldItem(_lanternGO, _photoCameraGO, true);
			}

			if (_mustStandUp && _isCrouching)
			{
				_mustStandUp = false;
				_isCrouching = false;
				_animator.SetBool("IsCrouching", false);
				ResetAllTriggerAnimation();
				CrouchAndStand(_characterInitialHeight);
			}

			//if (_isDetected && !_isDetectedMusicMustBePlayed)
			//{
			//	_isDetectedMusicMustBePlayed = true;
			//	_audioSourceMusicDetected.clip = _audioClipDetected;
			//	StartCoroutine(DetectedMusicSmooth(2, _isDetectedMusicMustBePlayed));
			//	_audioSourceMusicDetected.Play();
			//}

			//else if (!_isDetected && _isDetectedMusicMustBePlayed)
			//{
			//	_isDetectedMusicMustBePlayed = false;
			//	StartCoroutine(DetectedMusicSmooth(2, _isDetectedMusicMustBePlayed));
			//}

			OilLevelDecreasing();
			RegenLife(timeToRecoverLife);

			if (PostProcessManager._isPostProssessAttack && !_isPlayingTakingHitAnimSound)
			{
				_isPlayingTakingHitAnimSound = true;
				TakingHit();
			}

			if (_isDying && !_isPlayingDyingAnimSound)
			{
				Dying();
				_isActionPlaying = true;
				_isPlayingDyingAnimSound = true;
			}
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
			_motion.y -= 0.0001f;
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
						_currentSpeed = _isPlayerInjured ? _crouchSpeed : _walkSpeed;
						_isRunning = false;
						ResetAllTriggerAnimation();
					}

					if (!_isPlayerInjured)
					{
						_animator.SetBool(_triggerAnimationNames[0], true);
					}

					else
					{
						_animator.SetBool(_triggerAnimationNames[6], true);
					}
				}

				break;

			case InputAction.Run:

				if (!_isCrouching && !_isHiding && !_isPlayerInjured)
				{
					_currentSpeed = _sprintSpeed;
					ResetAllTriggerAnimation();
					_animator.SetBool(_triggerAnimationNames[0], true);
					_animator.SetBool(_triggerAnimationNames[1], true);
					_isRunning = true;
					//_audioSource.clip = _audioClipRun;
					//_audioSource.Play();
				}

				else
				{
					_animator.SetBool(_triggerAnimationNames[1], false);
					_isRunning = false;
				}

				break;

			case InputAction.Crouch:

				if (!_isHiding && !_isPlayerInjured)
				{
					if (!_isCrouching)
					{
						_isCrouching = true;
						_currentSpeed = _crouchSpeed;
						_animator.SetBool("IsCrouching", true);
						CrouchAndStand(_characterInitialHeight / 6);
					}

					else if (!CheckTopCollisionBeforeStand2(_characterInitialHeight))
					{
						_isCrouching = false;
						_currentSpeed = _walkSpeed;
						_animator.SetBool("IsCrouching", false);
						ResetAllTriggerAnimation();
						CrouchAndStand(_characterInitialHeight);
					}
				}

				break;

			case InputAction.Lantern:

				if (!_isHiding && _isLanternInInventory && !_isPlayerInjured)
				{
					_isLanternOnScreen = !_isLanternOnScreen;
					HoldItem(_lanternGO, _photoCameraGO);
					_audioSourceLoots.PlayOneShot(_audioClipLantern);
				}

				break;

			case InputAction.PhotoCamera:

				if (!_isHiding && !_isPlayerInjured)
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
						_animator.speed = 0;
						Sheet sheet = _hitForward.transform.parent.GetComponentInChildren<Sheet>();
						_currentReadingSheetIndex = sheet.sheetID;
						HUDDisplay(new HUDDisplayEventArgs { isActive = true, layerDetected = _hitForward.transform.gameObject.layer, isSheet = true, sheetID = sheet.sheetID });
						_audioSourceLoots.PlayOneShot(_audioClipReadSheet);
						break;
					}
				}

				if (_isReading && !InputManager._isPaused)
				{
					_isActionPlaying = !_isActionPlaying;
					_isReading = !_isReading;
					_animator.speed = 1;
					HUDDisplay(new HUDDisplayEventArgs { isActive = false, isSheet = true });

					if (_currentReadingSheetIndex == 3)
					{
						EndGame();
					}
				}

				break;

			case InputAction.Jump:

				if (_hitForward.transform != null && !_isActionPlaying && !_isHiding && !_isCrouching && _characterController.transform.position.y < _hitForward.point.y)
				{
					// Climbable walls
					if (_hitForward.transform.gameObject.layer == 12 && _hitForward.distance < 1.1f)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 1, _climbWallAnimationCurve));
						_animator.SetTrigger(_triggerAnimationNames[4]);
						//_audioSource.clip = _audioClipClimb;
						//_audioSource.Play();
					}

					// Climbable Box
					if (_hitForward.transform.gameObject.layer == 17 && _hitForward.distance < 1.1f)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 1, _climbBoxAnimationCurve));
						_animator.SetTrigger(_triggerAnimationNames[4]);
					}

					// Jumpable
					if (_hitForward.transform.gameObject.layer == 13)
					{
						StartCoroutine(AnimateMove(this.transform.position, _hitForward.transform.GetChild(0).position, 2, _jumpAnimationCurve));
					}
				}

				break;

			case InputAction.Pull:

				if (!_isHiding)
				{
					if (_hitForward.transform != null && _hitForward.transform.gameObject.CompareTag("Pullable") && CheckCollisionBeforePull(_characterInitialHeight) && _hitForward.distance < 1.7f)
					{
						_currentSpeed = _pullObjectSpeed;
						_isPulling = true;
						_animator.SetBool(_triggerAnimationNames[3], true);
						PullObject();
					}

					else
					{
						_currentSpeed = _walkSpeed;
						_isPulling = false;
						_animator.SetBool(_triggerAnimationNames[3], false);
					}
				}

				break;

			case InputAction.HoldBreath:

				if (!_isRunning && !_isPlayerInjured)
				{
					if (!_isDetected)
					{
						if (_holdingBreathSeconds < _holdingBreathSecondsAllowed && !_isHoldingBreathOnCooldown)
						{
							if (!_isHoldingBreath)
							{
								_audioSourcePlayerSounds.PlayOneShot(_audioClipHoldBreath);
							}

							_isHoldingBreath = true;
							PostProcessManager._isPostProssessHoldBreath = true;

							_holdingBreathSeconds += Time.deltaTime;
						}

						else if (!_isHoldingBreathOnCooldown)
						{
							StartCoroutine(HoldingBreathCooldown(_HoldingBreathHardCooldown));
						}
					}
				}

				else if (_isHoldingBreath)
				{
					StartCoroutine(HoldingBreathCooldown(_HoldingBreathSoftCooldown));
				}

				break;

			case InputAction.StopHoldingBreath:

				if (!_isRunning && !_isDetected && !_isHoldingBreathOnCooldown)
				{
					StartCoroutine(HoldingBreathCooldown(_HoldingBreathSoftCooldown));
				}

				break;

			default:
				
				if (!_isPlayingTakingHitAnimSound)
				{
					ResetAllTriggerAnimation();
				}

				_isRunning = false;
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
		// Loot Lightbulb
		if (_hitForward.transform.gameObject.CompareTag("Lightbulb"))
		{
			if (_lightbulbNbr < _lightbulbNbrMax)
			{
				_lightbulbNbr++;
				Destroy(_hitForward.transform.gameObject);
			}
			else print("You can't carry more lightbulbs!");
		}
		// Loot Oil
		if (_hitForward.transform.gameObject.CompareTag("Oil"))
		{
			if (_oilLevel < _oilLevelMax)
			{
				_oilLevel = _oilLevelMax;
				_fxFireLantern.transform.localPosition = new Vector3(_fxFireLantern.transform.localPosition.x, -0.155f, _fxFireLantern.transform.localPosition.z);
				_lanternLight.intensity = _lanternMaxIntensity;
				Destroy(_hitForward.transform.gameObject);
				_audioSourceLoots.PlayOneShot(_audioClipLootOil);
			}
			else print("Lantern is already full!");
		}

		// Loot lantern
		if (_hitForward.transform.gameObject.CompareTag("Lantern"))
		{
			if (!_isLanternInInventory)
			{
				_oilLevel = 0;
				_isLanternInInventory = true;
				_isLanternOnScreen = true;
				HoldItem(_lanternGO, _photoCameraGO);
				Destroy(_hitForward.transform.gameObject);
				_audioSourceLoots.PlayOneShot(_audioClipLantern);
			}
			else print("You are already carrying a lantern!");
		}
	}

	private void PullObject()
	{
		_hitForward.transform.Translate(new Vector3(_motion.x, 0, _motion.z) * Time.deltaTime);
		//_audioSource.clip = _audioClipMoveBox;
		//_audioSource.Play();
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
		if (_isLanternOnScreen)
		{
			_isLanternOnScreenBeforeHiding = true;
		}
		else _isLanternOnScreenBeforeHiding = false;

		HoldItem(_lanternGO, _photoCameraGO, true);
		_isLanternOnScreen = false;
		_isActionPlaying = true;
		_characterController.enabled = false;
		_isHiding = true;
		ResetAllTriggerAnimation();

		_positionBeforeHide = this.transform.position;
		this.transform.position = _hitForward.transform.parent.GetChild(0).transform.position;
		Quaternion rotation = Quaternion.LookRotation(-_hitForward.transform.right, Vector3.up);
		this.transform.rotation = rotation;
		Vector3 rotationParent = this.transform.GetChild(0).rotation.eulerAngles;
		rotationParent.x -= 30;
		this.transform.GetChild(0).rotation = Quaternion.Euler(rotationParent);
		_isActionPlaying = false;
	}

	private void GetOut()
	{
		if (_isLanternOnScreenBeforeHiding)
		{
			HoldItem(_lanternGO, _photoCameraGO);
			_isLanternOnScreen = true;
		}
		this.transform.position = _positionBeforeHide;
		Vector3 rotationParent = this.transform.GetChild(0).rotation.eulerAngles;
		rotationParent.x += 30;
		this.transform.GetChild(0).rotation = Quaternion.Euler(rotationParent);
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
			_audioSourcePlayerSounds.clip = _audioClipGetBreathSoft;
		}

		else
		{
			_audioSourcePlayerSounds.clip = _audioClipGetBreathHard;
		}

		_audioSourcePlayerSounds.Play();

		_holdingBreathSeconds = 0;
		yield return new WaitForSeconds(cooldown);
		_isHoldingBreathOnCooldown = false;
	}

	private void EventSubscription()
	{
		FindObjectOfType<InputManager>().OnDirectionInputPressed += PlayerAbilities_OnDirectionInputPressed;
		FindObjectOfType<InputManager>().OnCameraMove += PlayerAbilities_OnCameraInputPressed;
		FindObjectOfType<InputManager>().OnActionInputPressed += PlayerAbilities_OnActionButtonPressed;
		FindObjectOfType<PlayerAnimationEvents>().OnFootStepWalk += PlayerAbilities_OnFootStepWalk;
		FindObjectOfType<PlayerAnimationEvents>().OnFootStepRun += PlayerAbilities_OnFootStepRun;
		FindObjectOfType<PlayerAnimationEvents>().OnFootStepCrouch += PlayerAbilities_OnFootStepCrouch;
		FindObjectOfType<PlayerAnimationEvents>().OnFootStepLimp += PlayerAbilities_OnFootStepLimp;
		FindObjectOfType<PlayerAnimationEvents>().OnHit += PlayerAbilities_OnHit;
		FindObjectOfType<HudManager>().OnPause += PlayerAbilities_OnPause;
	}

	private void PlayerAbilities_OnHit(object sender, EventArgs e)
	{
		_audioSourceMovement.clip = _audioClipTakingDamage;
		_audioSourceMovement.PlayOneShot(_audioSourceMovement.clip);
	}

	private void PlayerAbilities_OnFootStepWalk(object sender, EventArgs e)
	{
		if (!_isHoldingBreath && _characterController.isGrounded)
		{
			_audioSourceMovement.clip = _audioClipWalk[UnityEngine.Random.Range(0, _audioClipWalk.Length - 1)];
			_audioSourceMovement.PlayOneShot(_audioSourceMovement.clip);
		}
	}

	private void PlayerAbilities_OnFootStepRun(object sender, EventArgs e)
	{
		if (!_isHoldingBreath && _characterController.isGrounded)
		{
			_audioSourceMovement.clip = _audioClipRun[UnityEngine.Random.Range(0, _audioClipRun.Length - 1)];
			_audioSourceMovement.PlayOneShot(_audioSourceMovement.clip);
		}
	}

	private void PlayerAbilities_OnFootStepCrouch(object sender, EventArgs e)
	{
		if (!_isHoldingBreath && _characterController.isGrounded)
		{
			_audioSourceMovement.clip = _audioClipCrouch[UnityEngine.Random.Range(0, _audioClipCrouch.Length - 1)];
			_audioSourceMovement.PlayOneShot(_audioSourceMovement.clip);
		}
	}

	private void PlayerAbilities_OnFootStepLimp(object sender, EventArgs e)
	{
		if (_characterController.isGrounded)
		{
			_audioSourceMovement.clip = _audioClipLimp[UnityEngine.Random.Range(0, _audioClipLimp.Length - 1)];
			_audioSourceMovement.PlayOneShot(_audioSourceMovement.clip);
		}
	}

	private void OilLevelDecreasing()
	{
		//Debug.Log(_oilLevel > 0);
		if (_oilLevel > 0f && _isLanternOnScreen && !_isReading && !InputManager._isPaused)
		{
			_oilLevel = Mathf.Clamp(_oilLevel - Time.deltaTime / _secondsOilLevelFullToEmpty, 0, 1);
			_fxFireLantern.transform.localPosition = Vector3.Lerp(new Vector3(0, -0.24f, 0), new Vector3(0, -0.155f, 0), _oilLevel);
			_lanternMaterial.SetFloat("_EmissiveIntensity", Mathf.Lerp(25, 120, _oilLevel));
			_lanternLight.intensity = Mathf.Lerp(_lanternMinIntensity, _lanternMaxIntensity, _oilLevel);
			//Debug.Log("Oil lvl:" + _oilLevel + " s");
			//Debug.Log("lanternLight intensity:" + _lanternLight.intensity);
		}
	}

	private IEnumerator WaitBeforeBeginMoves(float duration)
	{
		yield return new WaitForSeconds(duration);
		Debug.Log(duration);
		_isActionPlaying = false;
	}

	private IEnumerator WaitBeforeBeginningAnimation(float duration)
	{
		yield return new WaitForSeconds(duration);
		_animator.SetTrigger(_triggerAnimationNames[5]);
		this.transform.GetComponentInChildren<AudioSource>().enabled = true;
		GameObject.FindGameObjectWithTag("WindSound").GetComponent<AudioSource>().Play();
	}

	private void EndGame()
	{
		_isEndGame = true;
		_isActionPlaying = true;
		_audioSourcePlayerSounds.PlayOneShot(_audioClipMonster);
	}

	private void PlayerAbilities_OnPause(object sender, HudManager.PauseEventArgs e)
	{
		_animator.speed = e.isPaused ? 0 : 1;
	}

	private IEnumerator DetectedMusicSmooth(float duration, bool detected)
	{
		float elapsedTime = 0;
		float musicVolume;
		Debug.Log(detected + " test");

		while (elapsedTime <= duration)
		{
			elapsedTime += Time.deltaTime;
			musicVolume = detected ? Mathf.Lerp(0, 1, elapsedTime / duration) : Mathf.Lerp(1, 0, elapsedTime / duration);
			_audioSourceMusicDetected.volume = musicVolume;
			yield return null;
		}

		if (!_isDetected)
		{
			_audioSourceMusicDetected.Stop();
		}
	}

	private void RegenLife(float timeToRecoverLife)
	{
		if (PostProcessManager._isPostProssessAttack)
		{
			_timerHitRegen += Time.deltaTime;
		}
		else _timerHitRegen = 0;

		if (_timerHitRegen >= timeToRecoverLife)
		{
			PostProcessManager._isPostProssessAttack = false;
			_isPlayingTakingHitAnimSound = false;
		}
	}

	private void TakingHit()
	{
		ResetAllTriggerAnimation();
		Debug.Log(_triggerAnimationNames[7]);
		_animator.SetTrigger("IsHit");
	}

	private void Dying()
	{
		_audioSourcePlayerSounds.PlayOneShot(_audioClipDying);
		ResetAllTriggerAnimation();
		_animator.SetTrigger(_triggerAnimationNames[7]);
	}

	#endregion

	#region Events

	public class HUDDisplayEventArgs : EventArgs
	{
		public bool isActive;
		public bool isSheet;
		public int layerDetected;
		public int sheetID;
		public bool isHiding;
	}

	public event EventHandler<HUDDisplayEventArgs> OnHUDDisplay;

	public void HUDDisplay(HUDDisplayEventArgs e) => OnHUDDisplay?.Invoke(this, e);

	#endregion
}
