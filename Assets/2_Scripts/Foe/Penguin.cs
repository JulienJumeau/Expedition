using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum FoeState
{
	idle,
	Patrol,
	Chase,
	Attack,
	Alert
}

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	[SerializeField] private bool _isNextDestinationRandom = false, _isPenguinAggro = false;
	[SerializeField] private float _foePatrolSpeed = 0, _foeChaseSpeed = 0;
	[SerializeField] private float _detectionRadius = 0, _detectionRadiusAggro = 0, _detectionRadiusWHoldingBreath = 0, _secondsInAlertBeforeAggro = 0, _secondsInAlertAfterHiding = 0, _secondsBeforeFirstAttack = 0, _secondsBeforeSecondAttack = 0, timeToRecoverLife = 0;
	[SerializeField] private bool _allowAttacks = false;
	[SerializeField] private float _stoppingDistanceAttack = 4;
	[SerializeField] private float _minTimeRandomNoisePatrol = 0, _maxTimeRandomNoisePatrol = 0;

	[Header("Sounds")]
	[SerializeField] public AudioClip[] _audioClipPenguinPatrol;
	[SerializeField] public AudioClip[] _audioClipPenguinFootstepsRunnning;
	[SerializeField] private AudioClip _audioClipDying;
	[SerializeField] private AudioClip _audioClipTakingDamage;
	[SerializeField] private AudioClip _audioClipPenguinAlert;
	[SerializeField] private AudioClip _audioClipPenguinAggro;
	[SerializeField] private AudioClip _audioClipPenguinFootstepsWalking;
	[SerializeField] private AudioClip _audioClipPenguinAttack;

	[HideInInspector] public FoeState _foeState;
	private NavMeshAgent _agent;
	private PlayerAbilities _player;
	private Transform _targetPlayer;
	private Animator _animator;
	public AudioSource _audioSource;
	private int _nextDestinationIndex;
	private float _distancePlayerFoe, _currentDetectionRadius, _secondsWhileWounded, _currentChaseSpeed, _secondsWhileAlert, _secondsWhileAlertHiding;
	private bool _isAttacking, _isAggroSoundPlayed, _isPatrolSoundPlaying;
	private string[] _triggerAnimationNames;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_player = FindObjectOfType<PlayerAbilities>();
		_animator = GetComponentInChildren<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_targetPlayer = _player.transform;
		_nextDestinationIndex = 0;
		_distancePlayerFoe = 0;
		_currentDetectionRadius = _detectionRadius;
		_secondsWhileAlert = 0;
		_secondsWhileAlertHiding = 0;
		_secondsWhileWounded = 0;
		_triggerAnimationNames = new string[4] { "P_IsWalking", "P_IsRunning", "P_IsLookingFor", "P_IsBiting2" };
		_currentChaseSpeed = _foeChaseSpeed;
	}

	private void Start()
	{
		_foeState = _patrolPoints.Length == 1 && transform.position == _patrolPoints[0].position ? FoeState.idle : FoeState.Patrol;
		EventSubcription();
	}

	private void Update()
	{
		//Dans quel state est-on?
		//if (_foeState == FoeState.Attack)
		//{
		//	Debug.Log("Attack");
		//}
		//else if (_foeState == FoeState.Chase)
		//{
		//	Debug.Log("Chase");
		//}
		//else if (_foeState == FoeState.Patrol)
		//{
		//	Debug.Log("Patrol");
		//}
		//else if (_foeState == FoeState.Alert)
		//{
		//	Debug.Log("Alert");
		//}

		if (_agent.enabled)
		{
			FoePattern();
			//RegenLife(timeToRecoverLife);
			TriggerAnimation();
		}

		// Work but not sattified
		//if (_foeState == FoeState.Patrol)
		//{
		//	if (!_isPatrolSoundPlaying)
		//	{
		//		_isPatrolSoundPlaying = true;
		//		StartCoroutine(PatrolRandomSound());
		//	}
		//}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRadiusAggro);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _detectionRadius);
	}

	#endregion

	private void FoePattern() // /!\ A CORRIGER: Quand les pingu reviennent à leur next point de patrouille après avoir chase le player, ils ne reviennent pas exactement à ce point de patrouille mais le dépassent et se retrouvent plus loin que le point (à cause de la vitesse de cours élevée) 
	{							//Probleme avec les transitions d'anim Alert to Chase
		_distancePlayerFoe = Vector3.Distance(_targetPlayer.position, transform.position);

		// STATE = PATROL
		if (_foeState == FoeState.Patrol && !_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			GoToNextPatrolPoint();
		}

		// PLAYER AGGRO
		if (_distancePlayerFoe <= _currentDetectionRadius && _player._isHiding == false && !_isAttacking && _isPenguinAggro)
		{
			// PENGUIN ALERT
			if (_secondsWhileAlert >= _secondsInAlertBeforeAggro)
			{
				if (_distancePlayerFoe <= _currentDetectionRadius && _player._isHiding == false && !_isAttacking && _isPenguinAggro)
				{
					// START AGGRO PLAYER
					_currentDetectionRadius = _detectionRadiusAggro;
					_foeState = FoeState.Chase;
					PlayerAbilities._isDetected = true;
					_currentChaseSpeed = _foeChaseSpeed;
					SetFoeAgentProperties(_targetPlayer.position, _currentChaseSpeed, _stoppingDistanceAttack, true);

					if (!_isAggroSoundPlayed)
					{
						_isAggroSoundPlayed = true;
						_audioSource.PlayOneShot(_audioClipPenguinAggro);
					}

					if (IsFoeNearTarget())
					{
						_agent.velocity = Vector3.zero;
						_foeState = FoeState.Attack;
					}
				}
			}
			else if (_foeState == FoeState.Alert)
			{
				_secondsWhileAlert += Time.deltaTime;
				Debug.Log("SA: " + _secondsWhileAlert + " s");
				SetFoeAgentProperties(transform.position, 0, 0, false);
			}
			else if (_foeState != FoeState.Alert)
			{
				_foeState = FoeState.Alert;
				SetFoeAgentProperties(transform.position, 0, 0, false);
				//FaceTarget();
			}
		}
		// STOP AGGRO PLAYER
		else if (_foeState == FoeState.Chase)
		{
			PlayerAbilities._isDetected = false;
			_isAggroSoundPlayed = false;
			_currentDetectionRadius = _detectionRadius;
			SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foeChaseSpeed, 0, false);

			if (_agent.remainingDistance < 0.5f)
			{
				_foeState = FoeState.Patrol;
				_secondsWhileAlert = 0;
				_secondsWhileAlertHiding = 0;
			}
		}
		else if (_foeState == FoeState.Alert)
		{
			Debug.Log("SA: " + _secondsWhileAlert + " s");
			if (_secondsWhileAlert >= _secondsInAlertBeforeAggro)
			{
				PlayerAbilities._isDetected = false;
				_currentDetectionRadius = _detectionRadius;
				ResetAllTriggerAnimation();
				_animator.SetBool(_triggerAnimationNames[0], true);
				SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foePatrolSpeed, 0, false);

				//Stop Sound Alert

				if (_agent.remainingDistance < 0.5f)
				{
					_foeState = FoeState.Patrol;
					_secondsWhileAlert = 0;
					_secondsWhileAlertHiding = 0;
				}
			}
			else _secondsWhileAlert += Time.deltaTime;
		}

		// STATE = ATTACK
		if (_foeState == FoeState.Attack)
		{
			FaceTarget();
			if (_allowAttacks)
			{
				if (!_isAttacking)
				{
					StartCoroutine(Attack());
				}
			}

			if (!IsFoeNearTarget() || _player._isHiding == true)
			{
				_foeState = FoeState.Chase;
				_isAttacking = false;
			}
		}

		// WHEN HIDING
		if (_player._isHiding == true && _distancePlayerFoe <= _currentDetectionRadius && _isPenguinAggro && _foeState != FoeState.Patrol)
		{
			_secondsWhileAlertHiding += Time.deltaTime;
			//Debug.Log("SAH: " + _secondsWhileAlertHiding + " s");
			_secondsWhileAlert = 0;

			if (_secondsWhileAlertHiding >= _secondsInAlertAfterHiding)
			{
				_currentDetectionRadius = _detectionRadius;

				if (_secondsInAlertAfterHiding > 0)
				{
					ResetAllTriggerAnimation();
					_animator.SetBool(_triggerAnimationNames[0], true);
					SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foePatrolSpeed, 0, false);
				}
				else
				{
					ResetAllTriggerAnimation();
					_animator.SetBool(_triggerAnimationNames[1], true);
					SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foeChaseSpeed, 0, false);
				}
				

				//Stop Sound Alert

				if (_agent.remainingDistance < 0.5f)
				{
					_foeState = FoeState.Patrol;
					_secondsWhileAlert = 0;
					_secondsWhileAlertHiding = 0;
				}
			}
			else if (_foeState == FoeState.Alert)
			{
				SetFoeAgentProperties(transform.position, 0, 0, false);
			}
			else if (_foeState != FoeState.Alert)// && _foeState != FoeState.Patrol)
			{
				_foeState = FoeState.Alert;
				SetFoeAgentProperties(transform.position, 0, 0, false);
				//FaceTarget();
			}
		}

		// WHEN HOLDING BREATH
		if (_player._isHoldingBreath && (_foeState == FoeState.Patrol || _foeState == FoeState.idle))
		{
			_currentDetectionRadius = _detectionRadiusWHoldingBreath;
		}
		else if (_foeState == FoeState.Patrol || _foeState == FoeState.idle)
		{
			_currentDetectionRadius = _detectionRadius;
		}

		// STATE = IDLE
		if (_foeState == FoeState.idle)
		{
			_agent.speed = 0;
		}
	}

	private void GoToNextPatrolPoint()
	{
		switch (_patrolPoints.Length)
		{
			case 0:
				Debug.Log("Warning empty array points for partol");
				return;
			case 1:

				if (_agent.remainingDistance < 1)
				{
					_foeState = FoeState.idle;
				}

				break;
			default:
				SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foePatrolSpeed, 0, false);

				if (_isNextDestinationRandom)
				{
					_nextDestinationIndex = Random.Range(0, _patrolPoints.Length);
				}

				else
				{
					_nextDestinationIndex++;
				}

				_nextDestinationIndex %= _patrolPoints.Length;
				break;
		}
	}

	private void FaceTarget()
	{
		Vector3 direction = (_targetPlayer.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	private IEnumerator Attack()
	{
		_isAttacking = true;

		//_audioSource.clip = _audioClipPenguinAttack;
		//_audioSource.Play();

		if (PostProcessManager._isPostProssessAttack && !HudManager._isTheEnd)
		{
			yield return new WaitForSeconds(_secondsBeforeSecondAttack);
			if (IsFoeNearTarget())
			{
				_animator.SetBool(_triggerAnimationNames[3], true);
				//_audioSource.PlayOneShot(_audioClipDying);

				HudManager._isGameOver = true;

			}
		}
		else if (!HudManager._isTheEnd)
		{
			yield return new WaitForSeconds(_secondsBeforeFirstAttack);
			if (IsFoeNearTarget())
			{
				_animator.SetBool(_triggerAnimationNames[3], true);
				Debug.Log("attack lol");
				PostProcessManager._isPostProssessAttack = true;
			}
		}
		_isAttacking = false;
	}

	private void RegenLife(float timeToRecoverLife)
	{
		if (PostProcessManager._isPostProssessAttack)
		{
			_secondsWhileWounded += Time.deltaTime;
		}
		else _secondsWhileWounded = 0;

		if (_secondsWhileWounded >= timeToRecoverLife)
		{
			PostProcessManager._isPostProssessAttack = false;
		}
	}

	private void SetFoeAgentProperties(Vector3 targetPosition, float speed, float stoppingDistance, bool autoBraking)
	{
		_agent.SetDestination(targetPosition);
		_agent.speed = speed;
		_agent.stoppingDistance = stoppingDistance;
		_agent.autoBraking = autoBraking;
	}

	private void TriggerAnimation()
	{
		switch (_foeState)
		{
			case FoeState.idle:
				ResetAllTriggerAnimation();
				break;
			case FoeState.Patrol:
				ResetAllTriggerAnimation();
				_animator.SetBool(_triggerAnimationNames[0], true);
				break;
			case FoeState.Chase:
				ResetAllTriggerAnimation();
				_animator.SetBool(_triggerAnimationNames[1], true);
				break;
			case FoeState.Attack:
				_animator.SetBool(_triggerAnimationNames[0], false);
				_animator.SetBool(_triggerAnimationNames[1], false);
				break;
			case FoeState.Alert:
				ResetAllTriggerAnimation();
				_animator.SetBool(_triggerAnimationNames[2], true);
				break;
			default:
				break;
		}
	}

	private void ResetAllTriggerAnimation()
	{
		for (int i = 0; i < _triggerAnimationNames.Length; i++)
		{
			_animator.ResetTrigger(_triggerAnimationNames[i]);
		}
	}

	private bool IsFoeNearTarget() => _distancePlayerFoe <= _agent.stoppingDistance;

	private void EventSubcription()
	{
		FindObjectOfType<HudManager>().OnPause += Penguin_OnPause;
	}

	private void Penguin_OnPause(object sender, HudManager.PauseEventArgs e)
	{
		if (e.isPaused)
		{
			_animator.speed = 0;
			_agent.enabled = !e.isPaused;
		}

		else
		{
			_animator.speed = 1;
			_agent.enabled = !e.isPaused;
		}
	}

	private IEnumerator PatrolRandomSound()
	{
		yield return new WaitForSeconds(Random.Range(_minTimeRandomNoisePatrol, _maxTimeRandomNoisePatrol));
		_audioSource.PlayOneShot(_audioClipPenguinPatrol[Random.Range(0, _audioClipPenguinPatrol.Length)]);
		_isPatrolSoundPlaying = false;
	}
}
