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
	[SerializeField] private bool _isNextDestinationRandom = false;
	[SerializeField] private float _foePatrolSpeed = 0, _foeChaseSpeed = 0;
	[SerializeField] private float _detectionRadius = 0, _detectionRadiusAggro = 0, _detectionRadiusWHoldingBreath = 0, _secondsBeforeFirstAttack = 0, _secondsBeforeSecondAttack = 0, _secondsToRecoverFullLife = 0;
	[SerializeField] private bool _allowAttacks = false, _allowChasingAudiosource = false;
	[SerializeField] private float _stoppingDistanceAttack = 4;
	[HideInInspector] public FoeState _foeState;
	private NavMeshAgent _agent;
	private PlayerAbilities _player;
	private Transform _targetPlayer;
	private int _nextDestinationIndex;
	private float _distanceTargetAgent, _currentDetectionRadius, _secondsWhileWounded, _currentChaseSpeed;
	private bool _isAttacking;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_player = FindObjectOfType<PlayerAbilities>();
		_targetPlayer = _player.transform;
		_nextDestinationIndex = 0;
		_distanceTargetAgent = 0;
		_foeState = FoeState.Patrol;
		_currentDetectionRadius = _detectionRadius;
		_secondsWhileWounded = 0;
		_currentChaseSpeed = _foeChaseSpeed;
	}

	private void Start()
	{
		//GoToNextPatrolPoint();
	}

	private void Update()
	{
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
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRadiusAggro);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _detectionRadius);
	}

	#endregion

	private void FoePattern()
	{
		//gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _isChasingPlayer ? Color.red : Color.green;

		if (PostProcessManager._isPostProssessOn)
		{
			_secondsWhileWounded += Time.deltaTime;
		}

		else
		{
			_secondsWhileWounded = 0;
		}

		if (_secondsWhileWounded >= _secondsToRecoverFullLife)
		{
			PostProcessManager._isPostProssessOn = false;
		}

		if (_foeState == FoeState.Patrol && !_agent.pathPending && _agent.remainingDistance < 0.5f)
		{
			GoToNextPatrolPoint();
		}

		_distanceTargetAgent = Vector3.Distance(_targetPlayer.position, this.transform.position);

		if (_distanceTargetAgent <= _currentDetectionRadius && _player._isHiding == false && !_isAttacking)
		{
			_currentDetectionRadius = _detectionRadiusAggro;
			_foeState = FoeState.Chase;
			PlayerAbilities._isDetected = true;
			_currentChaseSpeed = _distanceTargetAgent <= _agent.stoppingDistance * 2 ? 2 : _foeChaseSpeed;
			SetFoeAgentProperties(_targetPlayer.position, _currentChaseSpeed, _stoppingDistanceAttack, true);

			if (IsFoeNearTarget())
			{
				_foeState = FoeState.Attack;
			}
		}

		else if (_foeState == FoeState.Chase)
		{
			_agent.SetDestination(this.transform.position);
			PlayerAbilities._isDetected = false;
			_currentDetectionRadius = _detectionRadius;
			SetFoeAgentProperties(_patrolPoints[_nextDestinationIndex].position, _foeChaseSpeed, 0, false);

			if (_agent.remainingDistance < 0.5f)
			{
				_foeState = FoeState.Patrol;
			}
		}

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
				//Debug.Log("_isAttacking = false");
			}
		}

		if (_player._isHoldingBreath && _foeState == FoeState.Patrol)
		{
			_currentDetectionRadius = _detectionRadiusWHoldingBreath;
		}

		else if (_foeState == FoeState.Patrol)
		{
			_currentDetectionRadius = _detectionRadius;
		}

	}

	private void GoToNextPatrolPoint()
	{
		if (_patrolPoints.Length == 0)
		{
			Debug.Log("Warning empty array points for partol");
			return;
		}

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

	private void FaceTarget()
	{
		Vector3 direction = (_targetPlayer.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	private IEnumerator Attack()
	{
		_isAttacking = true;
		// Play Anim attack penguin here

		if (PostProcessManager._isPostProssessOn)
		{
			yield return new WaitForSeconds(_secondsBeforeSecondAttack);
			if (IsFoeNearTarget())
			{
				ScenesManager._isGameOver = true;
			}
		}

		else
		{
			yield return new WaitForSeconds(_secondsBeforeFirstAttack);
			if (IsFoeNearTarget())
			{
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

	private bool IsFoeNearTarget() => _distanceTargetAgent <= _agent.stoppingDistance;

}
