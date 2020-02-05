using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum FoeState
{
	idle,
	Patrol,
	Chase,
	Attack
}

public sealed class Penguin : MonoBehaviour
{
	#region Variables declaration

	[SerializeField] Transform[] _patrolPoints = null;
	[SerializeField] private bool _isNextDestinationRandom = false, _isPenguinAggro = false;
	[SerializeField] private float _foePatrolSpeed = 0, _foeChaseSpeed = 0;
	[SerializeField] private float _detectionRadius = 0, _detectionRadiusAggro = 0, _detectionRadiusWHoldingBreath = 0, _secondsBeforeFirstAttack = 0, _secondsBeforeSecondAttack = 0, _secondsToRecoverFullLife = 0;
	[SerializeField] private bool _allowAttacks = false, _allowChasingAudiosource = false;
	[SerializeField] private float _stoppingDistanceAttack = 4;

	[Header("Sounds")]
	[SerializeField] private AudioClip _audioClipDying;
	[SerializeField] private AudioClip _audioClipTakingDamage;
	[SerializeField] private AudioClip _audioClipPenguinAlert;
	[SerializeField] private AudioClip _audioClipPenguinAggro;
	[SerializeField] private AudioClip _audioClipPenguinFootstepsRunnning;
	[SerializeField] private AudioClip _audioClipPenguinFootstepsWalking;
	[SerializeField] private AudioClip _audioClipPenguinAttack;

	[HideInInspector] public FoeState _foeState;
	private NavMeshAgent _agent;
	private PlayerAbilities _player;
	private Transform _targetPlayer;
	private Animator _animator;
	private AudioSource _audioSource;
	private int _nextDestinationIndex;
	private float _distanceTargetAgent, _currentDetectionRadius, _secondsWhileWounded, _currentChaseSpeed;
	private bool _isAttacking;
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
		_distanceTargetAgent = 0;
		_currentDetectionRadius = _detectionRadius;
		_secondsWhileWounded = 0;
		_triggerAnimationNames = new string[4] { "P_IsWalking", "P_IsRunning", "P_IsLookingFor", "P_IsBiting2" };
		_currentChaseSpeed = _foeChaseSpeed;
	}

	private void Start()
	{
		if (_patrolPoints.Length == 1 && transform.position == _patrolPoints[0].position)
		{
			_foeState = FoeState.idle;
		}
		else
		{
			_foeState = FoeState.Patrol;
		}
	}

	private void Update()
	{
		////Dans quel state est-on?
		//if (_foeState == FoeState.idle)
		//{
		//	Debug.Log("Idle");
		//}
		//else if (_foeState == FoeState.Attack)
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

		FoePattern();

		//Sound for chasing (provisoire car à incorporer à SoundManager)
		if (PlayerAbilities._isDetected)
		{
			if (_allowChasingAudiosource)
			{
				GetComponent<AudioSource>().enabled = true;
			}
		}

		else
		{
			if (_allowChasingAudiosource)
			{
				GetComponent<AudioSource>().enabled = false;
			}
		}

		if (_foeState == FoeState.Patrol)
		{
			ResetAllTriggerAnimation();
			_animator.SetBool(_triggerAnimationNames[0], true);
		}

		if (_foeState == FoeState.Chase)
		{
			ResetAllTriggerAnimation();
			_animator.SetBool(_triggerAnimationNames[1], true);
		}

		if (_foeState == FoeState.Attack)
		{
			_animator.SetBool(_triggerAnimationNames[0], false);
			_animator.SetBool(_triggerAnimationNames[1], false);
		}
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
	{
		// AFTER ATTACKING 1 TIME
		if (PostProcessManager._isPostProssessOn)
		{
			_secondsWhileWounded += Time.deltaTime;
		}
		else
		{
			_secondsWhileWounded = 0;
		}

		// REGEN LIFE
		if (_secondsWhileWounded >= _secondsToRecoverFullLife)
		{
			PostProcessManager._isPostProssessOn = false;
		}

		// STATE = PATROL
		if (_foeState == FoeState.Patrol && !_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			Debug.Log("Go to next patrol point");
			GoToNextPatrolPoint();
		}

		// STATE = CHASE
		_distanceTargetAgent = Vector3.Distance(_targetPlayer.position, this.transform.position);

				// PLAYER AGGRO
		if (_distanceTargetAgent <= _currentDetectionRadius && _player._isHiding == false && !_isAttacking && _isPenguinAggro)
		{
			_currentDetectionRadius = _detectionRadiusAggro;
			_foeState = FoeState.Chase;
			PlayerAbilities._isDetected = true;
			_currentChaseSpeed = _distanceTargetAgent <= _agent.stoppingDistance * 2 ? 2 : _foeChaseSpeed;
			SetFoeAgentProperties(_targetPlayer.position, _currentChaseSpeed, _stoppingDistanceAttack, true);

			_audioSource.clip = _audioClipPenguinAggro;
			_audioSource.Play();

			if (IsFoeNearTarget())
			{
				_foeState = FoeState.Attack;
			}
		}
				// STOP AGGRO PLAYER
		else if (_foeState == FoeState.Chase)
		{
			PlayerAbilities._isDetected = false;
			_currentDetectionRadius = _detectionRadius;
			SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foeChaseSpeed, 0, false);

			if (_audioSource.clip == _audioClipPenguinAggro)
			{
				_audioSource.Stop();
			}

			if (_agent.remainingDistance < 0.5f)
			{
				_foeState = FoeState.Patrol;
			}
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
			_agent.SetDestination(transform.position);
			ResetAllTriggerAnimation();
		}
	}

	private void GoToNextPatrolPoint()
	{
		// CASE 0 PATROL POINTS
		if (_patrolPoints.Length == 0)
		{
			Debug.Log("Warning empty array points for partol");
			return;
		}

		// CASE 1 PATROL POINT
		if (_patrolPoints.Length == 1 && (transform.position.z > _patrolPoints[0].position.z - 0.2f && transform.position.z < _patrolPoints[0].position.z + 0.2f) && (transform.position.x > _patrolPoints[0].position.x - 0.2f && transform.position.x < _patrolPoints[0].position.x + 0.2f))
		{
			_foeState = FoeState.idle;
		}
		else // CASE MORE THAN 1 PATROL POINTS
		{
			SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foePatrolSpeed, 0, false);

			if (_isNextDestinationRandom)
			{
				_nextDestinationIndex = Random.Range(0, _patrolPoints.Length);
			}
			else
			{
				_nextDestinationIndex++;
			}

			_nextDestinationIndex = (_nextDestinationIndex) % _patrolPoints.Length;
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

		_audioSource.clip = _audioClipPenguinAttack;
		_audioSource.Play();
		// Play Anim attack penguin here

		if (PostProcessManager._isPostProssessOn)
		{
			yield return new WaitForSeconds(_secondsBeforeSecondAttack);
			if (IsFoeNearTarget())
			{
				_animator.SetBool(_triggerAnimationNames[3], true);

				_audioSource.clip = _audioClipDying;
				_audioSource.Play();

				ScenesManager._isGameOver = true;

			}
		}
		else
		{
			yield return new WaitForSeconds(_secondsBeforeFirstAttack);
			if (IsFoeNearTarget())
			{
				_animator.SetBool(_triggerAnimationNames[3], true);

				_audioSource.clip = _audioClipTakingDamage;
				_audioSource.Play();

				PostProcessManager._isPostProssessOn = true;
			}
		}
		_isAttacking = false;
	}

	private void SetFoeAgentProperties(Vector3 targetPosition, float speed, float stoppingDistance, bool autoBraking)
	{
		_agent.SetDestination(targetPosition);
		_agent.speed = speed;
		_agent.stoppingDistance = stoppingDistance;
		_agent.autoBraking = autoBraking;
	}

	private void ResetAllTriggerAnimation()
	{
		for (int i = 0; i < _triggerAnimationNames.Length; i++)
		{
			_animator.ResetTrigger(_triggerAnimationNames[i]);
		}
	}

	private bool IsFoeNearTarget() => _distanceTargetAgent <= _agent.stoppingDistance;

}
